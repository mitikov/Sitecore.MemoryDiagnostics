namespace Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders
{
  using System;
  using System.Linq;
  using System.Runtime;
  using System.Runtime.CompilerServices;
  using System.Text;
  using Microsoft.Diagnostics.Runtime;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class StringReader : IPrimitiveEntityReader
  {
    [ThreadStatic]
    protected static byte[] stringLenBuffer;

    [ThreadStatic]
    protected static byte[] StringReadBatchSizeBuffer;

    [ThreadStatic]
    protected static byte[] WholeStringByteBuffer;

    public Type SupportedType => typeof(string);

    public static string ReadStringSafe(ClrHeap heap, ulong strAddress, int maxCharLimit = 3000)
    {
      if (strAddress == 0)
      {
        return TextConstants.NullReferenceString;
      }

      ulong ps = (ulong)heap.PointerSize;

      byte[] stringLengthBuffer = EnsureThreadStaticSufficientSize(sizeof(Int32), ref stringLenBuffer);

      var strLengAddress = strAddress + ps;

      // Get length of string
      var bytesReadTotal = heap.ReadMemory(strLengAddress, stringLengthBuffer, 0, sizeof(Int32));
      int len = 0;
      for (int i = bytesReadTotal - 1; i >= 0; i--)
      {
        len = len << 8;
        len += stringLengthBuffer[i];
      }

      if (len == 0)
      {
        return string.Empty;
      }
      if (len < 0)
      {
        return "[CorruptedString] " + strAddress.ToString("x8");
      }

      var maxBytesToRead = Math.Min(len, maxCharLimit) * 2;
      var readBatchSize = Math.Min(4096, maxBytesToRead);

      var dataStart = strLengAddress + sizeof(Int32);

      // Copy to local value 
      var readCharBuffer = EnsureThreadStaticSufficientSize(maxBytesToRead, ref WholeStringByteBuffer);

      int lastReadAdded = 0;

      bytesReadTotal = 0;

      var readBuffer = EnsureThreadStaticSufficientSize(readBatchSize, ref StringReadBatchSizeBuffer);

      while (bytesReadTotal < maxBytesToRead)
      {
        var readBytes = heap.ReadMemory(dataStart, readBuffer, 0, readBatchSize);
        if (readBytes >= 2)
        {
          for (int i = 0; i < readBytes - 1; i += 2)
          {
            if ((readBuffer[i] | readBuffer[i + 1]) == 0)
            {
              // 'End of line is met'
              // We are not near the end, thus string length is different from actual one.
              // lets return 
              return "[CorruptedString] " + strAddress.ToString("x8");
              return Encoding.Unicode.GetString(readCharBuffer.TakeWhile(t => t != 0).ToArray());
            }

            readCharBuffer[lastReadAdded++] = readBuffer[i];
            readCharBuffer[lastReadAdded++] = readBuffer[i + 1];
          }
        }

        bytesReadTotal += readBytes;
        readBatchSize = Math.Min(4096, maxBytesToRead - bytesReadTotal);
        if (readBatchSize <= 0)
        {
          return Encoding.Unicode.GetString(readCharBuffer, 0, maxBytesToRead);
        }
        dataStart += (ulong)readBytes;
      }

      var str = Encoding.Unicode.GetString(readCharBuffer, 0, maxBytesToRead);
      return str;
    }

    public object Read(ClrObject obj, string fldName)
    {
      return obj.GetStringFld(fldName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("PerformanceCritical")]
    protected static byte[] EnsureThreadStaticSufficientSize(int maxBytesToRead, ref byte[] array)
    {
      if ((array == null) || (maxBytesToRead > array.Length))
      {
        array = new byte[maxBytesToRead];
      }
      else
      {
        // Array.Clear(array, 0, maxBytesToRead);
      }

      return array;
    }
  }
}
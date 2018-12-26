namespace Sitecore.MemoryDiagnostics.Helpers
{
  using System;
  using System.Diagnostics;
  using Microsoft.Diagnostics.Runtime;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   Extracts <see cref="ClrObject" />s from given array pointer
  /// </summary>
  public class EnumerationHelper
  {
    public static readonly EnumerationHelper Instance = new EnumerationHelper();

    public virtual ClrObject[] DoEnumerateArrayType(ClrObject clrObject)
    {
      if ((clrObject.Type == null) || (clrObject.Type.ComponentType == null))
        return null;
      if (!clrObject.Type.ComponentType.IsValueClass)
        return EnumerateArrayOfClassInstances(clrObject);


#if TRACE
      Trace.TraceInformation("About to enumerate {0} of {1} value-typed array.", clrObject.Address.ToString("x8"), clrObject.Type.Name);
#endif
      int length = clrObject.Type.GetArrayLength(clrObject.Address);
      if (length < 0)
        return null;
      var arr = new ClrObject[length];

#if TRACE
      Trace.TraceInformation("{0} length of {1}", length, clrObject.Address.ToString("x8"));
#endif

      for (int i = 0; i < length; i++)
      {
        ulong reference = clrObject.Type.GetArrayElementAddress(clrObject.Address, i);
        if (reference == default(ulong))
          continue;
        arr[i] = new ClrObject(reference, clrObject.Type.ComponentType);
      }

      return arr;
      throw new NotImplementedException("TODO: AddOrMerge support for enumerating valueTypes");
    }

    public static ClrObject[] EnumerateArrayType(ClrObject clrObject)
    {
      return Instance.DoEnumerateArrayType(clrObject);
    }


    protected virtual ClrObject[] EnumerateArrayOfClassInstances(ClrObject clrObject)
    {
      try
      {
        return ClrCollectionHelper.EnumerateArrayOfRefTypes(clrObject).ToArray();
      }
      catch (Exception)
      {

        return ExecuteFallbackLogic(clrObject);


      }
    }

    protected virtual ClrObject[] ExecuteFallbackLogic(ClrObject clrObject)
    {
      int length;
      if (this.IsEmptyArray(clrObject, out length))
      {
        return new ClrObject[0];
      }

      var res = new ClrObject[length];

      ClrHeap heap = clrObject.Type.Heap;
      int pointerSize = heap.PointerSize;

      ulong arrayStart = this.CalculateArrayStart(clrObject, pointerSize);

      var isValueType = clrObject.Type.ComponentType.IsValueClass;

      var shiftSize = isValueType
        ? clrObject.Type.ComponentType.ElementSize
        : pointerSize;

      ulong dataReadPosition = arrayStart;
      for (int i = 0; i < length; i++)
      {
        dataReadPosition += (ulong)shiftSize;

        ulong elemAddress;
        ClrObject clrObj;
        if (!isValueType)
        {
          heap.ReadPointer(dataReadPosition, out elemAddress);
          if (elemAddress == default(ulong))
            continue;
          clrObj = new ClrObject(elemAddress, clrObject.Type.Heap);
        }
        else
        {
          clrObj = new ClrObject(dataReadPosition, clrObject.Type.Heap);
        }

        // object val = elemType.GetValue(elemAddress);
        // if (val == default(object) )
        // continue;
        // TODO: think should we skip object without type ?
        if (clrObj.Type == null)
          continue;
        res[i] = clrObj;
      }

      return res;
    }

    protected virtual ulong CalculateArrayStart(ClrObject clrObject, int pointerSize)
    {
      /*
  * ARRAY LAYOUT IN MEMORY:
  *      ARRAY_TYPE_MT_POINTER (f.e. String[])
  *      _SizeOfArray ( int64 - Alligned to pointer size )
  *      ARRAY_Elem_Type_MT_POINTER ( f.e. String )
  *      0_ElemPointer, or elem itself
  *      1_ElemPointer, or elem itself
  */
      return (ulong)pointerSize * 2 + clrObject.Address;
    }

    protected virtual bool IsEmptyArray(ClrObject clrObject, out int length)
    {
      length = clrObject.Type.GetArrayLength(clrObject.Address);
      return length <= 0;
    }
  }
}
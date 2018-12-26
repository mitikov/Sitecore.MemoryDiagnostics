namespace Sitecore.MemoryDiagnostics
{
  using System;
  using System.Linq;
  using System.Runtime;
  using System.Runtime.CompilerServices;
  using Sitecore;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Handy string-related utilities.
  /// </summary>
  public class StringUtil
  {
    /// <summary>
    /// Transforms size in byes into human-readable manner - KB, MB, GB sizes.
    /// </summary>
    /// <param name="size">Amout of bytes to translate.</param>
    /// <returns>Bytes converted into higher-granular units (f.e. Megabytes, Gigabytes).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static string GetSizeString(long size)
    {
      if (size < 1024)
        return size + " B";

      double res = ((double)size) / 1024;
      if (res < 1000)
        return Math.Round(res, 3) + " KB";

      res = res / 1024;
      if (res < 1000)
        return Math.Round(res, 3) + " MB";

      res = res / 1024;

      return Math.Round(res, 3) + " GB";
    }

    /// <summary>
    /// Transforms size in byes into human-readable manner - KB, MB, GB sizes.
    /// </summary>
    /// <param name="size">Amout of bytes to translate.</param>
    /// <returns>Bytes converted into higher-granular units (f.e. Megabytes, Gigabytes).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static string GetSizeString(ulong size)
    {
      if (size < 1024)
        return size + " B";

      double res = ((double)size) / 1024;
      if (res < 1000)
        return Math.Round(res, 3) + " KB";

      res = res / 1024;
      if (res < 1000)
        return Math.Round(res, 3) + " MB";

      res = res / 1024;

      return Math.Round(res, 3) + " GB";
    }


    /// <summary>
    /// Gets the size of <paramref name="stringBody"/> in bytes.
    /// </summary>
    /// <param name="stringBody">Text to get its size.</param>
    /// <returns>Number of bytes this text consumes in memory.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static long GetStringSize([NotNull] string stringBody)
    {
      var stringSize = stringBody.Length * 2 + 26;
      return ((stringSize + 7) >> 3) << 3;
    }

    /// <summary>
    /// Counts the number of lines in given <paramref name="text"/>.
    /// <para>Returns one more than 'newline' char found times.</para>
    /// </summary>
    /// <param name="text">Text to figure out how many lines would it take.</param>
    /// <returns>Count of lines needed to display the text.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static int LinesCount([CanBeNull] string text)
    {
      if (string.IsNullOrWhiteSpace(text))
      {
        return 1;
      }

      int n = text.Count(c => c == '\n');
      return n + 1;
    }

    [NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static string ProduceAutoPropertyName([NotNull] string fldName)
    {
      Assert.ArgumentNotNullOrEmpty(fldName, "fieldName");

      return string.Format("<{0}>k__BackingField", fldName);
    }

    [NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static string StripNamespaceFromTypeFullName([NotNull] string fullname)
    {
      Assert.ArgumentNotNullOrEmpty(fullname, "fullname");

      if (fullname.IndexOf("<") > 0)
      {
        return fullname; // generic type
      }

      var lastIndexOfDot = fullname.LastIndexOf('.');
      return lastIndexOfDot < 1 ? fullname : fullname.Substring(lastIndexOfDot + 1);
    }

    /// <summary>
    /// Trims <paramref name="inputText"/> to given <paramref name="maxLength"/>.    
    /// </summary>
    /// <param name="inputText"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static string TrimTo(string inputText, int maxLength = 60)
    {
      if (string.IsNullOrEmpty(inputText) || (inputText.Length <= maxLength))
      {
        return inputText;
      }
      else
      {
        return inputText.Substring(0, maxLength);
      }
    }
  }
}
namespace Sitecore.MemoryDiagnostics.Extensions
{
  using System;
  using System.Runtime;
  using System.Runtime.CompilerServices;

  public static class StringExtensions
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Pefrormace critical")]
    public static string FallbackTo(this string value, string fallback)
    {
      if (!string.IsNullOrEmpty(value))
        return value;

      return string.IsNullOrEmpty(fallback) ? "[EmptyFallback]" : fallback;
    }

    /// <summary>
    /// Determines whether <see cref="string.IsNullOrEmpty"/>, or equals to <see cref="TextConstants.NullReferenceString"/> - fallback value by StringReader.
    /// </summary>
    /// <param name="value">The value to be checked</param>
    /// <returns></returns>    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Pefrormace critical")]
    public static bool IsNullValue(this string value)
    {
      return string.IsNullOrEmpty(value) || value.Equals(TextConstants.NullReferenceString, StringComparison.OrdinalIgnoreCase);
    }
  }
}
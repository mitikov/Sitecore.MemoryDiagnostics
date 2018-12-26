namespace Sitecore.MemoryDiagnostics.Extensions
{
  using System;
  using System.Collections.Generic;

  public static class EnumerableExtensions
  {
    public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
    {
      foreach (T i in ie)
      {
        action(i);
      }
    }

    public static IEnumerable<T> WithoutEmpty<T>(this IEnumerable<T> ie)
    {
      if (ie == null)
      {
        yield break;
      }

      foreach (T i in ie)
      {
        var hasDefaultValue = EqualityComparer<T>.Default.Equals(i, default(T));
        if (!hasDefaultValue)
        {
          yield return i;
        }
      }
    }
  }
}
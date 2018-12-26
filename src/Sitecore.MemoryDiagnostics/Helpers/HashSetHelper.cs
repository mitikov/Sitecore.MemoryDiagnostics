namespace Sitecore.MemoryDiagnostics.Helpers
{
  using System.Collections.Generic;
  using System.Reflection;  

  /// <summary>
  /// A set of hacky-helpers for <see cref="HashSet{T}"/> class.
  /// </summary>
  public static class HashSetHelper
  {
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

    /// <summary>
    /// Creates a new <see cref="HashSet{T}"/> with a preset capacity to prevent further growth.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="capacity">The capacity.</param>
    /// <returns></returns>
    [NotNull]
    public static HashSet<T> GetHashSet<T>(int capacity)
    {
      return new HashSet<T>().SetCapacity(capacity);
    }

    /// <summary>
    /// Sets the capacity for the <see cref="HashSet{T}"/> to prevent further array relocations during growth.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="hs">The hs.</param>
    /// <param name="capacity">The capacity.</param>
    /// <returns></returns>
    [NotNull]
    public static HashSet<T> SetCapacity<T>(this HashSet<T> hs, int capacity)
    {
      var initialize = hs.GetType().GetMethod("Initialize", Flags);
      initialize.Invoke(hs, new object[]
      {
        capacity
      });
      return hs;
    }
  }
}
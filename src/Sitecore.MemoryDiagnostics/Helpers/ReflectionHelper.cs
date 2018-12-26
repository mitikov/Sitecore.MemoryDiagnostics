namespace Sitecore.MemoryDiagnostics.Helpers
{
  using System;
  using System.Linq;
  using System.Reflection;

  /// <summary>
  /// A set of <see cref="System.Reflection"/> hacks.
  /// </summary>
  [CLSCompliant(true)]
  public class ReflectionHelper
  {
    [NotNull]
    public static Type[] FilterTypesByBaseClassOrInterface([CanBeNull] Type filterType, [CanBeNull] Type[] toFilter)
    {
      if (toFilter == null)
      {
        return new Type[0];
      }

      if (filterType == null)
      {
        return toFilter;
      }

      if (filterType.IsValueType)
      {
        return new Type[0];
      }

      Type[] res = toFilter.Where(filterType.IsAssignableFrom).ToArray();
      return res;
    }

    [NotNull]
    public static Type[] GetNotAbstractClassTypesFromNamespace([CanBeNull] string namesp,
      bool includeNestedNamespaces = true)
    {
      if (string.IsNullOrEmpty(namesp))
      {
        return new Type[0];
      }

      return (from t in typeof(ReflectionHelper).Assembly.GetTypes()
        where
        (t != null)
        && t.IsClass
        && (!t.IsAbstract)
        && !string.IsNullOrEmpty(t.Namespace)
        && (includeNestedNamespaces
          ? t.Namespace.StartsWith(namesp, StringComparison.Ordinal)
          : t.Namespace.Equals(namesp, StringComparison.Ordinal))
        select t).ToArray();
    }

    /// <summary>
    /// Sets the type of the value for value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field">The field.</param>
    /// <param name="item">The item.</param>
    /// <param name="value">The value.</param>
    public static void SetValueForValueType<T>(FieldInfo field, ref T item, object value) where T : struct
    {
      field.SetValueDirect(__makeref(item), value);
    }
  }
}
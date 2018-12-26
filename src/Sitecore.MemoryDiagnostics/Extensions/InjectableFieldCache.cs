namespace Sitecore.MemoryDiagnostics
{
  using System;
  using System.Collections;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.CompilerServices;
  using Sitecore.MemoryDiagnostics.Attributes;  

  /// <summary>
  ///   Has a collection of <see cref="FieldInfo" /> per <see cref="Type"/> that has <see cref="InjectFieldValueAttribute" /> set.
  ///   <para>Check for attribute presence is cheap. Deserialization of attribute parameters is costly.</para>
  /// </summary>
  [NotThreadSafe]
  public sealed class InjectableFieldsCache
  {
    private static readonly FieldInfo[] NoFieldInfo = new FieldInfo[0];

    private readonly Hashtable innerCacheStorage = new Hashtable();

    [NotNull]
    public FieldInfo[] this[[CanBeNull] object obj] => this.GetInjectableFields(obj);

    [NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FieldInfo[] GetInjectableFields([CanBeNull] object obj)
    {
      return obj == null ? NoFieldInfo : this.GetInjectableFields(obj.GetType());
    }

    [NotNull]
    public FieldInfo[] GetInjectableFields([CanBeNull] Type target)
    {
      if (target == null)
      {
        return NoFieldInfo;
      }

      if (!this.innerCacheStorage.ContainsKey(target))
      {
        this.innerCacheStorage[target] = this.FindInjectableFieldsInType(target);
      }

      var res = this.innerCacheStorage[target] as FieldInfo[];
      return res ?? NoFieldInfo;

      // TODO: what if res = null ?!
    }

    [NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private FieldInfo[] FindInjectableFieldsInType([NotNull] Type target)
    {
      return target.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(t =>
          t.IsDefined(typeof(InjectFieldValueAttribute))).ToArray();
    }
  }
}
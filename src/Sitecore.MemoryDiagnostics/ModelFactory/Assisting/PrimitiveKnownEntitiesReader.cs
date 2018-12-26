namespace Sitecore.MemoryDiagnostics.ModelFactory.Assisting
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.CompilerServices;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Helpers;
  using Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders;
  using Sitecore;
  using Sitecore.Data;
  using Sitecore.Diagnostics;
  using Sitecore.StringExtensions;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   Reads <see cref="Guid" />,<see cref="ID" />,<see cref="DateTime" /> and other known entities from memory dump.
  ///   <para>Allows injecting known types into model fields. F.e. DateTime from MD to DateTime field in model.</para>
  ///   <para>Consists of <see cref="IPrimitiveEntityReader" /> class instances.</para>
  ///   <para>
  ///     Checks if target field type is in <see cref="KnownEntityFieldInjectors" /> keys. Corresponding value func (
  ///     <see cref="IPrimitiveEntityReader.Read" /> ) is called to get the resulting value in case field is found.
  ///   </para>
  /// </summary>
  [NotThreadSafe]
  public class PrimitiveKnownEntitiesReader
  {
    protected Dictionary<Type, Func<ClrObject, string, object>> KnownEntityFieldInjectors;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PrimitiveKnownEntitiesReader()
      : this(TextConstants.PrimitiveEntitiesNamespace)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PrimitiveKnownEntitiesReader([CanBeNull] string namespaceToLoadFrom)
      : this(ReflectionHelper.GetNotAbstractClassTypesFromNamespace(namespaceToLoadFrom))
    {
    }

    public PrimitiveKnownEntitiesReader([CanBeNull] Type[] readers)
    {
      if (InitInjectorsDictionaryCollectionEmpty(readers))
        return;

      foreach (Type tmp in readers)
      {
        AddOrUpdateReader(tmp);
      }
    }

    public PrimitiveKnownEntitiesReader([CanBeNull] IPrimitiveEntityReader[] readers)
    {
      if (InitInjectorsDictionaryCollectionEmpty(readers))
        return;

      foreach (IPrimitiveEntityReader tmp in readers)
      {
        AddOrUpdateReader(tmp);
      }
    }

    [NotNull]
    public Type[] SupportedTypeList
    {
      get
      {
        lock (KnownEntityFieldInjectors)
        {
          return KnownEntityFieldInjectors.Keys.ToArray();
        }
      }
    }

    [NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PrimitiveKnownEntitiesReader AddOrUpdateReader([NotNull] IPrimitiveEntityReader reader)
    {
      Assert.ArgumentNotNull(reader, "reader");

      KnownEntityFieldInjectors[reader.SupportedType] = reader.Read;
      return this;
    }

    [NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PrimitiveKnownEntitiesReader AddOrUpdateReader([NotNull] Type type)
    {
      Assert.ArgumentNotNull(type, "type");
      try
      {
        var candidate = Activator.CreateInstance(type) as IPrimitiveEntityReader;
        if (candidate != null)
        {
          return AddOrUpdateReader(candidate);
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex, "Could not create knownEntityReader. Type {0}".FormatWith(new object[]
        {
          type.FullName
        }));
      }

      return this;
    }

    [NotNull]
    public PrimitiveKnownEntitiesReader AddOrUpdateReaders([NotNull] Type[] types)
    {
      Assert.ArgumentNotNull(types, "types");
      foreach (Type type in types)
      {
        AddOrUpdateReader(type);
      }

      return this;
    }

    [NotNull]
    public PrimitiveKnownEntitiesReader AddOrUpdateReaders([NotNull] IPrimitiveEntityReader[] types)
    {
      Assert.ArgumentNotNull(types, "types");
      foreach (IPrimitiveEntityReader type in types)
      {
        AddOrUpdateReader(type);
      }

      return this;
    }

    public virtual bool TryCalcFieldValue([NotNull] FieldInfo fldInfo, ClrObject obj, [CanBeNull] out object value)
    {
      Assert.ArgumentNotNull(fldInfo, "Field info is null.");
      value = null;

      if (!CanInject(fldInfo))
        return false;
      try
      {
        Func<ClrObject, string, object> calcFunc = KnownEntityFieldInjectors[fldInfo.FieldType];

        value = calcFunc(obj, fldInfo.Name);

        return true;
      }
      catch (Exception ex)
      {
        Logger.SingleError(ex,
          "Cannot read {0} field of {1} object".FormatWith(new object[]
          {
            fldInfo.Name, obj.Address.ToString("X")
          }));
        return false;

        // throw;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool CanInject([NotNull] FieldInfo fldInfo)
    {
      Assert.ArgumentNotNull(fldInfo, "fieldInfo");
      return CanInject(fldInfo.FieldType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool CanInject([NotNull] Type fldType)
    {
      Assert.ArgumentNotNull(fldType, "No type provided");
      return KnownEntityFieldInjectors.ContainsKey(fldType);
    }

    private bool InitInjectorsDictionaryCollectionEmpty([CanBeNull] IEnumerable readers)
    {
      int countToInit = 0;
      if (readers != null)
      {
        countToInit += readers.Cast<object>().Count();
      }

      KnownEntityFieldInjectors = new Dictionary<Type, Func<ClrObject, string, object>>(countToInit);
      return countToInit == 0;
    }
  }
}
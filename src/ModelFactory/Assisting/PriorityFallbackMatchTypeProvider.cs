namespace Sitecore.MemoryDiagnostics.ModelFactory.Assisting
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Reflection;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Helpers;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Diagnostics;
  
  using Sitecore.StringExtensions;

  /// <summary>
  ///   Provides mapping between object type in MD and class model.
  ///   <para>
  ///     Model must have <see cref="ModelMappingAttribute" /> and implement <see cref="IClrObjMappingModel" />
  ///     interface to be valid for adding into provider.
  ///   </para>
  ///   Key - source (MD) class name, Value - matching model
  /// </summary>
  [DebuggerDisplay("{Priority.Count} Priority. {Fallback.Count} - Fallback")]
  public class PriorityFallbackMatchTypeProvider
  {
    protected Dictionary<string, Type> Fallback;

    /// <summary>
    ///   Key - typeToMapOn.
    ///   value - <see cref="IClrObjMappingModel" /> class.
    /// </summary>
    protected Dictionary<string, Type> Priority;

    public PriorityFallbackMatchTypeProvider()
      : this((string)null)
    {
    }

    public PriorityFallbackMatchTypeProvider([CanBeNull] string namespaceForPriority)
      : this(ReflectionHelper.GetNotAbstractClassTypesFromNamespace(namespaceForPriority))
    {
    }

    public PriorityFallbackMatchTypeProvider([CanBeNull] Type[] priority)
      : this(priority, TextConstants.FallbackModelsNamespace)
    {
    }

    public PriorityFallbackMatchTypeProvider([CanBeNull] Type[] priority, [CanBeNull] string fallbackNamespace)
      : this(priority, ReflectionHelper.GetNotAbstractClassTypesFromNamespace(fallbackNamespace))
    {
    }

    public PriorityFallbackMatchTypeProvider([CanBeNull] Type[] priority, [CanBeNull] Type[] fallback)
    {
      Priority = InitializeModelMappings(priority);
      Fallback = InitializeModelMappings(fallback);
    }

    public int FallbackModelMappingCount
    {
      get
      {
        return Fallback.Count;
      }
    }

    public int PriorityModelMappingCount
    {
      get
      {
        return Priority.Count;
      }
    }

    /// <summary>
    ///   Gets the supported type names.
    /// </summary>
    /// <value>
    ///   The supported type names.
    /// </value>
    [NotNull]
    public string[] SupportedTypeNames
    {
      get
      {
        string[] priority = SupportedTypeNamesPriority;
        string[] fallback = SupportedTypeNamesFallback;
        string[] rs = (from t in fallback
          where !priority.Contains(t)
          select t).ToArray();
        var res = new string[rs.Length + priority.Length];
        priority.CopyTo(res, 0);
        rs.CopyTo(res, priority.Length);
        return rs;
      }
    }

    [NotNull]
    public string[] SupportedTypeNamesFallback
    {
      get
      {
        return GetTypeNames(Fallback);
      }
    }

    [NotNull]
    public string[] SupportedTypeNamesPriority
    {
      get
      {
        return GetTypeNames(Priority);
      }
    }

    [CanBeNull]
    public Type this[[CanBeNull] string typeName]
    {
      get
      {
        return GetMatchingModel(typeName);
      }
    }

    [NotNull]
    public PriorityFallbackMatchTypeProvider AddOrUpdate([NotNull] Type entityModelCandidate)
    {
      Assert.ArgumentNotNull(entityModelCandidate, "entityModelCandidate");
      if (!CandidateTypeIsValid(entityModelCandidate))
        throw new ArgumentException(
          "{0} type is not valid as likely not implementing {1}".FormatWith(entityModelCandidate.Name,
            typeof(IClrObjMappingModel).Name));
      string typeName;
      if (ModelMappingAttribute.TryGetTypeToMapOn(entityModelCandidate, out typeName))
        Priority[typeName] = entityModelCandidate;
      else
      {
        throw new ArgumentException(
          "Could not read {0} attribute from {1} type.".FormatWith(typeof(ModelMappingAttribute).Name,
            entityModelCandidate.Name));
      }

      return this;
    }

    [NotNull]
    public PriorityFallbackMatchTypeProvider AddOrUpdate([NotNull] IClrObjMappingModel entityModelCandidate)
    {
      Assert.ArgumentNotNull(entityModelCandidate, "entityModelCandidate");
      return AddOrUpdate(entityModelCandidate.GetType());
    }

    [NotNull]
    public PriorityFallbackMatchTypeProvider AddOrUpdate([NotNull] IEnumerable<IClrObjMappingModel> entityModelCandidate)
    {
      Assert.ArgumentNotNull(entityModelCandidate, "entityModelCandidate");
      foreach (IClrObjMappingModel candidate in entityModelCandidate)
      {
        AddOrUpdate(candidate);
      }

      return this;
    }

    [NotNull]
    public PriorityFallbackMatchTypeProvider AddOrUpdate([NotNull] IEnumerable<Type> entityModelCandidate)
    {
      Assert.ArgumentNotNull(entityModelCandidate, "entityModelCandidate");
      foreach (Type candidate in entityModelCandidate)
      {
        AddOrUpdate(candidate);
      }

      return this;
    }

    public PriorityFallbackMatchTypeProvider RemoveMappingForSourceType([NotNull] string typeName)
    {
      Assert.ArgumentNotNullOrEmpty(typeName, "typeName");
      if (Priority.ContainsKey(typeName))
        Priority.Remove(typeName);
      return this;
    }

    public PriorityFallbackMatchTypeProvider RemoveMappingForSourceType(Type type)
    {
      Assert.ArgumentNotNull(type, "type");
      return RemoveMappingForSourceType(type.FullName);
    }

    [NotNull]
    protected static Dictionary<string, Type> InitializeModelMappings([CanBeNull] Type[] potentialEntityReaderTypes)
    {
      int len = (potentialEntityReaderTypes == null) ? 0 : potentialEntityReaderTypes.Length;

      var typeReaderDictionary = new Dictionary<string, Type>(len);

      if (potentialEntityReaderTypes == null)
        return typeReaderDictionary;

      foreach (Type entityReaderType in potentialEntityReaderTypes)
      {
        if (entityReaderType.IsValueType || (entityReaderType.IsAbstract))
          continue;

        string typeToMapModelOn = ModelMappingAttribute.GetTypeToMapOn(entityReaderType);

        if (string.IsNullOrEmpty(typeToMapModelOn))
          continue;

        if (typeof(IClrObjMappingModel).IsAssignableFrom(entityReaderType))
          typeReaderDictionary[typeToMapModelOn] = entityReaderType;
      }

      return typeReaderDictionary;
    }

    protected bool CandidateTypeIsValid([NotNull] Type entityReaderType)
    {
      if (entityReaderType.IsValueType || (entityReaderType.IsAbstract))
        return false;


      return
        entityReaderType.IsDefined(typeof(ModelMappingAttribute))
        && typeof(IClrObjMappingModel).IsAssignableFrom(entityReaderType);
    }

    [CanBeNull]
    protected virtual Type GetMatchingModel([CanBeNull] string typeFullName)
    {
      // TODO: Consider using base classes in converstion.
      if (string.IsNullOrEmpty(typeFullName))
        return null;
      if (Priority.ContainsKey(typeFullName))
        return Priority[typeFullName];

      return Fallback.ContainsKey(typeFullName) ? Fallback[typeFullName] : null;
    }

    [NotNull]
    private string[] GetTypeNames([CanBeNull] Dictionary<string, Type> candidate)
    {
      return candidate == null ? new string[0] : candidate.Keys.ToArray();
    }
  }
}
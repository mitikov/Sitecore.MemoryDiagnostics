namespace Sitecore.MemoryDiagnostics.Attributes
{
  using System;
  using System.Reflection;

  using Diagnostics;
  using Exceptions;
  using ModelFactory.Abstracts;
  using Models.BaseMappingModel;
  using Sitecore;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   Provides binding between <see cref="ClrObject" /> and <see cref="IClrObjMappingModel" /> via class name.
  ///   <para>Used by <see cref="IModelMapperFactory" />.</para>
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ModelMappingAttribute : Attribute
  {
    public readonly string TypeToMapOn;

    #region Constructors
    public ModelMappingAttribute([NotNull] string typeToMapOn)
    {
      Assert.ArgumentNotNullOrEmpty(typeToMapOn, "typeToMapOn");
      this.TypeToMapOn = typeToMapOn;
    }

    public ModelMappingAttribute([NotNull] Type typeToMapOn)
      : this(typeToMapOn.FullName)
    {
    }
    #endregion

    public static string GetTypeToMapOn([NotNull] Type candidate, bool assert = false)
    {
      Assert.ArgumentNotNull(candidate, "candidate");
      var attribute = candidate.GetCustomAttribute<ModelMappingAttribute>();

      if ((attribute == null) && assert)
      {
        throw new AttributeMissingException(candidate, typeof(ModelMappingAttribute));
      }

      return attribute == null ? string.Empty : attribute.TypeToMapOn;
    }

    public static bool TryGetTypeToMapOn([CanBeNull] Type candidate, out string typeToMapOn)
    {
      typeToMapOn = string.Empty;
      try
      {
        if (candidate == null)
        {
          return false;
        }

        var atr = candidate.GetCustomAttribute<ModelMappingAttribute>();
        if (string.IsNullOrEmpty(atr?.TypeToMapOn))
        {
          return false;
        }

        typeToMapOn = atr.TypeToMapOn;
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }  
}
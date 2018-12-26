namespace Sitecore.MemoryDiagnostics.ModelFactory
{
  using System.Reflection;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;
  using Sitecore;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Allows Lazy computing for fields decorated with <see cref="InjectFieldValueAttribute"/> and of <see cref="LazyLoad{T}"/>  type <see cref="IClrObjMappingModel"/>.
  /// <para><example><see cref="ScCache"/> content would be fetched only upon request.</example></para>
  /// </summary>
  public class LazyLoadModelMapperFactory : UnknownModelMapperFactory
  {
    /// <summary>
    /// Maps the field value to model, with LazyLoad support.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <param name="clrObject">The color object.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    protected override bool MapFieldValueToModel(
      [NotNull] FieldInfo fieldInfo,
      [ClrObjAndTypeNotEmpty] ClrObject clrObject,
      [NotNull] IClrObjMappingModel model)
    {
      if (fieldInfo.FieldType.IsGenericType &&
          (fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(LazyLoad<>)))
      {
        var lazy = new LazyLoad<IClrObjMappingModel>(
          o =>
          {
            object val;
            return this.ReadFieldValue(fieldInfo, o, out val) ? val as IClrObjMappingModel : null;
          },
          clrObject);

        fieldInfo.SetValue(model, lazy);
        return true;
      }

      return base.MapFieldValueToModel(fieldInfo, clrObject, model);
    }
  }
}
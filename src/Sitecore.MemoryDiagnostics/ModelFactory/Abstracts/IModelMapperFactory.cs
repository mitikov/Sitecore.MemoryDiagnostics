namespace Sitecore.MemoryDiagnostics.ModelFactory.Abstracts
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   Transforms <see cref="ClrObject" /> into corresponding <see cref="IClrObjMappingModel" />.
  ///   <para>
  ///     Injects fields into <see cref="IClrObjMappingModel" /> marked with <see cref="InjectFieldValueAttribute" />
  ///     attribute, and calls <see cref="IClrObjMappingModel.Compute" /> afterwards.
  ///   </para>
  /// </summary>
  public interface IModelMapperFactory
  {
    /// <summary>
    ///   Builds the corresponding model from provided <see cref="obj"/>.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    [NotNull]
    IClrObjMappingModel BuildModel(ClrObject obj);

    /// <summary>
    /// Reads the array. Handy API.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    ArrayMappingModel ReadArray(ClrObject obj);
  }
}
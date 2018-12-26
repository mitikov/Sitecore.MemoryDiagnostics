namespace Sitecore.MemoryDiagnostics.ModelFactory
{
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Transform <see cref="NoConverterForType"/> model into <see cref="GeneralMapping"/>.
  /// </summary>
  public class UnknownModelMapperFactory : SingleTypeModelMapperFactory
  {
    protected override IClrObjMappingModel DoBuildModelWrapped(ClrObject obj)
    {
      var result = base.DoBuildModelWrapped(obj);
      if (result is NoConverterForType)
      {
        return new GeneralMapping(obj);
      }

      return result;
    }
  }
}
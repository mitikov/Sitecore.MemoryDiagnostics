namespace Sitecore.MemoryDiagnostics.MetadataProviders
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public interface IRuntimeMetadataProvider<out T> where T : new()
  {
    [CanBeNull]
    T ExtractData([NotNull] ClrRuntime rn);

    T ExtractData([ClrObjAndTypeNotEmpty] ClrObject ClrObj);

    T ExtractData([NotNull] IClrObjMappingModel model);
  }
}
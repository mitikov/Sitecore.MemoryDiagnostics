namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Collections;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  [ModelMapping(typeof(DataProviderCollection))]
  public class DataProviderCollectionModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public ArrayMappingModel items;
  }
}
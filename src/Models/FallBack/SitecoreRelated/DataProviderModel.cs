namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Data.DataProviders;

  [ModelMapping(typeof(DataProvider))]
  public class DataProviderModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public ScCache prefetchCache;

    [InjectFieldValue]
    public ScCache propertyCache;
  }
}
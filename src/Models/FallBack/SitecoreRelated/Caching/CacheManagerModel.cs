namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Caching;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  [ModelMapping(typeof(CacheManager))]
  public class CacheManagerModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public ArrayMappingModel _cacheReferences;

    [InjectFieldValue]
    public long _runningTotal;
  }
}
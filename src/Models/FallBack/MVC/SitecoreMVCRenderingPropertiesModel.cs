namespace Sitecore.MemoryDiagnostics.Models.FallBack.MVC
{
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Data;

  // [ModelMapping(@"Sitecore.Mvc.Presentation.RenderingProperties")]
  public class SitecoreMVCRenderingPropertiesModel : ClrObjectMappingModel
  {
    public bool Cache_VaryByData;
    public bool Cacheable;

    public string CacheKey;

    public ID RenderingItemPath;
  }
}
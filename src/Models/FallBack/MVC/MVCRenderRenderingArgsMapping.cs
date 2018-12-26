namespace Sitecore.MemoryDiagnostics.Models.FallBack.MVC
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;

  [ModelMapping(@"Sitecore.Mvc.Pipelines.Response.RenderRendering.RenderRenderingArgs")]
  public class MVCRenderRenderingArgsMapping : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool _aborted;

    [InjectFieldValue]
    public bool Cacheable;

    [InjectFieldValue]
    public string CacheKey;

    [InjectFieldValue]
    public bool Rendered;

    [InjectFieldValue]
    public bool UsedCache;

    [InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> Rendering;

    [InjectFieldValue]
    protected LazyLoad<IClrObjMappingModel> _pageContext;
  }

  [ModelMapping(@"Sitecore.Mvc.Presentation.ControllerRenderer")]
  public class ControllerRendererMapping : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string ActionName;

    [InjectFieldValue]
    public string ControllerName;

  }
}
using System.Web;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.ModelFactory;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;

namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  [ModelMapping(@"System.Web.ThreadContext")]
  public class ThreadContextMappingModel : ClrObjectMappingModel
  {
    /// <summary>
    /// <see cref="HttpRuntime"/> unsets thread from HttpContext processing.
    /// </summary>
    [InjectFieldValue]
    public bool HasBeenDisassociatedFromThread;

    [InjectFieldValue]
    public bool _setCurrentThreadOnHttpContext;

    [InjectFieldValue]
    protected LazyLoad<IClrObjMappingModel> HttpContext;

    public HttpContextMappingModel AssignedHttpContext => this.HttpContext.Value as HttpContextMappingModel;
  }
}

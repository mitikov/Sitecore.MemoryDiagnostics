using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.SourceFactories;
using Sitecore.MemoryDiagnostics.Models.FallBack;
using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

namespace Sitecore.MemoryDiagnostics.API.Example.StatsProviders.Hardcore
{
  public class RequestsThatBeingProcessed
  {
    protected readonly IEnumerateClrObjectsFromClrRuntime ObjectEnumerator;

    protected readonly IFilteredObjectsProvider ObjectFilter;

    public RequestsThatBeingProcessed()
    {
      ObjectEnumerator = new HeapBasedClrObjectEnumerator();
      ObjectFilter = new FilteredObjectProviderByTypeName("System.Web.ThreadContext");
    }

    public virtual StringBuilder DoProcessing(MDFileConnection memoryDumpFileConnection)
    {
      var runtime = MDClrRuntimeFactory.Instance.BuildClrRuntime(memoryDumpFileConnection);

      var modelMapperFactory = ModelMapperManager.NewMapperFactory;

      var allThreadContext = ObjectFilter.ExtractFromRuntime(runtime, ObjectEnumerator);

      var gold = from threadContext in allThreadContext
                 let model = modelMapperFactory.BuildModel(threadContext) as ThreadContextMappingModel
                 where model != null
                 where model._setCurrentThreadOnHttpContext = true
                 let context = model.AssignedHttpContext
                 where context != null
                 where !context._finishPipelineRequestCalled /* NOT YET OVER */

                 let request = context._request as HttpRequestMappingModel
                 where request != null

                 let cookies = request._cookies.Value as HashtableMappingModel
                 where cookies != null

                 where cookies.Elements.ContainsKey(TextConstants.CookieNames.SitecoreAnalyticsGlobal)
                 // where cookies.Elements.ContainsKey(TextConstants.CookieNames.AspNetSession)

                 let analyticsCookie = cookies[TextConstants.CookieNames.SitecoreAnalyticsGlobal] as HttpCookieModel

                 let sessionCookie = cookies[TextConstants.CookieNames.AspNetSession] as HttpCookieModel

                 where !string.IsNullOrEmpty(analyticsCookie.Value)
                 // where !string.IsNullOrEmpty(sessionCookie.Value)

                 let workerRequest = context._wr as IIS7WorkerRequestModel

                 where workerRequest != null

                 let metadata = new
                 {
                   context.URL,
                   TotalSeconds = Math.Round(context.ExecutionDuration.TotalSeconds, 2),
                   analyticsId = analyticsCookie.Value,
                   aspSession = sessionCookie?.Value ?? "[NoSession]",
                   context = context.HexAddress,
                   request = request.HexAddress,
                   threadId = context.ManagedThreadId
                 }
                 where metadata.TotalSeconds > 0
                 orderby metadata.TotalSeconds descending


                 select metadata;

      var sb = new StringBuilder();
      foreach (var request in gold)
      {
        sb.AppendLine(new string(Enumerable.Repeat('*', 40).ToArray()));

        sb.AppendLine(request.ToString());

        sb.AppendLine();

      }

      return sb;
    }
  }
}

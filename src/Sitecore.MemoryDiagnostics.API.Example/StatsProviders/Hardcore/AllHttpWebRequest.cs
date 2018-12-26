using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.Extensions;
using Sitecore.MemoryDiagnostics.ModelComparers;
using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;
using Sitecore.MemoryDiagnostics.SourceFactories;

namespace Sitecore.MemoryDiagnostics.API.Example.StatsProviders.Hardcore
{
  public class AllHttpWebRequest
  {
    protected readonly IFilteredObjectsProvider ObjectFilter;

    public AllHttpWebRequest()
    {
      ObjectFilter = new FilteredObjectProviderByTypeName("System.Net.HttpWebRequest");
    }

    public virtual StringBuilder DoProcessing(MDFileConnection memoryDumpFileConnection)
    {
      var runtime = MDClrRuntimeFactory.Instance.BuildClrRuntime(memoryDumpFileConnection);

      var modelMapperFactory = ModelMapperManager.NewMapperFactory;

      var requests = ObjectFilter.EnumerateObjectsFromHeap<HttpWebRequestMappingModel>(memoryDumpFileConnection, modelMapperFactory)
        .Where(r => r.ContainUrl && !r.m_BodyStarted).OrderByDescending(r => r.StartTime);
        

      var sb = new StringBuilder();

      foreach (var request in requests)
      {
        sb.AppendLine($"Request: {request.Url}{Environment.NewLine}Started: {request.StartTime} Submited: {request.m_RequestSubmitted} OnceFailed: {request.m_OnceFailed}{Environment.NewLine} Body started:{request.m_BodyStarted} Hex: {request.HexAddress}");
      }
      return sb;
    }
  }
}

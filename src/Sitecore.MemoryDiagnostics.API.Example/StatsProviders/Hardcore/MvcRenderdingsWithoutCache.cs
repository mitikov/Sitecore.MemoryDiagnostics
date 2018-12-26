using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.Extensions;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;
using Sitecore.MemoryDiagnostics.Models.FallBack.MVC;
using Sitecore.MemoryDiagnostics.SourceFactories;

namespace Sitecore.MemoryDiagnostics.API.Example.StatsProviders.Hardcore
{
  public class MvcRenderdingsWithoutCache
  {

    protected readonly IFilteredObjectsProvider ObjectFilter;

    public MvcRenderdingsWithoutCache()
    {
      ObjectFilter = new FilteredObjectProviderByTypeName("Sitecore.Mvc.Pipelines.Response.RenderRendering.RenderRenderingArgs");
    }


    public virtual StringBuilder DoProcessing(MDFileConnection memoryDumpFileConnection)
    {
      var modelMapperFactory = ModelMapperManager.NewMapperFactory;

      var renderRenderingStreams = ObjectFilter.EnumerateObjectsFromHeap<MVCRenderRenderingArgsMapping>(memoryDumpFileConnection, modelMapperFactory);

      var sb = new StringBuilder();
      var gold = from args in renderRenderingStreams
                 where args.Rendered
                 where args.UsedCache == false
                 let pageContext = args.Obj.GetRefFld("_pageContext")
                 where !pageContext.IsNullObj
                 let innerHttpContext = pageContext.GetRefFldChained(new[] { "requestContext", "<HttpContext>k__BackingField", "_context" }, false)
                 where innerHttpContext.HasValue
                 let context = modelMapperFactory.BuildOfType<HttpContextMappingModel>(innerHttpContext.Value)
                 where context?.HasURL == true

                 let renderingPointer = args.Obj.GetRefFldChained(new[] { "<Rendering>k__BackingField", "renderer" }, false)
                 where renderingPointer.HasValue
                 let controllerRendered = modelMapperFactory.BuildOfType<ControllerRendererMapping>(renderingPointer.Value)

                 where controllerRendered != null

                 select new
                 {
                   args.HexAddress,
                   url = context.URL,
                   executed = context.ExecutionDuration.TotalSeconds,
                   controller = controllerRendered.ControllerName,
                   action = controllerRendered.ActionName,
                 };


      foreach (var tile in gold)
      {
        sb.Append(tile);
      }         

      return sb;
    }
  }
}

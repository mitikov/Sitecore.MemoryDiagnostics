using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
using Sitecore.MemoryDiagnostics.Extensions;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.FallBack.MVC;

namespace Sitecore.MemoryDiagnostics.API.Example
{
  public class RouteSample : RouteCollectionSample
  {
    public override void DoGrouping(string pathToMemoryDump, string pathToMscorDac)
    {

      var modelMapperFactory = CreateObjectMappingFactory();

      IFilteredObjectsProvider filter = new FilteredObjectByTypesProvider(
        WellknownTypeNames.MVC.Route,
        "System.Web.Http.WebHost.Routing.HttpWebRoute",
        "System.Web.Mvc.Routing.LinkGenerationRoute");

      var clrObjectMappingModelStream = modelMapperFactory.
      ExtractFromHeap(
      clrObjectFilter: filter,
      pathToDumpFile: pathToMemoryDump,
      pathToMsCord: pathToMscorDac);

      DoProcessing(clrObjectMappingModelStream);
      DoGrouping(pathToMemoryDump, pathToMscorDac, WellknownTypeNames.MVC.Route);
    }

    protected override void DoProcessing(IEnumerable<IClrObjMappingModel> clrObjectMappingModelStream)
    {
      var routeInfo = from route in clrObjectMappingModelStream.OfType<RouteMappingModel>()
                      let routeDefaultsTextView = (route.Defaults == null || route.Defaults.IsEmpty) ? "[no defaults]" : route.Defaults.ToString()
                      let routeDataTextView = (route.DataTokens == null || route.DataTokens.IsEmpty) ? "[no data tokens]" : route.DataTokens.ToString()
                      select new
                      {
                        url = route.UrlFormat,
                        routeAddress = route.HexAddress,
                        routeDefaultsTextView,
                        routeDataTextView,
                        route
                      };


      var sb = new StringBuilder();
      foreach (var routeData in routeInfo.OrderBy(r => r.url))
      {
        sb.Append(routeData);
        sb.AppendLine();
      }

      var text = sb.ToString();
      Console.Write(text);
    }
  }
}

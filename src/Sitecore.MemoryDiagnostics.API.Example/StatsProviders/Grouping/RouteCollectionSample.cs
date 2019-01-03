using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.MemoryDiagnostics.ModelFactory;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.FallBack.MVC;

namespace Sitecore.MemoryDiagnostics.API.Example
{
  /// <summary>
  /// Enumerates <see cref="WellknownTypeNames.MVC.RouteCollection"/> instances from heap.
  /// </summary>
  public class RouteCollectionSample : BaseGrouper
  {

    public virtual void DoGrouping(string pathToMemoryDump, string pathToMscorDac)
    {
      DoGrouping(pathToMemoryDump, pathToMscorDac, WellknownTypeNames.MVC.RouteCollection);
    }

    protected override ModelMapperFactory CreateObjectMappingFactory()
    {
      var theOne = base.CreateObjectMappingFactory();
      theOne.AddOrUpdate(typeof(RouteCollectionMappingModel));
      theOne.AddOrUpdate(typeof(RouteMappingModel));
      return theOne;
    }

    protected override void DoProcessing(IEnumerable<IClrObjMappingModel> clrObjectMappingModelStream)
    {
      var routeInfo = from routeCollection in clrObjectMappingModelStream.OfType<RouteCollectionMappingModel>()
                      let namedMap = routeCollection.Map
                      from keyValueRouteInfo in namedMap
                      let routeName = keyValueRouteInfo.Key
                      let route = keyValueRouteInfo.Value as RouteMappingModel
                      let routeDefaultsTextView = (route.Defaults == null || route.Defaults.IsEmpty) ? "[no defaults]" : route.Defaults.ToString()
                      let routeDataTextView = (route.DataTokens == null || route.DataTokens.IsEmpty) ? "[no data tokens]" : route.DataTokens.ToString()
                      where (routeName != null) && (route != null)
                      select new
                      {
                        routeName,
                        url = route.UrlFormat,
                        routeAddress = route.HexAddress,
                        routeDefaultsTextView,
                        routeDataTextView,
                        route
                      };

      var sb = new StringBuilder();
      foreach (var routeData in routeInfo)
      {
        sb.Append(routeData);
        sb.AppendLine();
      }

      var text = sb.ToString();
      Console.Write(text);
    }
  }
}

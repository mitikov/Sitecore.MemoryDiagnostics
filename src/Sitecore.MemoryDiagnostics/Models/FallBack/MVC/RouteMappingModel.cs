using System.Collections.Generic;
using System.Diagnostics;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.MVC
{
  /// <summary>
  /// Specifies how routing is processed in an ASP.NET application. 
  /// <para>You create a Route object for each URL pattern that you want to map to a class that can handle requests that correspond to that pattern.</para>
  /// <para>Link <a href="https://msdn.microsoft.com/en-us/library/system.web.routing.route">MSDN</a></para>
  /// </summary>
  [DebuggerDisplay("Route for: {_url}")]
  [ModelMapping(@"System.Web.Routing.Route")]
  public class RouteMappingModel : ClrObjectMappingModel
  {
    /// <summary>
    /// The route address. Sample format: "{controller}/{action}/{id}"
    /// </summary>
    [InjectFieldValue]
    protected string _url;

    /// <summary>
    /// The url parsed by MVC internal RouteParser.
    /// </summary>
    [InjectFieldValue]
    public ParsedRouteMappingModel _parsedRoute;

    /// <summary>
    /// An object containing default route values - to be used if nothing is specified.
    /// </summary>
    [InjectFieldValue]
    public RouteValueDictionaryMappingModel Defaults;

    /// <summary>
    /// An object containing constraints for route - sanity checks.
    /// <para>Example - specific HttpMethods (GET, and POST), or regex for specific parameter.</para>
    /// <para>See 'IRouteConstraint' interface for more details.</para>
    /// <para><a href="https://msdn.microsoft.com/en-us/library/system.web.routing.route.constraints">MSDN</a></para>
    /// </summary>
    [InjectFieldValue]
    public RouteValueDictionaryMappingModel Constraints;

    /// <summary>
    ///  Values associated with the route that are not used to determine whether a route matches a URL pattern.
    ///  <para>These values are passed to the route handler, where they can be used for processing the request.</para>
    ///  <para>Carries the namespaces associated with the route in under 'Namespaces' key.</para>
    ///  <para>In short - some values to be consumed during request processing.</para>
    /// </summary>
    [InjectFieldValue]
    public RouteValueDictionaryMappingModel DataTokens;

    /// <summary>
    /// The Url format this route should be applied for.
    /// </summary>
    public string UrlFormat => _url;

    /// <summary>
    /// The count of default parameters for the route.
    /// </summary>
    public int DefaultParamsCount => Defaults?.Count ?? 0;

    public int DataTokensCount => DataTokens?.Count ?? 0;

    public string[] Namespaces
    {
      get
      {
        if (DataTokens?.Contains("Namespaces") != true)
        {
          return new string[] { "[No namespaces defined]" };
        }

        var namespaces = DataTokens["Namespaces"] as ArrayMappingModel;
        var result = new List<string>();
        foreach (var element in namespaces.Elements)
        {
          if (element is StringModel)
          {
            result.Add(((StringModel)element).Value);
          }
          else
          {
            result.Add(element.ToString());
          }
        }

        return result.ToArray();
      }
    }

  }
  [ModelMapping(@"System.Web.Routing.RouteBase")]
  public class RouteBase : RouteMappingModel
  {
    /*HACK: will remove, promise */
  }

}

namespace Sitecore.MemoryDiagnostics
{
  public static class WellknownTypeNames
  {
    public static class IISSpecific
    {
      public const string IIS7WorkerRequest = @"System.Web.Hosting.IIS7WorkerRequest";
    }

    public static class MVC
    {
      public const string RouteCollection = @"System.Web.Routing.RouteCollection";

      public const string Route = @"System.Web.Routing.Route";
    }
  }
}

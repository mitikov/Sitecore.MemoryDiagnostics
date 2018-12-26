using HostingEnvironment = System.Web.Hosting.HostingEnvironment;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(typeof(HostingEnvironment))]
  public class HostingEnvironmentMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string __appPhysicalPath;

    [InjectFieldValue]
    public string _appId;

    [InjectFieldValue]
    public bool _isBusy;

    [InjectFieldValue]
    public bool _shutdownInitiated;

    [InjectFieldValue]
    public string _siteName;

    public override string Caption
    {
      get
      {
        return base.Caption + _siteName + " site";
      }
    }
  }
}
namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.Caching
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Security.AccessControl;

  [ModelMapping(typeof(AccessResult))]
  public class AccessResultModel : ClrObjectMappingModel
  {
    // TODO: Addd _explanation
    [InjectFieldValue]
    public AccessPermission _permission;

    public override string Caption
    {
      get
      {
        return base.Caption + _permission.ToString("G");
      }
    }
  }
}
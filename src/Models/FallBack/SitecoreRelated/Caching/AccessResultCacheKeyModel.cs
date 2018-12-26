namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Security.AccessControl;

  [ModelMapping(@"Sitecore.Caching.AccessResultCacheKey")]
  public class AccessResultCacheKeyModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public AccessRightModel AccessRight;

    [InjectFieldValue]
    public UserMappingModel Account;

    [InjectFieldValue]
    public string DatabaseName;

    [InjectFieldValue]
    public string EntityId;

    [InjectFieldValue]
    public string LongID;

    [InjectFieldValue]
    public PropagationType PropagationType;

    public string AccessRightCaption
    {
      get
      {
        return AccessRight == null ? AccessRightModel.NotSetAccessRight : AccessRight.Caption;
      }
    }

    public override string Caption
    {
      get
      {
        return base.Caption + string.Format("{0} for {1}", AccessRightCaption, EntityId ?? "[NoEntity]");
      }
    }
  }
}
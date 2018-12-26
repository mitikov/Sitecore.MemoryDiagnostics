namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.Caching
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Security.AccessControl;

  [ModelMapping(typeof(AccessResultCacheRecord))]
  public class AccessResultCacheRecordModelMapping : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public AccessResultModel Value;

    public override string Caption
    {
      get
      {
        return base.Caption + (Value == null ? "[NoAccessResult]" : Value.ToString());
      }
    }
  }
}
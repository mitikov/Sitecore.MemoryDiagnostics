namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(@"Sitecore.Data.DataProviders.NullRetryer")]
  public class NullRetryerModel : ClrObjectMappingModel
  {
    public override string ToString()
    {
      return "NullRetryer is set for the operation.";
    }
  }
}
namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  [ModelMapping("System.Data.ProviderBase.DbConnectionPool")]
  public class DbConnectionPoolModelMapping : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public ArrayMappingModel _objectList;
  }
}
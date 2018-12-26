namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  [ModelMapping("System.Data.ProviderBase.DbConnectionPoolGroup")]
  public class DbConnectionPoolGroupMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public SqlConnectionStringMappingModel _connectionOptions;

    [InjectFieldValue]
    public HashtableMappingModel _poolCollection;
  }
}
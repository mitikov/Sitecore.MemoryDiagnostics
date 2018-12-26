namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  [ModelMapping("System.Data.SqlClient.SqlConnectionFactory")]
  public class SqlConnectionFactoryModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public HashtableMappingModel _connectionPoolGroups;
  }
}
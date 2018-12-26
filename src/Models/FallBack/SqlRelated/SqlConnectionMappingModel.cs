using SqlConnection = System.Data.SqlClient.SqlConnection;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(typeof(SqlConnection))]
  public class SqlConnectionMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string _connectionString;

    [InjectFieldValue]
    public SqlInternalConnectionTdsMappingModel _innerConnection;

    [InjectFieldValue]
    public int ObjectID;
  }
}
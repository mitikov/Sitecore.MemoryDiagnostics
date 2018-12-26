namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping("System.Data.SqlClient.SqlConnectionPoolKey")]
  public class SqlConnectionPoolKeyModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string _connectionString;

    [InjectFieldValue]
    public int _hashValue;

    public override int GetHashCode()
    {
      return _hashValue;
    }
  }
}
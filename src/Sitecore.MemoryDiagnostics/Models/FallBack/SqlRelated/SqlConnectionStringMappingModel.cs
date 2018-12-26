namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [DebuggerDisplay("{_dataSource} / {_initialCatalog} {Obj.Address} {GetType().Name}")]
  [ModelMapping("System.Data.SqlClient.SqlConnectionString")]
  public class SqlConnectionStringMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public readonly string _usersConnectionString;

    [InjectFieldValue]
    public string _dataSource;

    [InjectFieldValue]
    public string _initialCatalog;


    [InjectFieldValue]
    public int _maxPoolSize;

    [InjectFieldValue]
    public int _minPoolSize;

    [InjectFieldValue]
    public bool _replication;
  }
}
using ConnectionState = System.Data.ConnectionState;
using DateTime = System.DateTime;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping("System.Data.SqlClient.SqlInternalConnectionTds")]
  public class SqlInternalConnectionTdsMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public DateTime _createTime;

    [InjectFieldValue]
    public IClrObjMappingModel _owningObject;

    [InjectFieldValue]
    public ConnectionState _state;
  }
}
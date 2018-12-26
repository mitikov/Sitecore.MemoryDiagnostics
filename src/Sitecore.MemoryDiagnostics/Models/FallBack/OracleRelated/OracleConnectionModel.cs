using ConnectionState = System.Data.ConnectionState;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.OracleRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.StringExtensions;

  [ModelMapping(@"Oracle.DataAccess.Client.OracleConnection")]
  public class OracleConnectionModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string m_databaseName;

    [InjectFieldValue]
    public bool m_disposed;

    [InjectFieldValue]
    public IClrObjMappingModel m_opoConCtx;

    [InjectFieldValue]
    public ConnectionState m_state;

    public override string Caption
    {
      get
      {
        return "{0} db OracleConnection #{1}".FormatWith(m_databaseName, HexAddress);
      }
    }

    public override string ToString()
    {
      return HexAddress + " connection is in {0} state for {1} database.".FormatWith(m_state, m_databaseName);
    }
  }
}
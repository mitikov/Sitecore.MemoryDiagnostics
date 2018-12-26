using StringBuilder = System.Text.StringBuilder;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.OracleRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(@"Oracle.DataAccess.Client.OracleDataReader")]
  public class OracleDataReaderModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool m_closed;

    [InjectFieldValue]
    public string m_commandText;

    [InjectFieldValue]
    public IClrObjMappingModel m_connection;

    [InjectFieldValue]
    public int m_recordCount;

    public override string Caption
    {
      get
      {
        return "Oracle Data Reader model " + Obj.HexAddress;
      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine("OracleDataReader is " + (m_closed ? "closed" : "opened"));
      sb.AppendLine((m_connection != null ? "has " + m_connection.Obj.HexAddress : "does not have") + " assosiated connection");
      sb.AppendLine("Command processed:");
      sb.Append(m_commandText);
      return sb.ToString();
    }
  }
}
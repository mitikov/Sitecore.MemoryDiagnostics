namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using System;
  using System.Data;
  using System.Data.SqlClient;
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;  

  /// <summary>
  ///   Base mapping for <see cref="System.Data.SqlClient.SqlCommand" /> class.
  /// </summary>
  [DebuggerDisplay("{_commandText}")]
  [ModelMapping(typeof(SqlCommand))]
  public class SqlCommandMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string _commandText;

    [InjectFieldValue]
    public int _commandTimeout;

    [InjectFieldValue]
    public CommandType _commandType;

    public override string Caption => base.Caption + StringUtil.TrimTo(this.CommandText);

    public string CommandText => string.IsNullOrEmpty(this._commandText) ? "[NoSqlCommandText]" : this._commandText;

    public override string ToString()
    {
      return string.Concat("Command text: ", CommandText, Environment.NewLine, "Command Timeout: ", _commandTimeout,
        Environment.NewLine, "Command Type: ", _commandType.ToString("G"));
    }
  }
}
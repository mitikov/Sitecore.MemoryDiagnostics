namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Attributes;
  using BaseMappingModel;
  using ModelFactory;
  using Sitecore;

  /// <summary>
  ///   <see cref="http://referencesource.microsoft.com/#System.Data/System/Data/SqlClient/SqlDataReader.cs,d576786f0fd1b1dc" />
  /// </summary>
  [ModelMapping(typeof(global::System.Data.SqlClient.SqlDataReader))]
  public class SqlDataReaderMappingModel : ClrObjectMappingModel
  {
    /// <summary>
    ///   Denote whether we have read first row for single row behavior
    /// </summary>
    [InjectFieldValue]
    public bool _haltRead;

    /// <summary>
    ///   Most likely an exception occurred during reading data.
    /// </summary>
    [InjectFieldValue]
    public bool _isClosed;

    /// <summary>
    ///   Is set after <see cref="global::System.Data.SqlClient.SqlDataReader" /> FinishExeucuteReader is called.
    ///   <para>Is true when command execution ended</para>
    /// </summary>
    [InjectFieldValue]
    public bool _isInitialized;

    [InjectFieldValue]
    public bool _metaDataConsumed;

    [InjectFieldValue]
    public int _recordsAffected;

    [InjectFieldValue]
    protected LazyLoad<IClrObjMappingModel> _command;

    [CanBeNull]
    public SqlCommandMappingModel Command => this._command?.Value as SqlCommandMappingModel;
  }
}
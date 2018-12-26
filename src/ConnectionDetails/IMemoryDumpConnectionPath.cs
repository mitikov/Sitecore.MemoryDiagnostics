namespace Sitecore.MemoryDiagnostics.ConnectionDetails
{
  /// <summary>
  /// Provides file-based connection to memory dump file.
  /// </summary>
  public interface IMemoryDumpConnectionPath
  {
    /// <summary>
    /// Gets the connection details string.
    /// </summary>
    /// <value>
    /// The connection details string.
    /// </value>
    [NotNull]
    string ConnectionDetailsString
    {
      get;      
    }

    /// <summary>
    ///   Gets the path to dump.
    /// </summary>
    /// <value>
    ///   The path to dump.
    /// </value>
    [NotNull]
    string PathToDump { get; }

    /// <summary>
    ///   Gets the path to mscordacwks.
    /// </summary>
    /// <value>
    ///   The path to mscord.
    /// </value>
    [CanBeNull]
    string PathToMsCorDacwks { get; }
  }
}
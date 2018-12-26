namespace Sitecore.MemoryDiagnostics.ConnectionDetails
{
  using Sitecore.MemoryDiagnostics.SourceFactories;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  //using Sitecore.LogAnalyzer.Settings;
  using SitecoreMemoryInspectionKit.Core.Diagnostics;

  /// <summary>
  ///   Memory dump file connection details.
  ///   <para>Allows to extract <see cref="ClrRuntime" /> via <see cref="IClrRuntimeFactory" />.</para>
  /// </summary>
  public class MDFileConnection : IMemoryDumpConnectionPath
  {
    /// <summary>
    ///   Dump File name.
    /// </summary>
    public readonly string DumpName;

    /// <summary>
    ///   Initializes a new instance of the <see cref="MDFileConnection" /> class.
    /// </summary>
    /// <param name="pathToDump">The path to dump.</param>
    /// <param name="pathToMsCorDacwks">The path to mscord.</param>
    public MDFileConnection([NotNull] string pathToDump, string pathToMsCorDacwks)
    {
      Assert.IsNotNullOrEmpty(pathToDump, "No dump path is specified");

      Assert.IsNotNullOrEmpty(pathToMsCorDacwks, "No mscord specified");

      this.PathToMsCorDacwks = pathToMsCorDacwks;

      this.PathToDump = pathToDump;

      this.DumpName = ExtractFileName(this.PathToDump);
    }

    /// <summary>
    ///   Gets the connection details string. Combines dumpName and PathTo Mscord.
    /// </summary>
    /// <value>
    ///   The connection details string.
    /// </value>
    public virtual string ConnectionDetailsString => $"{this.PathToDump}|{this.PathToMsCorDacwks}";

    /// <summary>
    ///   Gets the path to dump.
    /// </summary>
    /// <value>
    ///   The path to dump.
    /// </value>
    public string PathToDump { get; }

    /// <summary>
    ///   Gets the path to mscordacwks.
    /// </summary>
    /// <value>
    ///   The path to mscord.
    /// </value>
    public string PathToMsCorDacwks { get; }

    /// <summary>
    ///   Extracts the name of the file from path.
    /// </summary>
    /// <param name="filePathAndName">File path with file name</param>
    /// <returns>File name</returns>
    [NotNull]
    private static string ExtractFileName([NotNull] string filePathAndName)
    {
      Assert.IsNotNullOrEmpty(filePathAndName, "Cannot extract file name from empty string");

      return filePathAndName.Substring(filePathAndName.LastIndexOf('\\') + 1);
    }
  }
}
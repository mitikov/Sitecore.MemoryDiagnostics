namespace Sitecore.MemoryDiagnostics.SourceFactories
{
  using System;
  using System.IO;
  using Sitecore.MemoryDiagnostics.ConnectionDetails;
  using Microsoft.Diagnostics.Runtime;
  using Microsoft.Diagnostics.Runtime.Utilities;
  using Sitecore;
  using Sitecore.Diagnostics;

  /// <summary>
  ///   Transforms provided <see cref="MDFileConnection" /> into <see cref="ClrRuntime"/>.
  /// </summary>
  public class MDClrRuntimeFactory : IClrRuntimeFactory
  {
    #region Fields
    /// <summary>
    ///   A single instance of state-less object.
    /// </summary>
    private static MDClrRuntimeFactory _instance = new MDClrRuntimeFactory();
    #endregion

    #region Public Properties
    /// <summary>
    ///   Gets or sets instance of <see cref="MDClrRuntimeFactory" /> class.
    ///   <para>As class does not have any fields, it is safe to share one class instance application wide.</para>
    ///   <para>Never null.</para>
    /// </summary>
    [NotNull]
    public static MDClrRuntimeFactory Instance
    {
      get
      {
        return _instance;
      }

      set
      {
        Assert.IsNotNull(value, "Cannot set null to Instance");
        _instance = value;
      }
    }
    #endregion

    #region Public API
    #region Static methods
    /// <summary>
    /// Tries the get path to 'mscordacwks' file.
    /// </summary>
    /// <param name="pathToDumpFile">The path to dump file.</param>
    /// <param name="pathToMscord">The path to mscord.</param>
    /// <returns></returns>
    public static bool TryGetMscordacPath([CanBeNull] string pathToDumpFile, out string pathToMscord)
    {
      pathToMscord = string.Empty;
      if (string.IsNullOrEmpty(pathToDumpFile) && !File.Exists(pathToDumpFile))
      {
        return false;
      }

      try
      {
        var dump = DataTarget.LoadCrashDump(pathToDumpFile);
        dump.SymbolLocator = new DefaultSymbolLocator
        {
          SymbolCache = @"C:\symbols"
        };

        pathToMscord = dump.ClrVersions[0].LocalMatchingDac;

        return !string.IsNullOrEmpty(pathToMscord);
      }
      catch
      {
        return false;
      }
    }

    #endregion

    /// <summary>
    /// Builds runtime from provided connection details.
    /// </summary>
    /// <param name="pathToDump"></param>
    /// <param name="pathToMscord"></param>
    /// <returns></returns>
    public ClrRuntime BuildClrRuntime([NotNull] string pathToDump, [CanBeNull] string pathToMscord)
    {
      return this.BuildClrRuntime(new MDFileConnection(pathToDump, pathToMscord));
    }

    /// <summary>
    ///   Gets the source from provided connection strings.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns>Initialized runtime from provided connection details.</returns>
    /// <exception cref="System.ArgumentException">
    ///   in case <paramref name="settings" /> are not of
    ///   <see cref="MDFileConnection" /> type.
    /// </exception>
    [NotNull]
    public ClrRuntime BuildClrRuntime([NotNull] IMemoryDumpConnectionPath settings)
    {
      Assert.ArgumentNotNull(settings, "settings");
      if (settings is MDFileConnection == false)
      {
        throw new ArgumentException($"Expected {typeof(MDFileConnection).FullName}, recieved {settings.GetType().Name}");
      }

      var connectionDetails = settings as MDFileConnection;

      return this.BuildClrRuntimeCore(connectionDetails.PathToDump, connectionDetails.PathToMsCorDacwks);
    }

    #endregion
    /// <summary>
    ///   Creates the runtime by provided details.
    /// </summary>
    /// <param name="pathToDump">The path to dump.</param>
    /// <param name="pathToMscord">The path to mscord.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">When processor mismatch architecture (f.e. x86 vs x64 )</exception>
    /// <exception cref="ClrDiagnosticsException">DAC has wrong version</exception>
    [NotNull]
    protected virtual ClrRuntime BuildClrRuntimeCore([NotNull] string pathToDump, [CanBeNull] string pathToMscord)
    {
      Assert.IsNotNullOrEmpty(pathToDump, "No dump path is specified");

      var dump = DataTarget.LoadCrashDump(pathToDump);

      // dump.SymbolLocator = new DefaultSymbolLocator
      // {
      // SymbolCache = @"C:\symbols"
      // };
      Assert.IsNotNull(dump, "DataTarget is null");

      // TODO: NMI test. Migration to newer clrMD
      /*     
       * dump.SetSymbolPath(@"http://msdl.microsoft.com/download/symbols/");

     if (string.IsNullOrEmpty(pathToMscord))
        pathToMscord = dump.ClrVersions[0].TryDownloadDac();
      */
      var result = dump.ClrVersions[0].CreateRuntime(pathToMscord, ignoreMismatch: true);

      // Task.WaitAll(tasks: tasks.ToArray());
      return result;
    }
  }
}
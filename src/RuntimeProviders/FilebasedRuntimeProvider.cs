namespace Sitecore.MemoryDiagnostics.RuntimeProviders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading.Tasks;
  using Microsoft.Diagnostics.Runtime;
  using Microsoft.Diagnostics.Runtime.Utilities;
  
  using SitecoreMemoryInspectionKit.Core.Diagnostics;

  public class FilebasedRuntimeProvider
  {
    [NotNull]
    public ClrRuntime GetRuntime([NotNull] string pathToData)
    {
      Assert.ArgumentNotNullOrEmpty(pathToData, "No folder to load md");
      string md = pathToData + "w3wp.DMP";
      string mscord = pathToData + "mscordacwks.dll";

      return this.GetRuntime(md, mscord);
    }

    [NotNull]
    public virtual ClrRuntime GetRuntime([NotNull] string pathToDump, [CanBeNull] string pathToMsCordacwks)
    {
      Assert.ArgumentNotNullOrEmpty(pathToDump, "No folder to load md");

      DataTarget dump = DataTarget.LoadCrashDump(pathToDump);
      dump.SymbolLocator = new DefaultSymbolLocator
      {
        SymbolCache = @"C:\symbols"
      };
      bool mscordNotLoaded = string.IsNullOrWhiteSpace(pathToMsCordacwks) && !File.Exists(pathToMsCordacwks);
      if (mscordNotLoaded)
      {
        try
        {
          // pathToMsCordacwks = dump.ClrVersions[0].DacInfo.t();          
          if (!string.IsNullOrWhiteSpace(pathToMsCordacwks))
          {
            mscordNotLoaded = false;
          }
        }
        catch (Exception)
        {
          mscordNotLoaded = true;
        }
      }

      if (mscordNotLoaded)
      {
        throw new ArgumentException("Could not load mscordacwks. Path: " + pathToMsCordacwks);
      }

      var runtime = dump.ClrVersions[0].CreateRuntime(pathToMsCordacwks);
      var tasks = new List<Task>();
      foreach (ClrModule clrModule in runtime.Modules)
      {
        var task = dump.SymbolLocator.FindPdbAsync(clrModule.Pdb);
        tasks.Add(task);
      }

      // Task.WaitAll(tasks: tasks.ToArray());
      return runtime;
    }
  }
}
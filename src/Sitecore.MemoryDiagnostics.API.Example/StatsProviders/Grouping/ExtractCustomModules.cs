using System;
using System.Linq;
using System.Text;
using Microsoft.Diagnostics.Runtime;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.SourceFactories;

namespace Sitecore.MemoryDiagnostics.API.Example.StatsProviders.Grouping
{
  public class ExtractCustomModules
  {
    public virtual StringBuilder DoProcessing(MDFileConnection memoryDumpFileConnection)
    {
      var dump = DataTarget.LoadCrashDump(fileName: memoryDumpFileConnection.PathToDump);

      // dump.SymbolLocator = new DefaultSymbolLocator
      // {
      // SymbolCache = @"C:\symbols"
      // };


      // TODO: NMI test. Migration to newer clrMD
      /*     
       * dump.SetSymbolPath(@"http://msdl.microsoft.com/download/symbols/");

     if (string.IsNullOrEmpty(pathToMscord))
        pathToMscord = dump.ClrVersions[0].TryDownloadDac();
      */
      var runtime = dump.ClrVersions[0].CreateRuntime(memoryDumpFileConnection.PathToMsCorDacwks, ignoreMismatch: true);

      var dynamicModules = from module in runtime.Modules
                    where module.IsDynamic
                    where !module.IsFile
                    select module;

     var sb = new StringBuilder();

      foreach (var module in dynamicModules)
      {
        Console.WriteLine(module);
        sb.AppendLine(ProduceModuleInfo(module));
      }

      return sb;
    }

    private string ProduceModuleInfo(ClrModule module)
    {
      var sb = new StringBuilder();

      sb.AppendLine($"Module name: {(module.Name ?? "[NoModuleName]")}");
      sb.AppendLine($"Address: {module.MetadataAddress}");
      sb.AppendLine("Types read from module:");

      var intend = '\t';
      int number = 1;
      foreach (var type in module.EnumerateTypes())
      {
        sb.AppendLine($"{intend}{number} Name: {type.Name ?? "[NoTypeName]"}");
        sb.AppendLine($"{intend}{intend} IsInternal: {type.IsInternal}");
        sb.AppendLine($"{intend}{intend} IsPublic: {type.IsPublic}");
        number += 1;
        sb.AppendLine($"{intend}{intend} Methods:");

        foreach (var method in type.Methods)
        {
          sb.AppendLine($"{intend}{intend}{intend}{method.GetFullSignature() ?? "[NoSignature]"}");
        }
        sb.AppendLine($"{intend}{intend} Fields:");

        foreach (var field in type.Fields)
        {
          sb.AppendLine($"{intend}{intend}{intend}{field.Name ?? "[Name]"}");
        }

        sb.AppendLine("***************");
      }

      return sb.ToString();
    }

    private bool HasWeirdName(ClrModule module)
    {
      var name = module.Name;
      if (string.IsNullOrEmpty(name))
      {
        return false;
      }

      return ((name.IndexOf(',') == 8) && (name.EndsWith("Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")));
    }
  }
}

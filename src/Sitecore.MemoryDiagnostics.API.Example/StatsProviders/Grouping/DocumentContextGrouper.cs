using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.FallBack.JScripts;

namespace Sitecore.MemoryDiagnostics.API.Example.StatsProviders.Grouping
{
  /// <summary>
  /// Groups 'Microsoft.JScript.DocumentContext' instances by document name they represent.
  /// <para>By default, outputs statistics into the console.</para>
  /// </summary>
  public class JScriptDocumentContextGrouper : BaseGrouper
  {
    public void DoGrouping(string pathToMemoryDump, string pathToMscorDac)
    {
      base.DoGrouping(pathToMemoryDump, pathToMemoryDump, typeToExtract: "Microsoft.JScript.DocumentContext");
    }

    protected override void DoProcessing(IEnumerable<IClrObjMappingModel> clrObjectMappingModelStream)
    {
      var stats = from docContext in clrObjectMappingModelStream.OfType<DocumentContextModel>()
                  group docContext by docContext.documentName into grouped
                  let first = grouped.First()
                  let result = new
                  {
                    documentName = grouped.Key,
                    randomAddress = first.HexAddress,
                    hits = grouped.Count()
                  }
                  orderby result.hits descending
                  select result;

      foreach (var stat in stats)
      {
        Console.WriteLine(stat.ToString());
      }
      
    }
  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.FallBack.Reflection;

namespace Sitecore.MemoryDiagnostics.API.Example.StatsProviders.Grouping
{
  /// <summary>
  /// Processes dynamically generated assemblies.
  /// <para>Inspects instances of <see cref="System.Reflection.Emit.AssemblyBuilderData"/> type.</para>
  /// </summary>
  public class DynamicAssemblyGeneration : BaseGrouper
  {
    public void DoGrouping(string pathToMemoryDump, string pathToMscorDac)
    {
      base.DoGrouping(pathToMemoryDump, pathToMemoryDump, typeToExtract: "System.Reflection.Emit.AssemblyBuilderData");
    }

    protected string StatisticsOutputFileName => @"C:\PSS_casual\CurrentTickets\493211 Sitecore dotNet Runtime Error (in dmz environments)\assembly_info.txt";

    protected override void DoProcessing(IEnumerable<IClrObjMappingModel> clrObjectMappingModelStream)
    {
      var statsStream = from assemblyBuilder in clrObjectMappingModelStream.OfType<AssemblyBuilderDataModel>()
                        where assemblyBuilder.HasTypesDefined

                        select new
                        {
                          assemblyName = assemblyBuilder.m_strAssemblyName,
                          objHex = assemblyBuilder.HexAddress,
                          types =
                          Environment.NewLine + string.Join($",{Environment.NewLine}_______________{Environment.NewLine}",
                          assemblyBuilder.DefinedTypes.Select(type =>
                          {
                            var methodNames = string.Join($",{Environment.NewLine}", type.Methods.Select(method => method.MethodName));
                            return $"TYPE: {type.m_strName}{Environment.NewLine} METHODS: {methodNames} ";
                          }))
                        };
      
      var sb = new StringBuilder();

      var separator = string.Concat(Enumerable.Repeat(0, Console.BufferWidth).Select(i => '*'));
      foreach (var stat in statsStream)
      {        
        sb.AppendLine(separator);
        sb.AppendLine(stat.ToString());
      }

      File.WriteAllText(StatisticsOutputFileName, sb.ToString());
    }
  }
}

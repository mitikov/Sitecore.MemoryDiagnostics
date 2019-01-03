using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.Extensions;
using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.FallBack;
using Sitecore.MemoryDiagnostics.SourceFactories;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;
using SitecoreMemoryInspectionKit.Core.Diagnostics;

namespace Sitecore.MemoryDiagnostics.API.Example.StatsProviders.Grouping
{

  /// <summary>
  /// Prints <see cref="StringBuilder"/> grouped data (content, how many times met, random address) for those met over <see cref="_minimumTimeMetToPrint"/>.
  /// </summary>
  public class PrintStringBuilders : BaseGrouper
  {
    private readonly int _minimumTimeMetToPrint;
    private readonly IClrRuntimeFactory _runtimeFactory;
    private IModelMapperFactory _modelMapperFactory;

    public PrintStringBuilders(int minimumTimeMetToPrint = 3, IClrRuntimeFactory runtimeFactory = null, IModelMapperFactory modelMapperFactory = null)
    {
      _minimumTimeMetToPrint = minimumTimeMetToPrint;
      _runtimeFactory = runtimeFactory ?? MDClrRuntimeFactory.Instance;
      _modelMapperFactory = modelMapperFactory;
    }

    protected IModelMapperFactory ModelMapperFactory
    {
      get
      {
        if (_modelMapperFactory == null)
        {
          _modelMapperFactory = ModelMapperManager.NewMapperFactory;
        }

        Assert.IsNotNull(_modelMapperFactory, "modelMapperFactory");
        return _modelMapperFactory;
      }
    }

    public override void DoGrouping(string pathToMemoryDump, string pathToMscorDac, string typeToExtract)
    {
      var runtime = _runtimeFactory.BuildClrRuntime(new MDFileConnection(pathToMemoryDump, pathToMscorDac));
      var heap = runtime.Heap;

      var stringBuilderStream = new FilteredObjectProviderByTypeName(typeof(StringBuilder));

      var heapEnumerator = new HeapBasedClrObjectEnumerator();

      var uniqueBuilders = new HashSet<ulong>();
      var innerChunks = new HashSet<ulong>();

      var locateInnerBuilders = (from stringBuilderPointer in stringBuilderStream.ExtractFromRuntime(runtime, heapEnumerator)
                                 let previousChunk = stringBuilderPointer.GetRefFld("m_ChunkPrevious")
                                 where uniqueBuilders.Add(stringBuilderPointer.Address)
                                 where !previousChunk.IsNullObj && previousChunk.Address != stringBuilderPointer.Address
                                 where previousChunk.Type?.Name == stringBuilderPointer.Type.Name
                                 where innerChunks.Add(previousChunk.Address)
                                 select previousChunk)
            .ToArray();

      var topLevelBuilders = uniqueBuilders.Except(innerChunks);

      var factory = ModelMapperFactory;

      var builderContents = from u in uniqueBuilders
                            let clrObj = new ClrObject(u, heap)
                            let mapped = factory.BuildOfType<StringBuilderMappingModel>(clrObj)
                            where !mapped.IsEmpty()
                            select mapped;

      DoProcessing(builderContents);
    }

    protected override void DoProcessing(IEnumerable<IClrObjMappingModel> clrObjectMappingModelStream)
    {
      var grouped = from obj in clrObjectMappingModelStream
                    let mapped = obj as StringBuilderMappingModel
                    where mapped?.IsEmpty() == false
                    let sb = new
                    {
                      Address = mapped.Obj.HexAddress,
                      Content = mapped.TextContent,
                      mapped.TextContent.Length
                    }

                    group sb by sb.Content into sbByContent
                    let elem = sbByContent.First()

                    let stringBuilderStats = new
                    {
                      content = elem.Content,
                      address = elem.Address,
                      length = elem.Content.Length,
                      hits = sbByContent.Count(),
                      totalLength = sbByContent.Count() * elem.Content.Length
                    }

                    where stringBuilderStats.hits >= _minimumTimeMetToPrint

                    orderby stringBuilderStats.totalLength descending
                    select stringBuilderStats;

      foreach (var group in grouped)
      {
        Console.WriteLine(group);
        Console.WriteLine("============");
      }
    }
  }
}

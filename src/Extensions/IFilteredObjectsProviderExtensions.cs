using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.SourceFactories;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.Extensions
{
  public static class IFilteredObjectsProviderExtensions
  {
    public static IEnumerable<ClrObject> EnumerateObjectsFromHeap(this IFilteredObjectsProvider filteredObjectProvider, string pathToMemoryDump, string pathToMsCord)
    {      
      var connection = new MDFileConnection(pathToMemoryDump, pathToMsCord);

      return EnumerateObjectsFromHeap(filteredObjectProvider, connection);
    }

    public static IEnumerable<ClrObject> EnumerateObjectsFromHeap(this IFilteredObjectsProvider filteredObjectProvider, IMemoryDumpConnectionPath connection, IClrRuntimeFactory runtimeFactory = null )
    {
      runtimeFactory = runtimeFactory ?? MDClrRuntimeFactory.Instance;

      var runtime = MDClrRuntimeFactory.Instance.BuildClrRuntime(connection);

      var heapObjects = HeapBasedClrObjectEnumerator.Instance;

      return filteredObjectProvider.ExtractFromRuntime(runtime, heapObjects);
    }

    public static IEnumerable<T> EnumerateObjectsFromHeap<T>(this IFilteredObjectsProvider filteredObjectProvider, IMemoryDumpConnectionPath connection, IModelMapperFactory factory,  IClrRuntimeFactory runtimeFactory = null) where T: class, IClrObjMappingModel
    {
      runtimeFactory = runtimeFactory ?? MDClrRuntimeFactory.Instance;

      var runtime = MDClrRuntimeFactory.Instance.BuildClrRuntime(connection);

      var heapObjects = HeapBasedClrObjectEnumerator.Instance;

      var stream =  filteredObjectProvider.ExtractFromRuntime(runtime, heapObjects);

      foreach (var matched in stream)
      {
        yield return factory.BuildOfType<T>(matched);
      }
    }


  }
}

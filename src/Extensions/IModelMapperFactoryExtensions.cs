using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.Facades.ObjectEnumeration;
using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.SourceFactories;

namespace Sitecore.MemoryDiagnostics.Extensions
{
  public static class IModelMapperFactoryExtensions
  {

    public static IEnumerable<IClrObjMappingModel> ExtractFromHeapByType(this IModelMapperFactory factory, Type type, string pathToDumpFile, string pathToMsCord = null)
    {
      return ExtractFromHeapByType(factory, type.FullName, pathToDumpFile, pathToMsCord);
    }

    public static IEnumerable<IClrObjMappingModel> ExtractFromHeapByType(this IModelMapperFactory factory, string typeName, string pathToDumpFile, string pathToMsCord = null)
    {
      var clrObjectFilter = BuildFilterByTypeName(typeName);
     
      return ExtractFromHeap(factory, clrObjectFilter, pathToDumpFile, pathToMsCord);
    }

    public static IEnumerable<TMappingType> ExtractFromHeap<TMappingType>(this IModelMapperFactory factory, IMemoryDumpConnectionPath connectionDetails) where TMappingType : IClrObjMappingModel
    {
      var typeName = ModelMappingAttribute.GetTypeToMapOn(typeof(TMappingType), assert: true);

      return ExtractFromHeapByType(factory, typeName, connectionDetails).OfType<TMappingType>();
    }

    public static IEnumerable<IClrObjMappingModel> ExtractFromHeapByType(this IModelMapperFactory factory, string typeName, IMemoryDumpConnectionPath connectionDetails)
    {
      var clrObjectFilter = BuildFilterByTypeName(typeName);

      return ExtractFromHeap(factory, clrObjectFilter, connectionDetails);
    }

    public static IEnumerable<IClrObjMappingModel> ExtractFromHeapByTypeName(this IModelMapperFactory factory, string typeName, ClrRuntime runtime)
    {
      return ExtractFromHeap(factory, new HeapBasedFacadeObjectEnumerator(typeName), runtime);
    }

    public static IEnumerable<IClrObjMappingModel> ExtractFromHeap(this IModelMapperFactory factory, IFilteredObjectsProvider clrObjectFilter, string pathToDumpFile, string pathToMsCord=null)
    {
      if (string.IsNullOrEmpty(pathToMsCord))
      {
        MDClrRuntimeFactory.TryGetMscordacPath(pathToDumpFile, out pathToMsCord);
      }
      var connectionDetails = new MDFileConnection(pathToDumpFile, pathToMsCord);

      return ExtractFromHeap(factory, clrObjectFilter, connectionDetails);
    }

    public static IEnumerable<IClrObjMappingModel> ExtractFromHeap(this IModelMapperFactory factory, IFilteredObjectsProvider clrObjectFilter, IMemoryDumpConnectionPath connection)
    {
      var runtime = MDClrRuntimeFactory.Instance.BuildClrRuntime(connection);

      return ExtractFromHeap(factory, clrObjectFilter, runtime);
    }


    public static IEnumerable<IClrObjMappingModel> ExtractFromHeap(this IModelMapperFactory factory, IFilteredObjectsProvider clrObjectFilter, ClrRuntime runtime)
    {
      return ExtractFromHeap(factory, new HeapBasedFacadeObjectEnumerator(clrObjectFilter), runtime);
    }

    public static IEnumerable<IClrObjMappingModel> ExtractFromHeap(this IModelMapperFactory factory, IObjectEnumerationFacade objectEnumeration, ClrRuntime runtime)
    {      
      foreach (var clrObject in objectEnumeration.ExtractFromRuntime(runtime))
      {
        yield return factory.BuildModel(clrObject);
      }
    }

    private static IFilteredObjectsProvider BuildFilterByTypeName(string typeName)
    {
      return new FilteredObjectProviderByTypeName(typeName);
    }
  }
}

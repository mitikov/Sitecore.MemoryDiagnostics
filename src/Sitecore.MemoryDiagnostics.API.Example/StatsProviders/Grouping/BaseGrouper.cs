using System.Collections.Generic;
using Sitecore.MemoryDiagnostics.Extensions;
using Sitecore.MemoryDiagnostics.ModelFactory;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.API.Example
{
  public abstract class BaseGrouper
  {
    protected virtual ModelMapperFactory CreateObjectMappingFactory() => ModelMapperManager.NewMapperFactory;

    public virtual void DoGrouping(string pathToMemoryDump, string pathToMscorDac, string typeToExtract)
    {
      var modelMapperFactory = CreateObjectMappingFactory();

      var clrObjectMappingModelStream = modelMapperFactory.
      ExtractFromHeapByType(
      typeName: typeToExtract,
      pathToDumpFile: pathToMemoryDump,
      pathToMsCord: pathToMscorDac);

      DoProcessing(clrObjectMappingModelStream);
    }

    protected abstract void DoProcessing(IEnumerable<IClrObjMappingModel> clrObjectMappingModelStream);
  }
}

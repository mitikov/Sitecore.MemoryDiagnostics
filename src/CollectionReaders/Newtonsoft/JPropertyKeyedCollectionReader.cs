using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.ModelFactory;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  public class JPropertyKeyedCollectionReader : CollectionReaderBase
  {
    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      var keys = obj.GetRefFld("items");

      if (keys.IsNullObj)
      {
        return new ArrayMappingModel() { Obj = obj };
      }
      return GenericListReader.EnumerateList(keys, factory);
    }

    public override bool SupportTransformation([ClrObjAndTypeNotEmpty] ClrObject obj)
    {
      return base.SupportTransformation(obj) && obj.Type.Name == "Newtonsoft.Json.Linq.JPropertyKeyedCollection";
    }
  }
}

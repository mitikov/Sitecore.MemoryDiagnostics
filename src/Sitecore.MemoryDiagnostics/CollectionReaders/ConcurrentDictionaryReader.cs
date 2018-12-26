namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class ConcurrentDictionaryReader : CollectionReaderBase
  {
    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      ClrObject tablesFld = obj.GetRefFld("m_tables");

      ClrObject bucketsFld = tablesFld.GetRefFld("m_buckets");
      ClrType type = bucketsFld.Type;
      ClrHeap heap = type.Heap;

      IEnumerable<ClrObject> values = ClrCollectionHelper.EnumerateArrayOfRefTypes(bucketsFld).Where(t => !t.IsNullObj);


      ClrInstanceField valueField = type.ComponentType.GetFieldByName("m_value");

      ClrInstanceField keyField = type.ComponentType.GetFieldByName("m_key");
      var hashtableModel = new HashtableMappingModel
      {
        Obj = obj
      };

      foreach (ClrObject val in values)
      {
        ClrObject keyObj = val.GetRefFld("m_key");
        if (keyObj.IsNullObj)
          continue;

        ClrObject valObj = val.GetRefFld("m_value");

        hashtableModel.Elements.Add(
         key: factory.BuildModel(keyObj),
         value: factory.BuildModel(valObj));
      }

      return hashtableModel;
    }

    public override bool SupportTransformation([ClrObjAndTypeNotEmpty] ClrObject obj)
    {
      return obj.Type.Name.StartsWith("System.Collections.Concurrent.ConcurrentDictionary<");
    }
  }
}
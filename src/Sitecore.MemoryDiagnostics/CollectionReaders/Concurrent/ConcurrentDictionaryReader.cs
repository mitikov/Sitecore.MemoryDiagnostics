using System.Collections.Generic;
using System.Linq;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.ModelFactory;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.CollectionReaders.Concurrent
{
  public class ConcurrentDictionaryReader : CollectionReaderBase
  {
    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      var rawElelments = ProcessReferenceConcurrentDictionary(obj);

      var model = new HashtableMappingModel(rawElelments.Count, obj);
      foreach (var pair in rawElelments)
      {
        model.Elements.Add(factory.BuildModel(pair.Key), factory.BuildModel(pair.Value));
      }

      return model;
    }

    public override bool SupportTransformation([ClrObjAndTypeNotEmpty] ClrObject dictionary)
    {
      if (!base.SupportTransformation(dictionary) || !dictionary.Type.Name.Contains("System.Collections.Concurrent.ConcurrentDictionary"))
      {
        return false;
      }

      var m_bucketsArray = dictionary.GetRefFldChained(new[] { "m_tables", "m_buckets" });

      if (!m_bucketsArray.HasValue)
      {
        return false;
      }

      var mBucketType = m_bucketsArray.Value.Type;
      
      if (mBucketType.IsArray)
      {
        var keyType = mBucketType.Fields.Where(f=> f.Name == "m_key").FirstOrDefault();
        var valueType = mBucketType.Fields.Where(f => f.Name == "m_value").FirstOrDefault();

        return (keyType?.IsObjectReference == true && valueType?.IsObjectReference == true);
      }

      return false;
    }

    public static IDictionary<ClrObject, ClrObject> ProcessReferenceConcurrentDictionary(ClrObject dictionary)
    {
      var heap = dictionary.Type.Heap;
      var m_bucketsArray = dictionary.GetRefFldChained(new[] { "m_tables", "m_buckets" }).Value;

      var totalElements = m_bucketsArray.Type.GetArrayLength(m_bucketsArray.Address);

      var result = new Dictionary<ClrObject, ClrObject>();
      for (int i = 0; i < totalElements; i++)
      {
        var nodeAddress = (ulong)m_bucketsArray.Type.GetArrayElementValue(m_bucketsArray.Address, i);

        var type = heap.GetObjectType(nodeAddress);

        if (type == null)
          continue;
        var node = new ClrObject(nodeAddress, type);

        var key = node.GetRefFld("m_key");
        
        if (key.IsNullObj)
        {
          continue;
        }
        var value = node.GetRefFld("m_value");

        result.Add(key, value);
      }

      return result;
    }
  }
}

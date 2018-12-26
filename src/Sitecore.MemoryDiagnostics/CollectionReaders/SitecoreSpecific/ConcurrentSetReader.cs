using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.ModelFactory;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.CollectionReaders.Concurrent
{
  public class ConcurrentSetReader : CollectionReaderBase
  {
    public override IClrObjMappingModel ReadEntity(ClrObject set, ModelMapperFactory factory)
    {

      var heap = set.Type.Heap;
      ClrObject container = set.GetRefFld("container");

      var m_bucketsArray = container.GetRefFld("m_tables").GetRefFld("m_buckets");

      var totalElements = m_bucketsArray.Type.GetArrayLength(m_bucketsArray.Address);

      var result = new ArrayMappingModel
      {
        Obj = set
      };

      for (int i = 0; i < totalElements; i++)
      {
        var nodeAddress = (ulong)m_bucketsArray.Type.GetArrayElementValue(m_bucketsArray.Address, i);

        if (nodeAddress == 0)
          continue;

        var type = heap.GetObjectType(nodeAddress);

        if (type == null)
          continue;
        var node = new ClrObject(nodeAddress, type);

        var key = node.GetRefFld("m_key");
        if (!key.IsNullObj)
        {
          result.AddElement(factory.BuildModel(key));
        }
      }

      return result;
    }

    public override bool SupportTransformation([ClrObjAndTypeNotEmpty] ClrObject set)
    {
      if (!base.SupportTransformation(set) || !set.Type.Name.Contains("Sitecore.Collections.ConcurrentSet"))
      {
        return false;
      }

      var dictionary = set.GetRefFld("container");
      var m_bucketsArray = dictionary.GetRefFld("m_tables").GetRefFld("m_buckets");

      if (m_bucketsArray.Type == null)
      {
        return false;
      }

      var mBucketType = m_bucketsArray.Type;

      if (mBucketType.IsArray)
      {
        var keyType = mBucketType.ComponentType.Fields.Where(f => f.Name == "m_key").FirstOrDefault();

        return (keyType?.IsObjectReference == true);
      }

      return false;
    }
  }
}

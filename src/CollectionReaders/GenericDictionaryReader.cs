namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System;
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class GenericDictionaryReader : CollectionReaderBase
  {
    private const string entriesFldName = @"entries";

    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      var hashtableModel = new HashtableMappingModel
      {
        Obj = obj
      };

      ClrObject entriesField = obj.GetRefFld(entriesFldName);

      if (entriesField==null || entriesField.Type == null)
      {
        // Object contains this field, but value is null. 
        // Most likely dictionary is either in the middle of construction, or destruction.
        // Anyway we can assume it is empty.
        return hashtableModel;
      }

      ClrType type = entriesField.Type;

      ClrHeap heap = type.Heap;

      var componentType = type.ComponentType;
      if (componentType == null)
      {
        // TODO :Try reload type.
        return null;
      }

      ClrInstanceField valueField = componentType.GetFieldByName("value");

      ClrInstanceField keyField = componentType.GetFieldByName("key");

      int len = type.GetArrayLength(entriesField.Address);

      ulong entriesFieldAddress = entriesField.Address;

      for (int i = 0; i < len; i++)
      {
        ulong addr = type.GetArrayElementAddress(entriesFieldAddress, i);

        if (addr == 0)
          continue;
        try
        {
          var key = keyField.GetValue(addr, true);
          if (!(key is ulong))
            continue;
          var keyPointer = (ulong)key;
          if (keyPointer == 0)
            continue;

          var val = valueField.GetValue(addr, true);

          ulong valuePointer;
          if (val is ulong)
            valuePointer = (ulong)val;
          else
          {
            valuePointer = 0;
          }

          var keyObj = new ClrObject(keyPointer, heap);
          var valObj = new ClrObject(valuePointer, heap);

          hashtableModel.Elements.Add(factory.BuildModel(keyObj),
            factory.BuildModel(valObj));
        }
        catch (Exception)
        {
          Trace.TraceError("Count not read {0} object", obj.HexAddress);

          // Do nothing for now
        }
      }

      return hashtableModel;
    }

    public override bool SupportTransformation([ClrObjAndTypeNotEmpty] ClrObject obj)
    {
      if (!obj.Type.Name.StartsWith("System.Collections.Generic.Dictionary<"))
        return false;


      ClrInstanceField fld = obj.Type.GetFieldByName(entriesFldName);

#if TRACE
      if (fld == null)
        Trace.TraceInformation("{0} object {1} type does not have {2}", obj.Address.ToString("x8"), obj.Type.Name, entriesFldName);
#endif

      return fld != null;
      /*
       * if (fld == null)
        return false;

      var fldVal = obj.GetRefFld(entriesFldName);
      return !fldVal.IsNullObj;               
      */
    }
  }
}
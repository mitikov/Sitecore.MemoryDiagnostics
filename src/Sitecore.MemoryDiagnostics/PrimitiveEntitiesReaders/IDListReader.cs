namespace Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders
{
  using System;
  using System.Collections.Generic;
  using Collections;
  using Microsoft.Diagnostics.Runtime;
  using ModelFactory;
  using Models.BaseMappingModel;
  using Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class IDListReader : IPrimitiveEntityReader
  {
    public Type SupportedType => typeof(IDList);

    public static IDList Read(ClrObject obj, string fieldName)
    {
      if (obj.IsNullObj)
      {
        return null;
      }

      if (obj.Type == null)
      {
        return null;
      }

      ClrObject listPointer = obj.GetRefFld(fieldName);

      return Read(listPointer);
    }

    public static IDList Read(ClrObject obj)
    {
      if (obj.IsNullObj)
      {
        return null;
      }

      ClrObject idsRef = obj.GetRefFld("m_ids");
      if (idsRef.IsNullObj)
      {
        return null;
      }

      List<ClrObject> ids = idsRef.Type?.Name?.Contains("ArrayList") == true ? ClrCollectionHelper.EnumerateArrayList(idsRef) :
      ClrCollectionHelper.EnumerateListOfRefTypes(idsRef);
      var res = new IDList();
      foreach (ClrObject id in ids)
      {
        res.Add(IDReader.Read(id));
      }

      return res;
    }

    public IDListMappingModel GetModel(ClrObject obj)
    {
      return new IDListMappingModel
      {
        Obj = obj,
        Collection = Read(obj)
      };
    }

    public virtual IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      var res = new IDListMappingModel
      {
        Obj = obj
      };
      IDList list = Read(obj);
      res.Collection = list;
      return res;
    }

    public virtual bool SupportTransformation(ClrObject obj)
    {
      return obj.Type.Name.Equals(typeof(IDList).FullName, StringComparison.Ordinal);
    }

    object IPrimitiveEntityReader.Read(ClrObject obj, string fldName)
    {
      return Read(obj, fldName);
    }
  }
}
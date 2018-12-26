namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System;
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class SafeDictionaryReader : CollectionReaderBase
  {
    public static string SafeDictionaryClassName = @"Sitecore.Collections.SafeDictionary";
    private readonly CollectionReaderBase _hashtableReader = new HashtableReader();

    public static Dictionary<ClrObject, ClrObject> Read(ClrObject obj)
    {
      if (obj.IsNullObj)
      {
        return null;
      }

      obj.ReEvaluateType();
      if (obj.Type == null)
      {
        return null;
      }

      ClrObject hashtable = obj.GetRefFld("_hashtable");
      return HashtableReader.ReadClrObjects(hashtable);
    }

    public static Dictionary<ClrObject, ClrObject> Read(ClrObject obj, string fldName)
    {
      if (obj.IsNullObj)
      {
        return null;
      }

      obj.ReEvaluateType();
      if (obj.Type == null)
      {
        return null;
      }

      ClrObject fld = obj.GetRefFld(fldName);
      return Read(fld);
    }

    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      ClrObject hashtable = obj.GetRefFld("_hashtable");
      if (hashtable.IsNullObj)
      {
        return null;
      }

      return this._hashtableReader.SupportTransformation(hashtable) ? this._hashtableReader.ReadEntity(hashtable, factory) : null;

      // TODO : change in future.
    }

    public override bool SupportTransformation(ClrObject obj)
    {
      return obj.Type.Name.StartsWith(SafeDictionaryClassName, StringComparison.OrdinalIgnoreCase);
    }
  }
}
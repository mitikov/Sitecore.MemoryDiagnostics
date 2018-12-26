namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class HashtableReader : CollectionReaderBase
  {
    public static Dictionary<ClrObject, ClrObject> ReadClrObjects(ClrObject obj)
    {
      if (obj.Type == null)
      {
        return new Dictionary<ClrObject, ClrObject>();
      }

      ClrObject buckets = obj.GetRefFld("buckets", false);
      if (buckets.Type == null)
      {
        return new Dictionary<ClrObject, ClrObject>();
      }

      List<Tuple<ClrObject, ClrObject>> enumerated = ClrCollectionHelper.EnumerateHashtable(obj);

      var dict = new Dictionary<ClrObject, ClrObject>(enumerated.Count);
      foreach (var tuple in enumerated)
      {
        dict.Add(tuple.Item1, tuple.Item2);
      }

      return dict;
    }


    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      Dictionary<ClrObject, ClrObject> clrDictionary = ReadClrObjects(obj);
      HashtableMappingModel model;

      if (clrDictionary == null)
      {
        return new HashtableMappingModel(obj);
      }

      model = new HashtableMappingModel(clrDictionary.Count, obj);
      foreach (var pair in clrDictionary)
      {
        model.Elements.Add(factory.BuildModel(pair.Key), factory.BuildModel(pair.Value));
      }

      return model;
    }

    public override bool SupportTransformation(ClrObject obj)
    {
      if (obj.Type == null)
        return false;

      return obj.Type.Name.Equals(typeof(Hashtable).FullName, StringComparison.Ordinal);
    }
  }
}
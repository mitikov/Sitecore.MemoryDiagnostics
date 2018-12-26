namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Web;

  using ModelFactory;
  using Models.BaseMappingModel;
  using Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class HttpCookieCollectionReader : CollectionReaderBase
  {
    private const string entriesTableFldName = @"_entriesTable";

    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      ClrObject fld = obj.GetRefFld(entriesTableFldName);
      List<Tuple<ClrObject, ClrObject>> values = ClrCollectionHelper.EnumerateHashtable(fld);

      var hashtable = new Hashtable(values.Count);
      foreach (var keyValuePair in values)
      {
        ClrObject val = keyValuePair.Item2;
        if (val.IsNullObj)
        {
          continue;
        }


        var key = val.GetStringFld("Key");
        ClrObject value = val.GetRefFld("Value");

        if (key == null || value.IsNullObj)
        {
          continue;
        }
        
        IClrObjMappingModel valModel = factory.BuildModel(value);
        hashtable.Add(key, valModel);
      }

      var hashTableMapping = new HashtableMappingModel(hashtable, obj);

      return hashTableMapping;
    }

    public override bool SupportTransformation(ClrObject obj)
    {
      return obj.Type.Name.Equals(typeof(HttpCookieCollection).FullName) &&
             (obj.Type.GetFieldByName(entriesTableFldName) != null);
    }
  }
}
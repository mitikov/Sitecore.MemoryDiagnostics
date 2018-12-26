namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  [Obsolete]
  public class DictionaryEntryReader : CollectionReaderBase
  {
    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      if (obj.IsNullObj)
        return null;

      ClrObject keyClrObj = obj.GetRefFld("key");
      ClrObject valClrObj = obj.GetRefFld("val");
      if (keyClrObj.IsNullObj)
        return null;

      IClrObjMappingModel keyObj = factory.BuildModel(keyClrObj);
      if (keyObj is EmptyClrObjectModel)
        return null;

      IClrObjMappingModel valObj = factory.BuildModel(valClrObj);

      return new DictionaryEntryModel(keyObj, valObj);
    }

    public override bool SupportTransformation(ClrObject obj)
    {
      return obj.Type.Name.StartsWith("System.Collections.Generic.Dictionary+Entry<") && !obj.Type.IsArray;
    }
  }
}
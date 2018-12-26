namespace Sitecore.MemoryDiagnostics.CollectionReaders.SitecoreSpecific
{
  using System;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class ScSetReader : CollectionReaderBase
  {
    // protected CollectionReaderBase SDictionaryReader = new SafeDictionaryReader();
    public static string ScSetReaderName = @"Sitecore.Collections.Set";

    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      var field = obj.GetRefFld("_data");
      IClrObjMappingModel safeDictionary;
      if (!factory.CollectionEnumerator.TryProcessSpecialCollection(field, out safeDictionary))
      {
        return null;
      }

      var casted = safeDictionary as HashtableMappingModel;
      if (casted == null)
      {
        return null;
      }

      var model = new ArrayMappingModel();
      foreach (IClrObjMappingModel key in casted.Elements.Keys)
      {
        model.AddElement(key);
      }

      return model;
    }

    public override bool SupportTransformation(ClrObject obj)
    {
      return base.SupportTransformation(obj) && (obj.Type.Name.IndexOf(ScSetReaderName, StringComparison.OrdinalIgnoreCase) == 0);
    }
  }
}
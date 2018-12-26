namespace Sitecore.MemoryDiagnostics.CollectionReaders.SitecoreSpecific
{
  using System;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.MVC;
  using Sitecore.Data;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class MVCRenderingPropertiesReader : CollectionReaderBase
  {
    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      // TODO: remove the hack
      var fld = obj.GetRefFld("<Values>k__BackingField");
      if (fld.IsNullObj)
      {
        return null;
      }
      var values = factory.BuildModel(fld) as HashtableMappingModel;
      if (values == null)
      {
        return null;
      }

      var rs = new SitecoreMVCRenderingPropertiesModel
      {
        Obj = obj,
        Cacheable = string.Equals(values[@"Cacheable"] as string, "1", StringComparison.OrdinalIgnoreCase),
        Cache_VaryByData = string.Equals(values[@"Cache_VaryByData"] as string, "1", StringComparison.OrdinalIgnoreCase),
        CacheKey = values[@"CacheKey"] as string ?? "[NoCacheKey]"
      };

      ID.TryParse(values[@"RenderingItemPath"], out rs.RenderingItemPath);
      return rs;
    }

    public override bool SupportTransformation(ClrObject obj)
    {
      return base.SupportTransformation(obj) && obj.Type.Name.Equals("Sitecore.Mvc.Presentation.RenderingProperties", StringComparison.Ordinal);
    }
  }
}
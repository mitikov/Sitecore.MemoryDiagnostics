namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class GenericListReader : CollectionReaderBase
  {
    public static IClrObjMappingModel EnumerateList(ClrObject clrObject, ModelMapperFactory factory)
    {
      var result = new ArrayMappingModel
      {
        Obj = clrObject
      };
      ClrObject fld = clrObject.GetRefFld("_items");
      if (fld.Type == null)
        return result;
      ClrType tp = fld.Type.ComponentType;
      if ((!tp.IsValueClass) && (!tp.IsString))
      {
        // TODO: add support of reading strings.
        List<ClrObject> enumeration = ClrCollectionHelper.EnumerateListOfRefTypes(clrObject);
        foreach (ClrObject o in enumeration)
        {
          IClrObjMappingModel model = factory.BuildModel(o);
          result.Elements.Add(model);
        }
      }

      return result;
    }

    public static bool IsGenericList(ClrObject clrObject)
    {
      if ((clrObject.Type == null) || (!clrObject.Type.Name.Contains("System.Collections.Generic.List")))
        return false;

      ClrObject fld = clrObject.GetRefFld("_items");
      return (fld.Type != null) && (fld.Type.IsArray);
    }

    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      return EnumerateList(obj, factory);
    }

    public override bool SupportTransformation(ClrObject obj)
    {
      return obj.Type.Name.StartsWith("System.Collections.Generic.List") && (!obj.Type.IsValueClass);
    }
  }
}
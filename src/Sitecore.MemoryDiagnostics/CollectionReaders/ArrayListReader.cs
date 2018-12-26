namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System.Collections;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class ArrayListReader : ArrayReader
  {
    public override IClrObjMappingModel ReadEntity(ClrObject clrObject, ModelMapperFactory factory)
    {
      var _itemsRef = clrObject.GetRefFld("_items");
      if (_itemsRef.Type == null)
        return new ArrayMappingModel
        {
          Obj = clrObject
        };
      return base.ReadEntity(_itemsRef, factory);
    }

    public override bool SupportTransformation([ClrObjAndTypeNotEmpty] ClrObject obj)
    {
      return obj.Type?.Name.Equals(typeof(ArrayList).FullName) == true;
    }
  }
}
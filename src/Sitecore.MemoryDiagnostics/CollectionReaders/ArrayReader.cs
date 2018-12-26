namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System;
  using System.Linq;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Helpers;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class ArrayReader : CollectionReaderBase
  {

    protected EnumerationHelper EnumerationHelper = new StringArrayOptimizationsAwareEnumerator();
    /// <summary>
    /// Reads the entity.
    /// </summary>
    /// <param name="clrObject">ClrObject points on <see cref="Array"/>.</param>
    /// <param name="factory">The factory.</param>
    /// <returns></returns>
    public override IClrObjMappingModel ReadEntity([ClrObjAndTypeNotEmpty] ClrObject clrObject,
      [NotNull] ModelMapperFactory factory)
    {
      return this.ReadArray(clrObject, factory);
    }

    public override bool SupportTransformation([ClrObjAndTypeNotEmpty] ClrObject obj)
    {
      return obj.Type?.IsArray == true;
    }

    protected ArrayMappingModel ReadArray([ClrObjAndTypeNotEmpty] ClrObject clrObject,
      [NotNull] ModelMapperFactory factory)
    {
      var result = new ArrayMappingModel
      {
        Obj = clrObject
      };

      ClrObject[] arrayContent = this.EnumerationHelper.DoEnumerateArrayType(clrObject);
      if (arrayContent != null)
        foreach (ClrObject o in arrayContent)
        {
          o.ReEvaluateType();
          IClrObjMappingModel nested = factory.BuildModel(o);
          result.Elements.Add(nested);
        }

      return result;
    }


    protected virtual ArrayMappingModel ReadTheOnlyArrayInsideType(ClrObject obj, ModelMapperFactory factory)
    {
      ClrAssert.ClrObjectNotNull(obj);
      var type = obj.Type;
      Assert.Required(type, "NoType");

      var collectionFld = (from fld in type.Fields
        let fldType = fld.Type
        where fldType != null
        where fldType.IsArray
        select fld).FirstOrDefault();

      Assert.Required(collectionFld, "no array type found");

      var address = (ulong)collectionFld.GetValue(obj.Address);

      var arrayObj = new ClrObject(address, type.Heap);
      if ((arrayObj.Address == default(ulong)) || (arrayObj.Type == null))
        return null;
      return ReadArray(arrayObj, factory);
    }
  }
}
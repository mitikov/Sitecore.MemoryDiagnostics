namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class HashListReader : CollectionReaderBase
  {
    private readonly HashtableReader _reader = new HashtableReader();

    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      ClrObject _items = obj.GetRefFld("_items");
      return _reader.SupportTransformation(_items) ? _reader.ReadEntity(_items, factory) : null;
    }

    public override bool SupportTransformation(ClrObject obj)
    {
      return obj.Type.Name.StartsWith(@"Sitecore.Collections.HashList") && (obj.Type.GetFieldByName("_items") != null);
    }
  }
}
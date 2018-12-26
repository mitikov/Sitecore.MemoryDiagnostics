namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public abstract class CollectionReaderBase
  {
    // TODO: should this method output be marked as [NotNull]/[CanBeNull] ?
    public abstract IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory);


    public virtual bool SupportTransformation([ClrObjAndTypeNotEmpty] ClrObject obj)
    {
      bool violation = obj.IsNullObj || (obj.Type == null);
      return !violation;
    }

    // TODO: Add reading of array via base.
  }
}
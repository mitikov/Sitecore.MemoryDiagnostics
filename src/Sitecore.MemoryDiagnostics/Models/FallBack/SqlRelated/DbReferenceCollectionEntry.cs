namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping("System.Data.ProviderBase.DbReferenceCollection+CollectionEntry")]
  public class DbReferenceCollectionEntry : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public int _tag;

    [InjectFieldValue]
    public IClrObjMappingModel _weak;
  }
}
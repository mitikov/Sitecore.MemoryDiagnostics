namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Attributes;
  using BaseMappingModel;

  [ModelMapping("System.Data.SqlClient.SqlReferenceCollection")]
  public class SqlReferenceCollectionMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public IClrObjMappingModel _items;
  }
}
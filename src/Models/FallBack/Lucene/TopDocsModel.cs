namespace Sitecore.MemoryDiagnostics.Models.FallBack.Lucene
{
  using Attributes;
  using BaseMappingModel;

  [ModelMapping("Lucene.Net.Search.TopDocs")]
  public class TopDocsModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public int _totalHits;
  }
}
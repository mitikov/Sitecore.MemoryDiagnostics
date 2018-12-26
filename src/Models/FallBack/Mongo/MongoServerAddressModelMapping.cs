namespace Sitecore.MemoryDiagnostics.Models.FallBack.MongoRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(@"MongoDB.Driver.MongoServerAddress")]
  public class MongoServerAddressModelMapping : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string _host;

    [InjectFieldValue]
    public int _port;

    public override string Caption
    {
      get
      {
        return base.Caption + _host.FallbackTo("No Host");
      }
    }
  }
}
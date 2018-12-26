namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(@"System.Web.DirectoryMonitor")]
  public class DirectoryMonitorMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string Directory;

    public override string Caption
    {
      get
      {
        return base.Caption + Directory + " watched.";
      }
    }
  }
}
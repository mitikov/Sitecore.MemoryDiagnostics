namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.DataEngine
{
  using Sitecore.Data.Engines.DataCommands;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;


  [ModelMapping(typeof(ResolvePathCommand))]
  public class ResolvePathCommandModel : ClrObjectMappingModel
  {
    public string m_itemPath;
  }
}

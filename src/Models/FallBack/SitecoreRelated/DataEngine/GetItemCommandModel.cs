namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.DataEngine
{
  using Sitecore.Data;
  using Sitecore.Data.Engines.DataCommands;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(typeof(GetItemCommand))]
  public class GetItemCommandModel : ClrObjectMappingModel
  {
    public ID _itemId;
  }
}

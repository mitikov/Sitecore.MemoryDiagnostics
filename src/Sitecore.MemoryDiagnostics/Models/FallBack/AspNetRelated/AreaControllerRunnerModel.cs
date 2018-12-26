using Environment = System.Environment;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(@"MvcAreas.AreaControllerRunner")]
  public class AreaControllerRunnerModel : ClrObjectMappingModel
  {
    public string ActionName;

    [InjectFieldValue]
    public string actualControllerName;

    public string Area;

    public string ControllerName;

    public override string Caption
    {
      get
      {
        return base.Caption + Area + "->" + ControllerName + "->" + ActionName;
      }
    }

    public override void Compute()
    {
      ActionName = Obj.GetStringSafe(@"<ActionName>k__BackingField") ?? "[NoActionName]";
      ControllerName = Obj.GetStringSafe(@"<ControllerName>k__BackingField") ?? "[NoControllerName]";
      Area = Obj.GetStringSafe(@"<Area>k__BackingField") ?? "[NoArea]";
    }

    public override string ToString()
    {
      return string.Concat("Area: ", Area,
        Environment.NewLine,
        "Controller: ", ControllerName,
        Environment.NewLine,
        "Action: ", ActionName,
        Environment.NewLine
      );
    }
  }
}
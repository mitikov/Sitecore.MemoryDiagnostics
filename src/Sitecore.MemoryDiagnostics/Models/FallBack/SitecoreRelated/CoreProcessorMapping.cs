namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Pipelines;

  [ModelMapping(typeof(CoreProcessor))]
  public class CoreProcessorMapping : ClrObjectMappingModel
  {
    public string _methodName;
    public string _name;

    public override string Caption
    {
      get
      {
        return base.Caption + _name;
      }
    }

    public override void Compute()
    {
      var type = Obj.Type;
      if (type == null)
      {
        _name = _methodName = "[NoType]";
        return;
      }

      _name = Obj.GetStringSafe("_name") ?? "[NoName]";
      _methodName = Obj.GetStringSafe("_methodName") ?? "[NoMethodName]";
    }

    public override string ToString()
    {
      return string.Format("{0}, method= \"{1}\"", _name, _methodName);
    }
  }
}
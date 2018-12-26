namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore;

  [ModelMapping(typeof(Sitecore.Security.AccessControl.AccessRight))]
  public class AccessRightModel : ClrObjectMappingModel
  {
    public static readonly string NotSetAccessRight = "[NotSetAccessRight]";

    [InjectFieldValue]
    public string _comment;

    [InjectFieldValue]
    public string _name;

    [InjectFieldValue]
    public string _title;

    [NotNull]
    public override string Caption
    {
      get
      {
        return _name ?? NotSetAccessRight;
      }
    }

    public override string ToString()
    {
      return _comment ?? Caption;
    }
  }
}
namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.Users
{
  using System.Text;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.Security;

  [ModelMapping(typeof(Sitecore.SecurityModel.UserRuntimeSettings))]
  public class UserRuntimeSettingsModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public RolesCollectionModel addedRoles;

    [InjectFieldValue]
    public bool isAdministrator;

    [InjectFieldValue]
    public bool isVirtual;

    [InjectFieldValue]
    public RolesCollectionModel removedRoles;

    [InjectFieldValue]
    public string UserName;


    public override string Caption
    {
      get
      {
        return base.Caption + string.Format("{0} isVirtual {1}, admin {2}", UserName ?? "[NoName]", isVirtual, isAdministrator);
      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder(base.ToString());
      sb.AppendLine("Added roles");
      sb.AppendLine(this.addedRoles?.ToString() ?? "No Added roles");
      sb.AppendLine("Removed roles");
      sb.AppendLine(this.removedRoles?.ToString() ?? "No removed roles");

      return sb.ToString();
    }
  }
}
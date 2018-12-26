namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Attributes;
  using BaseMappingModel;
  using ModelMetadataInterfaces;

  [ModelMapping(typeof(Sitecore.Security.Accounts.User))]
  public class UserMappingModel : ClrObjectMappingModel, ICaptionHolder
  {
    [InjectFieldValue]
    public string _name;

    public string Caption => $"[{this.GetType().Name}] {this.UserName}";

    public string UserName => string.IsNullOrEmpty(this._name) ? "[NoUserName]" : this._name;
  }
}
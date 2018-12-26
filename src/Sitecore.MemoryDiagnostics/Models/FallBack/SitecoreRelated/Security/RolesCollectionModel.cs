namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.Security
{
  using System.Text;

  using Attributes;
  using BaseMappingModel;
  using InternalProcessing;
  using SecurityModel;
  using StringExtensions;

  [ModelMapping(typeof(RolesCollection))]
  public class RolesCollectionModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public HashtableMappingModel _data;

    [InjectFieldValue]
    public string UserName;

    public override string Caption => base.Caption + this.UserName;

    public string[] Roles
    {
      get
      {
        var res = new string[this._data.Elements.Count];
        int i = 0;
        foreach (var t in this._data.Elements.Keys)
        {
          // string model
          var casted = t as string;

          res[i] = casted ?? t.ToString();
          i++;
        }

        return res;
      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine("User {0} Roles:".FormatWith(this.UserName));
      foreach (string role in this.Roles)
      {
        sb.AppendLine(role);
      }

      return sb.ToString();
    }
  }
}
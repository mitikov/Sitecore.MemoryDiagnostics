using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.Users
{
  [ModelMapping(@"Sitecore.Security.Principal.SitecoreIdentity")]
  public class SitecoreIdentityModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool isAuthenticated;

    [InjectFieldValue]
    public string name;
  }
}

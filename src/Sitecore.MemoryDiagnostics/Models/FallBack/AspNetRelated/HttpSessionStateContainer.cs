using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  [ModelMapping(typeof(System.Web.SessionState.HttpSessionStateContainer))]
  public class HttpSessionStateContainer : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string _id;

    [InjectFieldValue]
    public int _timeout;

    [InjectFieldValue]
    public bool _isReadonly;

    [InjectFieldValue]
    public bool _newSession;

    [InjectFieldValue]
    public System.Web.SessionState.SessionStateMode _mode;

    public override string Caption => $"{base.Caption} {_id} {_mode.ToString()}";    
  }
}

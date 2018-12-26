using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.Newtonsoft
{
  [ModelMapping(@"Newtonsoft.Json.Linq.JProperty+JPropertyList")]
  public class JPropertyListMappingModel: ClrObjectMappingModel
  {
    [InjectFieldValue]
    IClrObjMappingModel _token;

    public JValueMappingModel Token => _token as JValueMappingModel;

    public bool IsEmpty => Token == null;

    public override string ToString() 
      => $"{HexAddress}: {_token?.ToString() ?? "[No token met]"}";
  }
}

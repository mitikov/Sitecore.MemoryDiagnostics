using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.Reflection
{
  [ModelMapping(@"System.Reflection.RuntimeMethodInfo")]
  public class RuntimeParameterInfoModel: ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string NameImpl;

  }
}

using System.Diagnostics;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.Reflection
{
  /// <summary>
  /// Defines a how many arguments will be in the method signature generated on fly.
  /// </summary>
  [DebuggerDisplay("Enforces {m_argCount} arguments; baked: {m_sigDone}")]
  [ModelMapping("System.Reflection.Emit.SignatureHelper")]
  public class SignatureHelperModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public int m_argCount;

    [InjectFieldValue]
    public bool m_sigDone;
  }
}

using System.Diagnostics;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.JScripts
{
  /// <summary>
  /// Represents a JScript document context - simply a name file that is being processed.
  /// </summary>
  [DebuggerDisplay("{documentName}")]
  [ModelMapping("Microsoft.JScript.DocumentContext")]
  public class DocumentContextModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string documentName;

    [InjectFieldValue]
    public bool debugOn;

    public override string Caption => base.Caption + documentName + (debugOn ? "Debug on" : string.Empty);
  }
}

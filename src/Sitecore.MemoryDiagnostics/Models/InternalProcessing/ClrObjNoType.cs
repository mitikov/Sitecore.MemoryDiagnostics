namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  /// <summary>
  ///   Represents an object without type.
  /// </summary>
  public sealed class ClrObjNoType : ClrObjectMappingModel
  {
    public ClrObjNoType()
    {
      this.BindingLog.AppendLine("[NoType]");
    }
  }
}
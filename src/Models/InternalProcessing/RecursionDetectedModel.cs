namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  /// <summary>
  /// Shows that an object is already set for processing.
  /// </summary>
  public sealed class RecursionDetectedModel : ClrObjectMappingModel
  {
    public static readonly string RecursionText = "[Recursion]";
  }
}
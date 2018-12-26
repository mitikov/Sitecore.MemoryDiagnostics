namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  /// <summary>
  ///   Object without address model.
  /// </summary>
  public sealed class EmptyClrObjectModel : ClrObjectMappingModel
  {
    public static readonly string EmptyModelText = "[Empty]";

    public static readonly EmptyClrObjectModel Instance = new EmptyClrObjectModel();
  }
}
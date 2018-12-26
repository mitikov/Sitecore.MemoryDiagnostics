namespace Sitecore.MemoryDiagnostics.ModelMetadataInterfaces
{
  using Sitecore;

  /// <summary>
  /// Indicates instance has short representation.
  /// </summary>
  public interface ICaptionHolder
  {
    /// <summary>
    /// Gets a brief instance description.
    /// </summary>
    [NotNull]
    string Caption { get; }
  }
}
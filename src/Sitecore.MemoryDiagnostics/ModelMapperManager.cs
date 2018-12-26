namespace Sitecore.MemoryDiagnostics
{
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;

  /// <summary>
  /// Provides API for easy access to <see cref="ModelMapperFactory"/> instances.
  /// </summary>
  public static class ModelMapperManager
  {
    /// <summary>
    /// Produces a new instance of <see cref="IModelMapperFactory"/>.
    /// </summary>
    public static ModelMapperFactory NewMapperFactory => new LazyLoadModelMapperFactory();
  }
}

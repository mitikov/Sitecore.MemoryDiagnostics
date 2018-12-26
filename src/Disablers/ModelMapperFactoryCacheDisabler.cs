namespace Sitecore.MemoryDiagnostics.Disablers
{
  using Sitecore.MemoryDiagnostics.ModelFactory;

  /// <summary>
  ///   Forbids <see cref="ModelMapperFactory" /> to use caching layer.
  /// </summary>
  public class ModelMapperFactoryCacheDisabler : ThreadStaticDisablerBase<ModelMapperFactoryCacheDisabler>
  {
  }
}
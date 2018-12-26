namespace Sitecore.MemoryDiagnostics.Disablers
{
  using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;

  /// <summary>
  ///   Avoids to skip recursion checks for <see cref="IModelMapperFactory" /> for simple types to improve performance.
  /// </summary>
  public class RecursionCheckerDisabler : ThreadStaticDisablerBase<RecursionCheckerDisabler>
  {
  }
}
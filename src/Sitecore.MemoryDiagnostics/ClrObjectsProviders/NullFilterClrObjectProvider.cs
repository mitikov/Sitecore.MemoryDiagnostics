namespace Sitecore.MemoryDiagnostics.ClrObjectsProviders
{
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// No filtering is applied - simply streams incoming sequence.
  /// </summary>
  public sealed class NullFilterClrObjectProvider : FilteredObjectsProviderBase
  {
    /// <summary>
    /// The instance
    /// </summary>
    public static readonly NullFilterClrObjectProvider Instance = new NullFilterClrObjectProvider();

    /// <summary>
    /// Always matches the extract criteria.
    /// </summary>
    /// <param name="clrObj">The object.</param>
    /// <returns></returns>
    public override bool MatchesExtractCriteria(ClrObject clrObj)
    {
      return true;
    }
  }
}
namespace Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base
{
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Filters strings from runtime.
  /// </summary>
  /// <seealso cref="ClrObjectsProviders.Base.FilteredObjectsProviderBase" />
  public class StringObjectEnumerator : FilteredObjectsProviderBase
  {
    public override bool MatchesExtractCriteria(ClrObject clrObj)
    {
      return (clrObj.Type != null) && clrObj.Type.IsString;
    }
  }
}
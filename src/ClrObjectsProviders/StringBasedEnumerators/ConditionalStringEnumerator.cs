using System;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
using Sitecore.MemoryDiagnostics.Extensions;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.ClrObjectsProviders.StringBasedEnumerators
{
  /// <summary>
  /// Filters <see cref="ClrObject"/>s allowing only <see cref="string"/> that match provided criteria.
  /// </summary>
  public class PredicateBasedStringFilter : FilteredObjectsProviderBase
  {
    #region Fields
    protected readonly StringObjectEnumerator InnerStringEnumerator;

    /// <summary>
    /// User provided function to filter <see cref="string"/>.
    /// </summary>
    protected readonly Func<string, bool> Predicate;
    #endregion

    public PredicateBasedStringFilter(Func<string, bool> predicate) : this(predicate, new StringObjectEnumerator())
    {

    }

    public PredicateBasedStringFilter(Func<string, bool> predicate, [CanBeNull] StringObjectEnumerator inner)
    {
      this.Predicate = predicate;
      this.InnerStringEnumerator = inner;
    }

    public override bool MatchesExtractCriteria([CanBeNullObject] ClrObject clrObj)
    {
      if (InnerStringEnumerator.MatchesExtractCriteria(clrObj))
      {
        var text = this.ExtractString(clrObj);
        return Predicate(text);
      }

      return false;
    }

    public virtual string ExtractString(ClrObject clrObj)
    {
      return clrObj.GetStringSafeFromSelf();
    }
  }
}

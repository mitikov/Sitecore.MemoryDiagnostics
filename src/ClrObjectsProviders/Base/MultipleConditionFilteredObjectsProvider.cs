using System;
using System.Linq;
using Sitecore.MemoryDiagnostics.Attributes;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base
{
  public class MultipleConditionFilteredObjectsProvider : FilteredObjectsProviderBase
  {
    protected Func<ClrObject, bool>[] Filters;
    public MultipleConditionFilteredObjectsProvider(params FilteredObjectsProviderBase[] filters)
    {
      this.Filters = (from filter in filters
                      select new Func<ClrObject, bool>(filter.MatchesExtractCriteria))
                 .ToArray();
    }

    public MultipleConditionFilteredObjectsProvider(params Func<ClrObject, bool>[] filters)
    {
      this.Filters = filters;
    }

    public override bool MatchesExtractCriteria([CanBeNullObject] ClrObject clrObj)
    {
      return Filters.Any(filter => filter(clrObj));
    }
  }
}

using System.Collections.Generic;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.ModelFilters
{
  public class SkipsObjectsOfKnownType : FilteredObjectByTypesProvider
  {
    public SkipsObjectsOfKnownType(params string[] typeNames) : base(typeNames)
    {
    }

    public SkipsObjectsOfKnownType([NotNull] IEnumerable<string> typeNames):base(typeNames)
    {
    }

    public override bool MatchesExtractCriteria(ClrObject clrObj)
    {
      if (clrObj.Type == null)
      {
        return false;
      }

      var isOfTypeToSkip = base.MatchesExtractCriteria(clrObj);

      return !isOfTypeToSkip;
    }
  }
}

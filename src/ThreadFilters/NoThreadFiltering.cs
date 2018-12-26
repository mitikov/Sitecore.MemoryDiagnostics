using Microsoft.Diagnostics.Runtime;
using Sitecore.MemoryDiagnostics.Interfaces;

namespace Sitecore.MemoryDiagnostics.ThreadFilters
{
  public sealed class NoThreadFiltering : IFilter<ClrThread>
  {
    bool IFilter<ClrThread>.Matches(ClrThread tested)
    {
      return true;
    }
  }
}

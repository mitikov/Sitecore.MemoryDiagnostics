namespace Sitecore.MemoryDiagnostics.SitecoreSpecific
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;
  using Sitecore.Data;

  public class PrefetchCacheModel
  {
    public Dictionary<ID, PrefetchDataModel> CachedValues;

    public long CurrentSize;
    public long MaxSize;
    public string Name;

    public PrefetchDataModel this[ID val] => this.CachedValues?[val];
  }
}
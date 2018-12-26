using System;
using System.Collections;
using System.Collections.Generic;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.Caching
{
  [ModelMapping(@"Sitecore.Caching.DefaultCacheManager")]
  public class DefaultCacheManagerMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public HashtableMappingModel cacheReferences;

    public IReadOnlyCollection<ScCache> Caches;

    public override void Compute()
    {
      var caches = new List<ScCache>();
      foreach (DictionaryEntry pair in cacheReferences)
      {
        var cacheName = pair.Key;

        var cachesRegisteredForName = pair.Value as IEnumerable;

        foreach (ScCache cache in cachesRegisteredForName ?? Array.Empty<ScCache>())
        {
          caches.Add(cache);
        }
      }

      Caches = caches;
    }
  }
}

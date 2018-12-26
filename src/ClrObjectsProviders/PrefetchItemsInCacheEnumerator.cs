namespace Sitecore.MemoryDiagnosticsPoweredBase.Example.ItemCacheStats
{
  /*
    /// <summary>
    /// Enumerates content of <see cref="Cache"/> matched by name registration in <see cref="CacheManager"/> collection.
    /// <para>Default checked value is SqlDataProvider - Prefetch data(core)</para>
    /// </summary>
    public class PrefetchDataModelEnumerator : SitecoreCacheEnumerator
    {
      private const string _prefetchCacheName = @"SqlDataProvider - Prefetch data(core)";
  
      protected string SearchedCacheName;
  
      public PrefetchDataModelEnumerator()
        : this(_prefetchCacheName)
      {
      }
  
      public PrefetchDataModelEnumerator([NotNull]string searchedCacheName)
      {
        Assert.ArgumentNotNullOrEmpty(searchedCacheName, "empty cache name is given");
        SearchedCacheName = searchedCacheName;
      }
      public override IEnumerable<ClrObject> EnumerateObjectsFromSource(ClrRuntime ds)
      {
        
        var factory = Context.GetOrDefault<IModelMapperFactory>(new LazyLoadModelMapperFactory());
  
        //Base code would cache objects, thus call to factory would be cheap.
        foreach (var cacheObjReference in base.EnumerateObjectsFromSource(ds))
        {
          var cacheModel = factory.BuildModel(cacheObjReference) as ScCache;
          if (cacheModel == null)
            continue;
          var loopedCacheName = cacheModel.name;
          if (string.IsNullOrEmpty(loopedCacheName) || loopedCacheName.IndexOf(SearchedCacheName, StringComparison.OrdinalIgnoreCase)>0)
            continue;
  
          var data = cacheModel.Data;
          if ((data == null) || (data.Count == 0))
          {
            Context.Message("{0} cache is empty.", _prefetchCacheName);
            continue;
          }
          foreach (DictionaryEntry elem in data.Elements)
          {
            var casted = (elem.Value as CacheEntryModel);
            if (casted == null)
              continue;
            var target = (casted.m_data as PrefetchDataModel);
            if (target == null)
              continue;
            yield return target.Obj;
          }
        }
  
      }
    }*/
}
namespace Sitecore.MemoryDiagnostics.Interfaces
{
  interface IProvideCacheLayer
  {
    /// <summary>
    /// Cleans the cache.
    /// </summary>
    void CleanCache();

    /// <summary>
    /// Gets the number of the cached objects.
    /// </summary>
    /// <returns>number of elements in model cache.</returns>
    int CachedObjectCount { get; }

    bool CacheOn { get; }
  }
}

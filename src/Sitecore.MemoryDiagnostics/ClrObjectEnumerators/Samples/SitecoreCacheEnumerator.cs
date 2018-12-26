namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators.Samples
{
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
  using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
  using Sitecore.Caching;
  using Microsoft.Diagnostics.Runtime;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;
  using System.Collections.Generic;
  using System.Collections;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using System;

  /// <summary>
  /// Enumerates Cache objects that are bind to <see cref="CacheManager"/> ( via private list of weak references).
  /// <para>Use <see cref="IFilteredObjectsProvider"/> to provide further filtering (f.e. by cache name).</para>
  /// </summary>
  public sealed class SitecoreCacheEnumerator : StaticFieldValuesEnumerator
  {
    private readonly IModelMapperFactory modelMapper;


    public SitecoreCacheEnumerator(IModelMapperFactory modelMapper) : base("Sitecore.Caching.CacheManager", "Instance")
    {
      this.modelMapper = modelMapper;
    }

    public override IEnumerable<ClrObject> EnumerateObjectsFromSource([NotNull] ClrRuntime runtime)
    {
      var cacheManagerLazyResetableStream = base.EnumerateObjectsFromSource(runtime);

      foreach (var cacheManagerLazyResetable in cacheManagerLazyResetableStream)
      {
        ClrObject cacheManager = ExtractCacheManager(cacheManagerLazyResetable);
        if (cacheManager.Type?.GetFieldByName("cacheReferences") != null)
        {
          var cacheReferences = cacheManager.GetRefFld("cacheReferences");
          var cachesByNames = (IEnumerable)modelMapper.BuildModel(cacheReferences);

          foreach (DictionaryEntry cacheByNameModel in cachesByNames)
          {
            var cachesRegisteredByName = (IEnumerable)cacheByNameModel.Value;
            foreach (IClrObjMappingModel cache in cachesRegisteredByName)
            {
              yield return cache.Obj;
            }
          }
        }
      }

    }

    private static ClrObject ExtractCacheManager(ClrObject cacheManagerLazyResetable)
    {
      var type = cacheManagerLazyResetable.Type;
      if (type.GetFieldByName("value") != null)
      {
        return cacheManagerLazyResetable.GetRefFld("value");
      }
      else if (type.GetFieldByName("m_boxed") != null)
      {
        var lazy = cacheManagerLazyResetable.GetRefFld("m_boxed");
        return lazy.GetRefFld("m_value");
      }
      throw new InvalidOperationException($"{cacheManagerLazyResetable.HexAddress} of {type.Name} has weird field layout so could not fetch data from it");

    }
  }
}
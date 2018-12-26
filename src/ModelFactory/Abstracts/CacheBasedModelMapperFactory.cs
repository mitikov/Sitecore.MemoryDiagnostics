namespace Sitecore.MemoryDiagnostics.ModelFactory.Abstracts
{
  using System;
  using System.Collections;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Disablers;
  using Sitecore.MemoryDiagnostics.Interfaces;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public abstract class CacheBasedModelMapperFactory : BaseModelMapperFactory, IProvideCacheLayer
  {
    private readonly Hashtable mappingModelsCache = new Hashtable(capacity: 50000);

    /// <summary>
    /// Gets the number of the cached objects.
    /// </summary>
    /// <returns>number of elements in model cache.</returns>
    int IProvideCacheLayer.CachedObjectCount => this.mappingModelsCache.Count;
    bool IProvideCacheLayer.CacheOn => !ModelMapperFactoryCacheDisabler.IsActive;

    private bool CacheOn => ((IProvideCacheLayer)this).CacheOn;

    private bool InEnabledCache(ulong key)
    {
      return this.CacheOn && this.mappingModelsCache.ContainsKey(key);
    }

    /// <summary>
    ///   Parent class ( <see cref="BaseModelMapperFactory.BuildModel" /> API) ensures that method would not go into recursion
    ///   during processing circular references.
    /// </summary>
    /// <param name="obj">The object ensured to have type and not empty pointer.</param>
    /// <returns>
    ///   The <see cref="IClrObjMappingModel" />.
    /// </returns>
    protected override sealed IClrObjMappingModel DoBuildModelFreeOfRecursion([ClrObjAndTypeNotEmpty] ClrObject obj)
    {
      ClrAssert.ObjectNotNullTypeNotEmpty(obj);
      try
      {
        if (this.InEnabledCache(obj.Address))
        {
          return this.mappingModelsCache[obj.Address] as IClrObjMappingModel;
        }

        IClrObjMappingModel model = DoBuildModelWrapped(obj);

        if (!this.mappingModelsCache.ContainsKey(obj.Address)
          && (this.CacheOn) &&
            (!(model is RecursionDetectedModel)))
        {
          this.mappingModelsCache[obj.Address] = model;
        }

        return model;
      }
      catch (Exception ex)
      {
        if (this.InEnabledCache(obj.Address))
        {
          return this.mappingModelsCache[obj.Address] as IClrObjMappingModel;
        }

        // If mapping field is specified as end type ( not IClrObjMappingModel), than set field reflection would fail.
        var res = new ErrorDuringProcessing
        {
          Obj = obj,
          Ex = ex
        };
        if (this.CacheOn)
        {
          this.mappingModelsCache[obj.Address] = res;
        }

        return res;
      }
    }

    /// <summary>
    ///   Executes the build model wrapped into cache layer.
    /// </summary>
    /// <param name="clrObject">ClrObject with type and non-empty pointer</param>
    /// <returns>
    ///   The <see cref="IClrObjMappingModel" />.
    /// </returns>
    protected abstract IClrObjMappingModel DoBuildModelWrapped([ClrObjAndTypeNotEmpty] ClrObject clrObject);

    /// <summary>
    /// Cleans the cache.
    /// </summary>
    void IProvideCacheLayer.CleanCache()
    {
      this.mappingModelsCache.Clear();
    }
  }
}
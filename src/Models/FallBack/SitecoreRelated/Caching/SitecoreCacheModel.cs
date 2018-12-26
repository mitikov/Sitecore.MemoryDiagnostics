namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.SitecoreSpecific;
  using Sitecore.Caching;
  using Sitecore.Data;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using Sitecore.StringExtensions;

  /// <summary>
  /// Sitecore Cache Model Mapping. 
  /// <para>Has current and max size, cache name.</para>
  /// <para>Cache <see cref="data"/> is a <see cref="LazyLoad{T}"/>, thus would be loaded on demand.</para>
  /// </summary>
  [DebuggerDisplay(
     "{name} {FilledText}. Size {currentSize} out of {maxSize}.[Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping(typeof(Cache))]
  public class ScCache : ClrObjectMappingModel
  {
    /// <summary>
    /// The current size of the cache.
    /// </summary>
    public long currentSize;

    /// <summary>
    /// The maximum size of the cache.
    /// </summary>
    [InjectFieldValue]
    public long maxSize;

    /// <summary>
    /// Cache name
    /// </summary>
    [InjectFieldValue]
    public string name;


    [InjectFieldValue]
    public bool enabled;

    public override void Compute()
    {
      var innerBox = Obj.GetRefFld("box");

      if (!innerBox.IsNullObj)
      {
        currentSize = innerBox.GetInt64Fld("currentSize");
      }
    }

    //[InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> data;

    /// <summary>
    /// Gets the caption.
    /// </summary>
    /// <value>
    /// The caption.
    /// </value>
    public override string Caption => $"{this.name} filled {this.FilledText}";

    /// <summary>
    /// Cache content LazyLoad.
    /// </summary>
    /// <value>
    /// The data.
    /// </value>
    public HashtableMappingModel Data => this.data?.Value as HashtableMappingModel;

    public float FilledPercent => this.maxSize == 0 ? 0 : ((float)this.currentSize / this.maxSize);

    public string FilledText => this.FilledPercent == 0 ? "[Empty]" : this.FilledPercent.ToString("P");

    public static explicit operator PrefetchCacheModel([CanBeNullObject] ScCache model)
    {
      if (model == null)
      {
        return null;
      }

      var prefetchCacheModel = new PrefetchCacheModel
      {
        Name = model.name,
        MaxSize = model.maxSize,
        CurrentSize = model.currentSize
      };
      if (model.Data?.Elements == null)
      {
        return prefetchCacheModel;
      }

      Hashtable elem = model.Data.Elements;

      prefetchCacheModel.CachedValues = new Dictionary<ID, PrefetchDataModel>(elem.Count);

      foreach (DictionaryEntry pair in elem)
      {
        if (!(pair.Key is IDModel))
        {
          throw new InvalidCastException("Cannot cast {0} to ID".FormatWith(pair.Key.GetType().Name));
        }

        var key = (ID)(pair.Key as IDModel);

        var tmp = pair.Value as CacheEntryModel;
        if (tmp == null)
        {
          continue;
        }

        var val = tmp.data as PrefetchDataModel;
        if ((val == null) && (pair.Value != null))
        {
          throw new InvalidCastException("Cannot cast {0} to PrefetchDataModel.".FormatWith(pair.Value.GetType()));
        }

        prefetchCacheModel.CachedValues.Add(key, val);
      }

      return prefetchCacheModel;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return this.currentSize == 0 ?
        $"{this.name} is empty. MaxSize: {StringUtil.GetSizeString(this.maxSize)}"
        : $"{this.name} {StringUtil.GetSizeString(this.currentSize)} out of {StringUtil.GetSizeString(this.maxSize)} {this.FilledText}";
    }


  }
}
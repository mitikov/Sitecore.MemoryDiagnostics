

namespace Sitecore.MemoryDiagnostics.Models.CustomModels
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using InternalProcessing;
  using Sitecore.Caching;
  using Sitecore.Data;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;
  using Sitecore.MemoryDiagnostics.SitecoreSpecific;
  using Sitecore.StringExtensions;

  using DictionaryEntry = System.Collections.DictionaryEntry;
  using Hashtable = System.Collections.Hashtable;
  using InvalidCastException = System.InvalidCastException;

  [DebuggerDisplay(
     "{name} {FilledText}. Size {currentSize} out of {maxSize}.[Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping(typeof(Cache))]
  public class SitecoreCacheLazy : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public long currentSize;

    [InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> data;

    [InjectFieldValue]
    public long maxSize;

    [InjectFieldValue]
    public string name;

    public HashtableMappingModel Data => this.data?.Value as HashtableMappingModel;

    public string Filled => this.maxSize == 0 ? "[NoMaxSize]" : ((float)this.currentSize / this.maxSize).ToString("P");

    public static explicit operator PrefetchCacheModel(SitecoreCacheLazy model)
    {
      if (model == null)
      {
        return null;
      }

      var rs = new PrefetchCacheModel
      {
        Name = model.name,
        MaxSize = model.maxSize,
        CurrentSize = model.currentSize
      };
      if ((model.Data == null) || (model.Data.Elements == null))
      {
        return rs;
      }

      Hashtable elem = model.Data.Elements;

      rs.CachedValues = new Dictionary<ID, PrefetchDataModel>(elem.Count);

      foreach (DictionaryEntry pair in elem)
      {
        if (!(pair.Key is IDModel))
        {
          throw new InvalidCastException("Cannot cast {0} to ID".FormatWith(pair.Key.GetType().Name));
        }

        var key = (ID)(pair.Key as IDModel);

        var tmp = pair.Value as CacheEntryModel;
        if (tmp == null)
          continue;

        var val = tmp.data as PrefetchDataModel;
        if ((val == null) && (pair.Value != null))
        {
          throw new InvalidCastException("Cannot cast {0} to PrefetchDataModel.".FormatWith(pair.Value.GetType()));
        }

        rs.CachedValues.Add(key, val);
      }

      return rs;
    }

    public override string ToString()
    {
      return $"{this.name} Filled {this.Obj.HexAddress}";
    }
  }
}
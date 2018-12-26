using StringBuilder = System.Text.StringBuilder;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelMetadataInterfaces;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.StringExtensions;
  using Sitecore.Web.UI.WebControls;

  [ModelMapping(typeof(Placeholder))]
  public class WebPlaceholderMapping : ClrObjectMappingModel, ICaptionHolder
  {
    [InjectFieldValue]
    public string _cacheKey;

    // Internal enum for System.Web.UI.ControlState
    [InjectFieldValue]
    public int _controlState;

    [InjectFieldValue]
    public long _dataCacheHits;

    [InjectFieldValue]
    public long _dataCacheMisses;

    [InjectFieldValue]
    public string _dataSource;

    [InjectFieldValue]
    public string _id;

    [InjectFieldValue]
    public string _parameters;

    [InjectFieldValue]
    public long _physicalReads;

    [InjectFieldValue]
    public bool _usedCache;

    [InjectFieldValue]
    public string contextKey;

    public string CacheKey
    {
      get
      {
        return string.IsNullOrEmpty(_cacheKey) ? "[NoCacheKey]" : _cacheKey;
      }
    }

    public bool CachingConfigured
    {
      get
      {
        return !string.IsNullOrEmpty(_cacheKey);
      }
    }

    public string Caption
    {
      get
      {
        return _id + " (" + ParentName + ") Rendered: " + IsRendered + " Cacheable: " + CachingConfigured;
      }
    }

    public string DataSource
    {
      get
      {
        return string.IsNullOrEmpty(_dataSource) ? "[NoDataSource]" : _dataSource;
      }
    }

    public bool IsRendered
    {
      get
      {
        return _controlState > 4;
      }
    }

    public string ParentName
    {
      get
      {
        var val = Obj.GetRefFld("_parent");
        if (val.IsNullObj || (val.Type == null))
          return "[NoParent]";
        return val.Type.Name;
      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine("ID: {0} (Parent: {1})".FormatWith(_id, ParentName));
      if (IsRendered)
      {
        sb.AppendLine("Rendered from " + (_usedCache ? "cache" : " database"));
      }

      sb.AppendLine("Not yet rendered.");
      sb.AppendLine("Caching is {0} configured".FormatWith(CachingConfigured ? _cacheKey : "not"));
      sb.AppendLine("Data cache hits/misses:{0}/{1}, _physical reads:{2}".FormatWith(_dataCacheHits, _dataCacheMisses,
        _physicalReads));
      return sb.ToString();

      // string.Format( is rendered: {2}; Cacheble {3}")
    }
  }
}
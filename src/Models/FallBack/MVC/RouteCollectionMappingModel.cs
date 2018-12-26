using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.MVC
{
  [DebuggerDisplay("RouteCollection count: {Count}")]
  [ModelMapping(@"System.Web.Routing.RouteCollection")]
  public class RouteCollectionMappingModel : ClrObjectMappingModel
  {
    #region Fields
    /// <summary>
    /// Collection of all 'RouteBase'.
    /// </summary>
    [InjectFieldValue]
    public ArrayMappingModel items;

    /// <summary>
    /// Key - <see cref="StringModel"/>, value 'RouteBase'.
    /// </summary>
    [InjectFieldValue]
    protected HashtableMappingModel _namedMap;
    #endregion

    #region Properties
    /// <summary>
    /// A collection of route data per <see cref="string"/> key.
    /// </summary>
    public Dictionary<string, ClrObjectMappingModel> Map = new Dictionary<string, ClrObjectMappingModel>();

    public bool Contains(string key) => this.Map.ContainsKey(key);

    public int Count => this.items?.Count ?? 0;

    public bool IsEmpty => this.Count == 0;
    #endregion
    public override void Compute()
    {
      if (this._namedMap != null)
      {
        foreach (DictionaryEntry pair in _namedMap)
        {
          var keyModel = (pair.Key as StringModel);

          if (!keyModel.IsNullOrEmpty)
          {
            this.Map[keyModel.Value] = pair.Value as ClrObjectMappingModel;
          }
        }
      }

      base.Compute();
    }
  }
}

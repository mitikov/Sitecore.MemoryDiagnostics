using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.MVC
{
  [DebuggerDisplay("RouteValueDictionary: {Count}")]
  [ModelMapping(@"System.Web.Routing.RouteValueDictionary")]
  public class RouteValueDictionaryMappingModel : ClrObjectMappingModel, ICollection<KeyValuePair<string, ClrObjectMappingModel>>
  {
    #region Fields to Inject
    /// <summary>
    /// Key-value for routes.
    /// <para>Key is <see cref="string"/> name of the parameter from route.</para>
    /// <para>Value is <see cref="object"/> to be applied for the parameter from route.</para>
    /// <para>Example: parameter default value, or parameter validation expression.</para>
    /// </summary>
    [InjectFieldValue]
    protected HashtableMappingModel _dictionary;

    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    protected Dictionary<string, ClrObjectMappingModel> RouteValueDictionary = new Dictionary<string, ClrObjectMappingModel>();

    #region properties
    public bool Contains(string key) => this.RouteValueDictionary.ContainsKey(key);

    /// <summary>
    /// Number of elements in inner collection.
    /// </summary>
    public int Count => _dictionary?.Count ?? 0;

    public bool IsEmpty => this.Count == 0;

    public IEnumerable<string> Keys => this.RouteValueDictionary.Keys;

    public IEnumerable<ClrObjectMappingModel> Values => this.RouteValueDictionary.Values;

    public bool IsReadOnly => ((ICollection<KeyValuePair<string, ClrObjectMappingModel>>)RouteValueDictionary).IsReadOnly;

    public KeyValuePair<string, ClrObjectMappingModel>[] Pairs => (RouteValueDictionary as IEnumerable<KeyValuePair<string, ClrObjectMappingModel>>).ToArray();
    #endregion

    public override void Compute()
    {
      if (this._dictionary != null)
      {
        foreach (DictionaryEntry pair in _dictionary)
        {
          var keyModel = (pair.Key as StringModel);

          if (!keyModel.IsNullOrEmpty)
          {
            this.RouteValueDictionary[keyModel.Value] = pair.Value as ClrObjectMappingModel;
          }
        }
      }

      base.Compute();
    }

    public virtual string TextView()
    {
      var sb = new StringBuilder();
      foreach (var keyPair in RouteValueDictionary)
      {
        var key = keyPair.Key;
        var value = keyPair.Value;
        if (value is StringModel)
        {
          sb.AppendFormat($"Key: '{key}' value: '{((StringModel)value).Value}'");
        }
        else if ((value is ArrayMappingModel))
        {
          var array = (ArrayMappingModel)(value);
          if (array.IsEmpty)
          {
            sb.AppendLine($"Key: '{key}' value: 'empty array'");
          }
          else
          {
            sb.AppendLine($"Key: '{key}' value: 'Array'");
            foreach (var elem in (array).Elements)
            {
              sb.AppendLine(elem.ToString());
            }
            sb.AppendLine("'");
          }
        }
        else
        {
          sb.AppendFormat($"Key: '{key}' value: '{value}'");
        }
        sb.AppendLine($"[Value address:{value.HexAddress}]");
        sb.AppendLine("----------");
        sb.AppendLine();
      }

      return sb.ToString();
    }

    public override string ToString()
    {
      return this.TextView();
    }


    public ClrObjectMappingModel this[string key] => RouteValueDictionary[key];


    public IEnumerator<KeyValuePair<string, ClrObjectMappingModel>> GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<string, ClrObjectMappingModel>>)RouteValueDictionary).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<string, ClrObjectMappingModel>>)RouteValueDictionary).GetEnumerator();
    }

    public void Add(KeyValuePair<string, ClrObjectMappingModel> item)
    {
      ((ICollection<KeyValuePair<string, ClrObjectMappingModel>>)RouteValueDictionary).Add(item);
    }

    public void Clear()
    {
      ((ICollection<KeyValuePair<string, ClrObjectMappingModel>>)RouteValueDictionary).Clear();
    }

    public bool Contains(KeyValuePair<string, ClrObjectMappingModel> item)
    {
      return ((ICollection<KeyValuePair<string, ClrObjectMappingModel>>)RouteValueDictionary).Contains(item);
    }

    public void CopyTo(KeyValuePair<string, ClrObjectMappingModel>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<string, ClrObjectMappingModel>>)RouteValueDictionary).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, ClrObjectMappingModel> item)
    {
      return ((ICollection<KeyValuePair<string, ClrObjectMappingModel>>)RouteValueDictionary).Remove(item);
    }
  }
}

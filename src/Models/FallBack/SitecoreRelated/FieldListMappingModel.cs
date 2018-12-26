
namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Text;

  using Attributes;
  using BaseMappingModel;
  using Data;
  using Diagnostics;
  using Helpers;
  using InternalProcessing;
  using ModelFactory;
  using Sitecore;

  /// <summary>
  /// Represents <see cref="FieldList"/> mapping ( Sitecore version fields ).
  /// </summary>
  [ModelMapping(typeof(FieldList))]
  public class FieldListMappingModel : ClrObjectMappingModel, IEnumerable<KeyValuePair<ID, string>>
  {
    #region Injected fields

    [InjectFieldValue]
    public long _valueSize;

    [InjectFieldValue]
    protected LazyLoad<IClrObjMappingModel> _fields;

    #endregion

    #region private fields
    private Dictionary<ID, string> _converted;
    #endregion

    #region Properties
    public bool FieldsRead => this.fields?.Elements != null;



    [NotNull]
    protected Dictionary<ID, string> Converted
    {
      get
      {
        if (this._converted != null)
        {
          return this._converted;
        }

        if (this._fields != null)
        {
          this.InitFields();
        }
        else
        {
          this._converted = new Dictionary<ID, string>(0);
        }

        return this._converted;
      }
    }

    protected HashtableMappingModel fields => this._fields.Value as HashtableMappingModel;

    #region indexers
    public string this[ID id]
    {
      get
      {
        Assert.IsNotNull(this._fields, "No fields read!");
        return this.Converted.ContainsKey(id) ? this.Converted[id] : null;
      }
    }

    #endregion
    #endregion

    #region Static operators
    public static explicit operator Dictionary<ID, string>(FieldListMappingModel model)
    {
      return model?.Converted;
    }

    public static explicit operator FieldList(FieldListMappingModel model)
    {
      var enumer = model?.Converted;
      if (enumer == null)
      {
        return null;
      }

      var fld = new FieldList();

      foreach (var pair in enumer)
      {
        fld.Add(pair.Key, pair.Value);
      }

      return fld;
    }

    #endregion

    #region Public API
    public bool Contains(Guid ID)
    {
      return this.Contains(new ID(ID));
    }

    public bool Contains(ID id)
    {
      return !ReferenceEquals(id, null) && Converted.ContainsKey(id);
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      foreach (KeyValuePair<ID, string> keyValuePair in Converted)
      {
        sb.AppendLine($"{IDsNameHelper.ToString((ID)keyValuePair.Key)} -> {keyValuePair.Value}");
      }

      return sb.ToString();
    }

    #endregion

    #region  Enumeration


    public IEnumerator<KeyValuePair<ID, string>> GetEnumerator()
    {
      return this.Converted.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion


    #region private methods
    /// <summary>
    ///   Transforms read <see cref="_fields" /> into <see cref="System.Collections.Generic.Dictionary{TKey,TValue}" /> where  Field<see cref="ID" /> is
    ///   key and value is <see cref="string" />.
    /// </summary>
    private void InitFields()
    {
      if ((this.fields == null) || (this.fields.Count == 0))
      {
        // Init empty
        this._converted = new Dictionary<ID, string>(0);
        return;
      }

      this._converted = new Dictionary<ID, string>(this.fields.Elements.Count);

      foreach (DictionaryEntry pair in this.fields.Elements)
      {
        var key = (ID)(pair.Key as IDModel);

        if (ID.IsNullOrEmpty(key))
        {
          continue;
        }

        var val = (string)(pair.Value as StringModel);
        this._converted[key] = val;
      }
    }
    #endregion
  }
}
namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System.Collections.Generic;
  using System.Diagnostics;

  using Sitecore.Collections;
  using Sitecore.Data;

  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Helpers;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using Sitecore.StringExtensions;
  using DateTime = System.DateTime;
  using DictionaryEntry = System.Collections.DictionaryEntry;
  using Enumerable = System.Linq.Enumerable;
  using StringBuilder = System.Text.StringBuilder;

  /// <summary>
  /// Base mapping for Sitecore <see cref="Sitecore.Data.DataProviders.PrefetchData"/>.
  /// </summary>
  [DebuggerDisplay("ID: {ItemID}. Parent {ParentID} ")]
  [ModelMapping(typeof(Sitecore.Data.DataProviders.PrefetchData))]
  public class PrefetchDataModel : ClrObjectMappingModel, IDateTimeHolder
  {
    [InjectFieldValue]
    public IDList _childIds;

    [InjectFieldValue]
    public long _dataLength;

    [InjectFieldValue]
    public ItemDefinitionModel _itemDefinition;

    [InjectFieldValue]
    protected LazyLoad<IClrObjMappingModel> _fieldLists;

    [InjectFieldValue]
    protected ID _parentId;

    private Dictionary<string, FieldListMappingModel> fieldsByKey;

    public override string Caption => $"{this.ItemName} {this.ItemID}";

    public uint? ChildrenCount
    {
      get
      {
        if (this._childIds == null)
        {
          return null;
        }

        return (uint)this._childIds.Count;
      }
    }

    /// <summary>
    /// FAKE dateTime depending on _dataLength for sorting.
    /// </summary>
    /// <value>
    /// The datetime.
    /// </value>
    public DateTime Datetime => DateTime.Today.AddTicks(-this._dataLength * 1000);

    public Dictionary<string, FieldListMappingModel> Fields
    {
      get
      {
        if (this.fieldsByKey == null)
        {
          this.InitFields();
        }

        return this.fieldsByKey;
      }
    }

    public bool HasItemDefinition => (this._itemDefinition != null) && !ReferenceEquals(null, this._itemDefinition._itemID);

    public ID ItemID
    {
      get
      {
        return this.HasItemDefinition
          ? this._itemDefinition._itemID
          : ID.Null;
      }
    }

    public string ItemName => this.HasItemDefinition
      ? this._itemDefinition.ItemName
      : "[NoDefinition-no owner]";

    public ID ParentID => this._parentId ?? ID.Null;

    public long SizeOfAllFieldLists
    {
      get
      {
        return Enumerable.Sum(this.Fields, t => t.Value._valueSize);
      }
    }

    public double RatioListsWithKeys(string key)
    {
      if (this._dataLength == 0)
      {
        return 0;
      }

      var sizeOfListsWithKey = this.SizeOfFieldListsWithKey(key);
      return ((double)sizeOfListsWithKey) / this._dataLength;
    }

    public long SizeOfFieldListsWithKey(string key = "JA-JP")
    {
      Diagnostics.Assert.ArgumentNotNullOrEmpty(key, "no key provided");
      long sum = 0;
      foreach (KeyValuePair<string, FieldListMappingModel> keyValuePair in Fields)
      {
        if (keyValuePair.Key.Contains(key))
        {
          sum += keyValuePair.Value._valueSize;
        }
      }

      return sum;
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine($"Item Name: {this.ItemName} Has {this.ChildrenCount?.ToString() ?? "no"} children and its size is {StringUtil.GetSizeString(this._dataLength)}");

      sb.AppendLine($"ParentId: {this.ParentID}");
      if (this.Fields != null)
      {
        foreach (KeyValuePair<string, FieldListMappingModel> fields in Fields)
        {
          sb.AppendLine();
          sb.AppendLine("Processing fields of key " + fields.Key);
          foreach (KeyValuePair<ID, string> field in fields.Value)
          {
            sb.AppendLine($" \t {IDsNameHelper.ToString((ID)field.Key)} -> {field.Value}");
          }
        }
      }

      if ((_childIds != null) && (_childIds.Count > 0))
      {
        sb.AppendLine("Children: {0}".FormatWith(_childIds.Count));
        foreach (var childId in _childIds)
        {
          sb.AppendLine(childId.ToString());
        }
      }
      else
      {
        sb.AppendLine("No childrenIDs found");
      }

      return sb.ToString();
    }

    private void InitFields()
    {
      var hashtablemapping = this._fieldLists?.Value as HashtableMappingModel;
      if (hashtablemapping == null)
      {
        return;
      }

      this.fieldsByKey = new Dictionary<string, FieldListMappingModel>();

      foreach (DictionaryEntry keyValue in hashtablemapping)
      {
        // F.e. DA#1
        var key = (string)(keyValue.Key as StringModel);
        var val = keyValue.Value as FieldListMappingModel;
        this.fieldsByKey.Add(key, val);
      }
    }
  }
}
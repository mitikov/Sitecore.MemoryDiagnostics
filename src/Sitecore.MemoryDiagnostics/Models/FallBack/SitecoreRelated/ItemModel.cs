namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System;
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Data;
  using Sitecore.Data.Items;

  [DebuggerDisplay("_uniqueId [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping(typeof(Item))]
  public class ItemModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public ItemDataModel _innerData;

    [InjectFieldValue]
    public ID _itemID;

    [InjectFieldValue]
    public string _uniqueId;

    public override string Caption => base.Caption + (this._uniqueId.IsNullValue() ? this.ItemId : this._uniqueId);

    public string DisplayName => (this._innerData == null) ? "[NoInnerData]" : this._innerData.DisplayName;

    public string ItemId => ReferenceEquals(null, this._itemID) ? "[NoItemID]" : this._itemID.ToString();

    public string UniqueID => this._uniqueId.IsNullValue() ? "[NoUniqueId]" : this._uniqueId;

    public override string ToString()
    {
      return $"ItemId {this._itemID} (display name - {this.DisplayName}){Environment.NewLine}UniqueId {this.UniqueID}";
    }
  }
}
namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Data;

  [DebuggerDisplay("{_name}#{_itemID}. [ModelType] {GetType().Name}. Pointer {Obj.Address}")]
  [ModelMapping(typeof(ItemDefinition))]
  public class ItemDefinitionModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public ID _branchId;

    [InjectFieldValue]
    public bool _cacheable;

    [InjectFieldValue]
    public long _dataLength;

    [InjectFieldValue]
    public ID _itemID;

    [InjectFieldValue]
    public string _name;

    [InjectFieldValue]
    public ID _templateID;

    protected ItemDefinition itemDef;

    public override string Caption => base.Caption + this.ItemName + " [" + this.ItemID + "]";

    public ItemDefinition ItemDefinition => new ItemDefinition(this._itemID, this._name, this._templateID, this._branchId);

    public ID ItemId => this._itemID;

    public string ItemID => ID.IsNullOrEmpty(this._itemID) ? "[NoId]" : this._itemID.ToString();

    public string ItemName => string.IsNullOrEmpty(this._name) ?
      "[NoName]" : this._name;

    public static explicit operator ItemDefinition(ItemDefinitionModel model)
    {
      if (object.ReferenceEquals(model, null))
      {
        return null;
      }

      // TODO: fill asserts for model init.
      return new ItemDefinition(model._itemID, model._name, model._templateID, model._branchId);
    }

    public override string ToString()
    {
      var templateId = this._templateID?.ToString() ?? "[NoID]";
      return $"{this.Caption} based on TemplateID {templateId}";
    }
  }
}
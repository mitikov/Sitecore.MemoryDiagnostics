namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System;
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore;
  using Sitecore.Data;

  [DebuggerDisplay("{DisplayName}#{_language} [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping(typeof(ItemData))]
  public class ItemDataModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public ItemDefinitionModel _definition;

    [InjectFieldValue]
    public FieldListMappingModel _fields;

    [InjectFieldValue]
    public LanguageModel _language;

    public string DisplayName
    {
      get
      {
        if ((this._fields == null) || (!this._fields.FieldsRead) || (this._definition == null))
        {
          return "NoDisplayName";
        }

        var displayName = this._fields[FieldIDs.DisplayName];
        return string.IsNullOrEmpty(displayName) ? this.Name : displayName;
      }
    }

    public string Name => (this._definition == null) ? "[NoDefinition]" : this._definition._name;

    public DateTime? Updated
    {
      get
      {
        if ((this._fields == null) || (!this._fields.FieldsRead))
        {
          return null;
        }

        string date = this._fields[FieldIDs.Updated];
        if (string.IsNullOrEmpty(date))
        {
          return null;
        }

        DateTime res;
        if (DateTime.TryParse(date, out res))
        {
          return res;
        }

        return null;
      }
    }
  }
}
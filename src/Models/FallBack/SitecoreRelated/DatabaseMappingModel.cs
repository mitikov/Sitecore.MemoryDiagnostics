namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System.Diagnostics;
  using Sitecore.Data;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelMetadataInterfaces;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  /// <summary>
  ///   Mapping for Sitecore <see cref="Database" /> class.
  /// </summary>
  [DebuggerDisplay("_name [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping("Sitecore.Data.Database")]
  public class DatabaseMappingModel : ClrObjectMappingModel, ICaptionHolder
  {
    [InjectFieldValue]
    public DataProviderCollectionModel _dataProviders;

    [InjectFieldValue]
    public string _name;

    [InjectFieldValue]
    public bool _securityEnabled;

    public string Caption
    {
      get
      {
        return base.Caption + DatabaseName + " security: " + _securityEnabled;
      }
    }

    public string DatabaseName
    {
      get
      {
        return string.IsNullOrEmpty(_name) ? "[NoDatabaseName]" : _name;
      }
    }


    public override string ToString()
    {
      return DatabaseName + " Sitecore Database. Security: " + _securityEnabled;
    }
  }
}
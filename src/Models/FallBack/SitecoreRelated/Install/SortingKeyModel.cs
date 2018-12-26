namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.Install
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  /// <summary>
  /// Used for determining item installation order.
  /// </summary>
  /// <seealso cref="Models.BaseMappingModel.ClrObjectMappingModel" />
  [ModelMapping(@"Sitecore.Install.Framework.SortingKey")]
  public class SortingKeyModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public int contentRank;

    [InjectFieldValue]
    public int databaseRank;

    [InjectFieldValue]
    public int dependencyRank;

    [InjectFieldValue]
    public bool invertResult;

    [InjectFieldValue]
    public string path;

    [InjectFieldValue]
    public string prefix;

    [InjectFieldValue]
    public string uniqueid;

    [InjectFieldValue]
    public string version;

    public override string Caption => this.path;
  }
}
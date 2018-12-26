namespace Sitecore.MemoryDiagnostics.Models.FallBack.MongoRelated
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  [ModelMapping(@"MongoDB.Driver.Internal.MongoConnectionPool")]
  public class MongoConnectionPoolMappingModel : ClrObjectMappingModel, IHaveChildEntries
  {
    [InjectFieldValue]
    public ArrayMappingModel _availableConnections;

    [InjectFieldValue]
    public int _poolSize;

    [InjectFieldValue]
    public IClrObjMappingModel _settings;

    public override string Caption => this.HexAddress + base.Caption;

    public IEnumerable<IClrObjMappingModel> ChildrenEntries
    {
      get
      {
        if (!ArrayMappingModel.ArrayIsNullOrEmpty(this._availableConnections))
        {
          foreach (IClrObjMappingModel connection in this._availableConnections)
          {
            yield return connection;
          }
        }
      }
    }
  }
}
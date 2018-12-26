using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.OracleRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(@"Oracle.DataAccess.Client.OpoConCtx")]
  public class OpoConCtxModel : ClrObjectMappingModel, IDateTimeHolder
  {
    [InjectFieldValue]
    public DateTime creationTime;

    [InjectFieldValue]
    public int maxPoolSize;

    [InjectFieldValue]
    public int minPoolSize;

    private TimeSpan? _createdAgo;

    public override string Caption
    {
      get
      {
        return "Oracle Internal connection #" + HexAddress;
      }
    }

    public TimeSpan CreatedAgo
    {
      get
      {
        if (!_createdAgo.HasValue)
          _createdAgo = MetadataProviders.MetadataManager.GetDumpTime(Obj) - creationTime.ToUniversalTime();
        return _createdAgo.Value;
      }
    }

    public DateTime Datetime
    {
      get
      {
        return creationTime;
      }
    }

    public override string ToString()
    {
      return HexAddress + " executed for " + CreatedAgo;
    }
  }
}
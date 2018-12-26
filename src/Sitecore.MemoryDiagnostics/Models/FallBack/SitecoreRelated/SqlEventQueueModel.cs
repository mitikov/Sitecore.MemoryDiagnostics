using DateTime = System.DateTime;
using Environment = System.Environment;
using StringBuilder = System.Text.StringBuilder;
using TimeSpan = System.TimeSpan;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.MetadataProviders;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Data.Eventing;

  [ModelMapping(typeof(SqlServerEventQueue))]
  public class SqlEventQueueModel : ClrObjectMappingModel
  {
    public string InstanceName;

    [InjectFieldValue]
    public DateTime lastSavedStamp;

    public bool ListenToRemoteEvents;

    [InjectFieldValue]
    public EventQueueTimeStampModel timestamp;

    public override string Caption
    {
      get
      {
        return base.Caption + InstanceName;
      }
    }

    public TimeSpan StampSetTimeSpan
    {
      get
      {
        return MetadataManager.GetDumpTime(Obj) - lastSavedStamp;
      }
    }

    public override void Compute()
    {
      InstanceName = Obj.GetStringSafe("<InstanceName>k__BackingField");
      ListenToRemoteEvents = Obj.GetBoolFld("<ListenToRemoteEvents>k__BackingField");
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine("Instance name: " + InstanceName);
      sb.AppendLine("Listens to remote events: " + ListenToRemoteEvents);
      sb.AppendLine("Stamp was flushed to db :" + StampSetTimeSpan);
      if (timestamp != null)
        sb.AppendLine("Timestamp data: " + Environment.NewLine + timestamp);
      return sb.ToString();
    }
  }
}
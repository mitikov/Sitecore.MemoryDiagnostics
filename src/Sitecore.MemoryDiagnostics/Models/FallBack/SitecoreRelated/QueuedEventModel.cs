using DateTime = System.DateTime;
using Math = System.Math;
using StringBuilder = System.Text.StringBuilder;
using TimeSpan = System.TimeSpan;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.MetadataProviders;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders;
  using Sitecore;
  using Sitecore.MemoryDiagnostics.Helpers;

  [ModelMapping(typeof(Sitecore.Eventing.QueuedEvent))]
  public class QueuedEventModel : ClrObjectMappingModel
  {
    public DateTime Created;

    public string InstanceData;

    public string InstanceName;

    public long Timestamp;

    public string UserName;

    protected string eventTypeValue;

    public override string Caption
    {
      get
      {
        return base.Caption + EventTypeName;
      }
    }

    public TimeSpan CreatedAgo
    {
      get
      {
        return MetadataManager.GetDumpTime(Obj) - Created;
      }
    }

    [NotNull]
    public string EventType
    {
      get
      {
        return eventTypeValue.FallbackTo("[NoEventType]");
      }
    }

    public string EventTypeName
    {
      get
      {
        var classNameDelimiterPos = EventType.IndexOf(',');
        var start = Math.Max(EventType.IndexOf('.'), 0);
        return ((classNameDelimiterPos < 0) || (classNameDelimiterPos - start < 1)) ? EventType : EventType.Substring(start, classNameDelimiterPos - start);
      }
    }

    public override void Compute()
    {
      Created = DateTimeReader.FromObjToDate(Obj, "created");

      eventTypeValue = Obj.GetStringSafe("<EventTypeName>k__BackingField");

      InstanceData = Obj.GetStringSafe("<InstanceData>k__BackingField");

      InstanceName = Obj.GetStringSafe("<InstanceName>k__BackingField");

      Timestamp = Obj.GetInt64Fld("<Timestamp>k__BackingField");

      UserName = Obj.GetStringSafe("<UserName>k__BackingField");
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine("Event Type:" + eventTypeValue.FallbackTo("[No Event Type]"));
      sb.AppendLine("Timestamp: " + Timestamp.ToString("x"));
      sb.AppendLine("Raised by: " + InstanceName.FallbackTo("[No Instance name]"));
      sb.AppendLine("Raised at: " + Created.ToString("g"));
      sb.AppendLine("  " + CreatedAgo.ToString("g") + " ago");
      sb.AppendLine("User Name: " + UserName.FallbackTo("[No User Name]"));
      sb.AppendLine("Event data: ");
      sb.AppendLine(string.IsNullOrEmpty(InstanceData) ? "[No Instance data]" : JsonHelper.FormatJson(InstanceData));

      return sb.ToString();
    }
  }
}
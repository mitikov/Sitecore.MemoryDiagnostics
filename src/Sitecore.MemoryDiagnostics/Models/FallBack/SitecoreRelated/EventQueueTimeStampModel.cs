using DateTime = System.DateTime;
using Environment = System.Environment;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.MetadataProviders;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders;

  [ModelMapping(@"Sitecore.Eventing.EventQueue+TimeStamp")]
  public class EventQueueTimeStampModel : ClrObjectMappingModel
  {
    public long stamp;
    public DateTime time;

    public override string Caption
    {
      get
      {
        return base.Caption + stamp.ToString("x");
      }
    }

    public override void Compute()
    {
      stamp = Obj.GetInt64Fld("<Sequence>k__BackingField");

      time = DateTimeReader.FromObjToDate(Obj, "<Date>k__BackingField");
    }

    public override string ToString()
    {
      return "Stamp :" + stamp.ToString("X") + Environment.NewLine + "Time: " + (time == DateTime.MinValue ? "Not set" : (time.ToString("g") + " lag (" + (MetadataManager.GetDumpTime(Obj) - time).ToString("g")));
    }
  }
}
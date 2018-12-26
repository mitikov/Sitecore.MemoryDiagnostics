namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System.Diagnostics;
  using Attributes;
  using BaseMappingModel;
  using MetadataProviders;
  using Services;
  using DateTime = System.DateTime;
  using Math = System.Math;
  using StringBuilder = System.Text.StringBuilder;
  using TimeSpan = System.TimeSpan;

  [DebuggerDisplay("{m_interval}. Next {m_nextRing} [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping(typeof(AlarmClock))]
  public class AlarmClockModel : ClrObjectMappingModel, IDateTimeHolder
  {
    [InjectFieldValue]
    public TimeSpan m_interval;

    [InjectFieldValue]
    public DateTime m_nextRing;

    public override string Caption => base.Caption + Math.Round(this.m_interval.TotalSeconds, 2) + "s. interval";

    public DateTime Datetime => this.m_nextRing;

    public TimeSpan NextRingIn => this.m_nextRing - MetadataManager.GetDumpTime(this.Obj);

    public override string ToString()
    {
      var sb = new StringBuilder();

      sb.AppendLine("Interval: " + this.m_interval.ToString("g"));
      sb.AppendLine("Next ring: " + this.m_nextRing.ToString("G"));
      sb.AppendLine("Ring would happen in: " + this.NextRingIn.ToString("g") + " (if minus = due)");

      return sb.ToString();
    }
  }
}
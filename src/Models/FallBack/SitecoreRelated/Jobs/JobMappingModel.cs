using DateTime = System.DateTime;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.Jobs
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Jobs;

  [ModelMapping(typeof(Job))]
  public class JobMappingModel : ClrObjectMappingModel, IDateTimeHolder
  {
    [InjectFieldValue]
    public DateTime _queueTime;

    public string JobName;

    [InjectFieldValue]
    public IClrObjMappingModel m_status;

    [InjectFieldValue]
    public IClrObjMappingModel options;

    public DateTime Datetime => this._queueTime;
  }
}
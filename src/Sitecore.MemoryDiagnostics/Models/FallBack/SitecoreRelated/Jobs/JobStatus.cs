namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.Jobs
{
  using Attributes;
  using BaseMappingModel;
  using Sitecore.Jobs;

  [ModelMapping(typeof(JobStatus))]
  public class JobStatusMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool m_failed;

    [InjectFieldValue]
    public long m_processedWork;

    [InjectFieldValue]
    public IClrObjMappingModel m_result;

    [InjectFieldValue]
    public JobState m_state;
  }
}
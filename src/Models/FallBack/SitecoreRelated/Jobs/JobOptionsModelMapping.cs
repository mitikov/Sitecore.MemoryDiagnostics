using TimeSpan = System.TimeSpan;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Jobs;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  [ModelMapping(typeof(JobOptions))]
  public class JobOptionsModelMapping : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool atomic;

    [InjectFieldValue]
    public string clientLanguage;

    [InjectFieldValue]
    public IClrObjMappingModel ContextUser;

    [InjectFieldValue]
    public IClrObjMappingModel CustomData;

    [InjectFieldValue]
    public bool EnableSecurity;

    [InjectFieldValue]
    public TimeSpan initialDelay;

    [InjectFieldValue]
    public string jobName;

    [InjectFieldValue]
    public ArrayMappingModel parameters;

    public override string Caption => base.Caption + this.jobName;
  }
}
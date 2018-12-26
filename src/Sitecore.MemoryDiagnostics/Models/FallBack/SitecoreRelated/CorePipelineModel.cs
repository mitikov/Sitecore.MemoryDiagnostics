using StringBuilder = System.Text.StringBuilder;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using Sitecore.Pipelines;

  [ModelMapping(typeof(CorePipeline))]
  public class CorePipelineModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    protected string _name;

    [InjectFieldValue]
    protected ArrayMappingModel _processors;

    public override string Caption
    {
      get
      {
        return base.Caption + Name;
      }
    }


    public string Name
    {
      get
      {
        return _name ?? "[NoPipelineName]";
      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine("Pipeline: " + Name);
      if ((_processors == null) || (_processors.Count == 0))
      {
        sb.AppendLine("No processors found");
        return sb.ToString();
      }

      foreach (IClrObjMappingModel processor in _processors)
      {
        sb.AppendLine(processor.ToString());
        sb.AppendLine("-----");
      }

      return string.Intern(sb.ToString());
    }
  }
}
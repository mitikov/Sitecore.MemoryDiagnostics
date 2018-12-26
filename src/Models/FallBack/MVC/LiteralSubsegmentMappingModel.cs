using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.MVC
{
  [ModelMapping(@"System.Web.Routing.LiteralSubsegment")]
  public class LiteralSubsegmentMappingModel : ClrObjectMappingModel
  {

    [InjectFieldValue]
    public string Literal;
  }

  [ModelMapping(@"System.Web.Routing.ParameterSubsegment")]
  public class ParameterSubsegmentMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool IsCatchAll;

    [InjectFieldValue]
    public string ParameterName;
  }



  /// <summary>
  /// 
  /// </summary>
  [ModelMapping(@"System.Web.Routing.ParsedRoute")]
  public class ParsedRouteMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string _url;

    /// <summary>
    /// An array of PathSegment-derived classes.
    /// <para><see ref="ParameterSubsegmentMappingModel"/>, or <see cref="LiteralSubsegmentMappingModel"/>.</para>
    /// </summary>
    [InjectFieldValue]
    public ArrayMappingModel PathSegments;
  }
}

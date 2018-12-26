using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.Log4net
{
  [ModelMapping(QuietTextWriterTypeName)]
  public class QuietTextWriterModel : ClrObjectMappingModel
  {
    public const string QuietTextWriterTypeName = "log4net.helpers.QuietTextWriter";

    [InjectFieldValue]
    public bool m_closed;

    [InjectFieldValue]
    protected IClrObjMappingModel m_writer;

    public IClrObjMappingModel InnerWritter => m_writer;
  }
}

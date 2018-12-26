using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.Log4net
{
  [ModelMapping(SitecoreLogFileAppenderTypeName)]
  public class SitecoreLogFileAppenderModel: ClrObjectMappingModel
  {
    public const string SitecoreLogFileAppenderTypeName = @"log4net.Appender.SitecoreLogFileAppender";

    #region Injected fields
    [InjectFieldValue]
    protected string m_name;

    [InjectFieldValue]
    protected IClrObjMappingModel m_qtw;

    [InjectFieldValue]
    public bool m_immediateFlush;

    [InjectFieldValue]
    public string m_fileName;

    [InjectFieldValue]
    public string m_originalFileName;

    [InjectFieldValue]
    public DateTime m_currentDate;
    #endregion

    public IClrObjMappingModel TextWritter => m_qtw;

    public string LogAppenderName => m_name;
  }
}

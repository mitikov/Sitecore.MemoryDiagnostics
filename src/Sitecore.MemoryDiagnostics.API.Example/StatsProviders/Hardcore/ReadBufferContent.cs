using System.Linq;
using System.Text;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.Extensions;
using Sitecore.MemoryDiagnostics.Models.FallBack.IO;
using Sitecore.MemoryDiagnostics.Models.FallBack.Log4net;
using Sitecore.MemoryDiagnostics.SourceFactories;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.API.Example.StatsProviders.Hardcore
{               
  /// <summary>
  /// Output content of Sitecore Log API buffers.
  /// <para>Shows messages flushed when a snapshot was collected</para>
  /// </summary>
  public class SitecoreLogsBufferContent
  {

    public virtual StringBuilder DoProcessing(MDFileConnection memoryDumpFileConnection)
    {

      var sb = new StringBuilder();
      var map = ModelMapperManager.NewMapperFactory;
      var runtime = MDClrRuntimeFactory.Instance.BuildClrRuntime(memoryDumpFileConnection);


      var messages = from fileAppender in map.ExtractFromHeap<SitecoreLogFileAppenderModel>(memoryDumpFileConnection)
                     let textWriterModel = fileAppender.TextWritter as QuietTextWriterModel
                     let innerWriter = textWriterModel?.InnerWritter as StreamWriterModel
                     let lastLogEntry = innerWriter?.InMemoryContent
                     where !string.IsNullOrEmpty(lastLogEntry)
                     select new
                     {
                       loggerName = fileAppender.LogAppenderName,
                       file = fileAppender.m_fileName,
                       logTime = fileAppender.m_currentDate,
                       bufferAddress = innerWriter.HexAddress,
                       lastLogEntry
                     };

      messages.ToArray().ForEach(message => sb.AppendLine(message.ToString()));

      return sb;
    }
  }
}

using System;
using System.Net;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  [ModelMapping(typeof(HttpWebRequest))]
  public class HttpWebRequestMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public int m_Aborted;

    [InjectFieldValue]
    public bool m_Pipelined;

    [InjectFieldValue]
    public bool m_BodyStarted;

    [InjectFieldValue]
    private long m_StartTimestamp;

    [InjectFieldValue]
    public bool m_RequestSubmitted;

    [InjectFieldValue]
    public bool m_OnceFailed;

    public DateTime StartTime => DateTimeReader.TicksToDt((ulong)m_StartTimestamp);

    [InjectFieldValue]
    private IClrObjMappingModel _Uri;

    public string Url => (_Uri as UriModel)?.m_String ?? TextConstants.NoUrlText;

    public bool ContainUrl => (_Uri as UriModel)!=null;
  }
}

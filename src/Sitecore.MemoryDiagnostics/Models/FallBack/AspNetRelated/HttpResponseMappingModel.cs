using Environment = System.Environment;
using HttpResponse = System.Web.HttpResponse;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  /// <summary>
  ///   Has <see cref="HttpStatusCode" />, type of content, and if <see cref="WasFlushed" />.
  /// </summary>
  [DebuggerDisplay("{HttpStatusCode} {_contentType}. IsCompleted {_completed} {HexAddress}")]
  [ModelMapping(typeof(HttpResponse))]
  public class HttpResponseMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool _completed;

    [InjectFieldValue]
    public string _contentType;

    [InjectFieldValue]
    public bool _flushing;

    [InjectFieldValue]
    public bool _headersWritten;

    [InjectFieldValue]
    public int _statusCode;

    [InjectFieldValue]
    public ClrObjectMappingModel _wr;

    [InjectFieldValue]
    public bool _contentLengthSet;

    /// <summary>
    ///   <see cref="_flushing" /> is set during <see cref="System.Web.HttpResponse.Flush" /> method execution.
    ///   <para>Shows if response is being flushed via network</para>
    /// </summary>
    /// <value>
    ///   <c>true</c> if [being flushed]; otherwise, <c>false</c>.
    /// </value>
    public bool BeingFlushed => this._flushing;

    public bool HasIIS7WorkerRequest => this.WorkerRequest != null;

    public bool HasWorkerRequest => this._wr != null;

    public int HttpStatusCode => this._statusCode;

    /// <summary>
    ///   <see cref="_completed" /> is set in the end of <see cref="System.Web.HttpResponse.Flush" /> method.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [was flushed]; otherwise, <c>false</c>.
    /// </value>
    public bool WasFlushed => this._completed;

    public IIS7WorkerRequestModel WorkerRequest => this._wr as IIS7WorkerRequestModel;

    public override string ToString()
    {
      var text = $"HttpResponse {this.HttpStatusCode} HTTP Status code so far. {this._contentType} type. Headers formed {this._headersWritten} ";

      if (this.HasIIS7WorkerRequest)
      {
        text += Environment.NewLine + this.WorkerRequest.ToString();
      }

      return text;
    }
  }
}
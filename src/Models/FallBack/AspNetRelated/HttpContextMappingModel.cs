namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  using System.Diagnostics;
  using Sitecore;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  using DateTime = System.DateTime;
  using Environment = System.Environment;
  using HttpContext = System.Web.HttpContext;
  using TimeSpan = System.TimeSpan;

  /// <summary>
  ///   Mapping for <see cref="System.Web.HttpContext" /> class.
  ///   <para>Has <see cref="URL" />, <see cref="ExecutionDuration" />, <see cref="StackTrace" /> and others.</para>
  /// </summary>
  [DebuggerDisplay("{URL} Executed for {ExecutionDuration}. ThreadId {ManagedThreadId}. Obj {HexAddress}")]
  [ModelMapping(typeof(HttpContext))]
  public class HttpContextMappingModel : ClrObjectMappingModel, IDateTimeHolder
  {
    // HashtableModel would be injected here.
    [InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> _items;

    [InjectFieldValue]
    public HttpRequestMappingModel _request;

    [InjectFieldValue]
    public HttpResponseMappingModel _response;

    [InjectFieldValue]
    public bool _finishPipelineRequestCalled;

    [InjectFieldValue]
    [CanBeNull]
    public ThreadMappingModel _thread;

    [InjectFieldValue]
    public long _timeoutStartTimeUtcTicks;

    [InjectFieldValue]
    public long _timeoutTicks;

    [InjectFieldValue]
    public DateTime _utcTimestamp;

    [InjectFieldValue]
    public IClrObjMappingModel _wr;

    public string HandlerName;

    public override string Caption => $"[{this.InstanceTypeName}] {this.URL}";

    /// <summary>
    ///   Gets the context creation time.
    ///   <para>Set inside <see cref="System.Web.HttpContext" /> class constructors (call to Init private method)</para>
    /// </summary>
    /// <value>
    ///   The context creation time.
    /// </value>
    public DateTime ContextCreationTime => this._utcTimestamp;

    public DateTime Datetime => this.ContextCreationTime;

    /// <summary>
    ///   Gets the duration of the execution.
    /// </summary>
    /// <value>
    ///   The duration of the execution.
    /// </value>
    public TimeSpan ExecutionDuration => this.WorkerRequestModel?.ExecutionDuration ?? TimeSpan.Zero;

    /// <summary>
    ///   Content of <see cref="System.Web.HttpContext.Items" />.
    ///   <para>WARNING - SLOW!! </para>
    /// </summary>
    /// <value>
    ///   The items.
    /// </value>
    public HashtableMappingModel Items => this._items.Value as HashtableMappingModel;

    /// <summary>
    ///   Gets the managed thread identifier.
    /// </summary>
    /// <value>
    ///   The managed thread identifier.
    /// </value>
    public int ManagedThreadId
    {
      get
      {
        if (this._thread == null)
        {
          return -1;
        }

        return this._thread.m_ManagedThreadId;
      }
    }

    /// <summary>
    /// Gets the request assigned to Context.
    /// </summary>
    /// <value>
    /// The request.
    /// </value>
    public HttpRequestMappingModel Request => this._request;

    /// <summary>
    /// Gets the response assigned to Context.
    /// </summary>
    /// <value>
    /// The response.
    /// </value>
    public HttpResponseMappingModel Response => this._response;

    /// <summary>
    ///   Gets the stack trace.
    /// </summary>
    /// <value>
    ///   The stack trace.
    /// </value>
    [NotNull]
    public string StackTrace => this._thread == null ? "[No thread associated with HttpContext]" : this._thread.StackTrace;

    /// <summary>
    ///   Gets a value indicating whether thread is assigned for current request.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [thread assigned]; otherwise, <c>false</c>.
    /// </value>
    public bool HasThreadAssigned => this._thread != null;

    /// <summary>
    ///   Gets the URL.
    /// </summary>
    /// <value>
    ///   The URL.
    /// </value>
    [NotNull]
    public string URL => this.WorkerRequestModel == null ? TextConstants.NoUrlText : this.WorkerRequestModel.URL;

    public bool HasURL => URL != TextConstants.NoUrlText;

    [CanBeNull]
    public IIS7WorkerRequestModel WorkerRequestModel => this._wr as IIS7WorkerRequestModel;

    public override void Compute()
    {
      ClrObject hander = this.Obj.GetRefFld("_handler");

      if (hander.IsNullObj || (hander.Type == null))
      {
        this.HandlerName = "[No Handler Assigned]";
      }
      else
      {
        this.HandlerName = hander.Type.Name;
      }
    }

    public override string ToString()
    {
      return "HttpContext was created " + this.ExecutionDuration.ToString("g") + " ago." + Environment.NewLine + "Has thread Assigned " + this.HasThreadAssigned;
    }
  }
}
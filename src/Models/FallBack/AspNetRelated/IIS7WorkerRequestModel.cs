namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;

  using Attributes;
  using BaseMappingModel;
  using InternalProcessing;
  using MetadataProviders;
  using ModelFactory;
  using ModelMetadataInterfaces;
  using Sitecore;

  /// <summary>
  ///   Base info about executed request <see cref="URL" />, <see cref="ExecutionStartTime" />,
  ///   <see cref="ExecutionDuration" />
  /// </summary>
  [DebuggerDisplay("{URL} executed for {SecondsExecuted} seconds, {GetType().Name} {HexAddress}")]
  [ModelMapping(WellknownTypeNames.IISSpecific.IIS7WorkerRequest)]
  public class IIS7WorkerRequestModel : ClrObjectMappingModel, IDateTimeHolder, ICaptionHolder
  {
    #region Injected fields
    [InjectFieldValue]
    public string _cacheUrl;

    [InjectFieldValue]
    public bool _connectionClosed;

    [InjectFieldValue]
    public bool _disconnected;

    [InjectFieldValue]
    public bool _headersSent;

    [InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> _knownRequestHeaders;    

    /// <summary>
    ///   Time when object instance was created.
    ///   <para>FilledText inside <see cref="System.Web.HttpWorkerRequest" /> constructor</para>
    /// </summary>
    [InjectFieldValue]
    public DateTime _startTime;

    [InjectFieldValue]
    public bool _traceEnabled;

    [InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> _unknownRequestHeaders;

    #endregion

    public string Caption
    {
      get
      {
        return base.Caption + URL;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether <see cref="System.Web.HttpWorkerRequest.EndFlush" /> was called.
    ///   <para>If flush was called, then request considered to be processed.</para>
    /// </summary>
    /// <value>
    ///   <c>true</c> if [flush called]; otherwise, <c>false</c>.
    /// </value>
    public bool ConnectionClosed
    {
      get
      {
        return _connectionClosed;
      }
    }

    public DateTime Datetime
    {
      get
      {
        return ExecutionStartTime;
      }
    }

    public TimeSpan ExecutionDuration
    {
      get
      {
        DateTime dt = MetadataManager.GetDumpTime(Obj);
        return dt - _startTime;
      }
    }

    public DateTime ExecutionStartTime
    {
      get
      {
        return _startTime;
      }
    }

    public string[] KnownRequestHeaders
    {
      get
      {
        var list = new List<string>();

        var known = _knownRequestHeaders.Value as ArrayMappingModel;
        if (known == null)
          return list.ToArray();

        foreach (object knownRequestHeader in known)
        {
          if ((knownRequestHeader is EmptyClrObjectModel) || (knownRequestHeader as StringModel == null))
            continue;

          list.Add((knownRequestHeader as StringModel).Value);
        }

        return list.ToArray();
      }
    }

    public double SecondsExecuted
    {
      get
      {
        return Math.Round(ExecutionDuration.TotalSeconds, 3);
      }
    }

    public Dictionary<string, string> UnknownHeaders
    {
      get
      {
        var unknown = _unknownRequestHeaders.Value as ArrayMappingModel;
        if ((unknown == null) || (unknown.Count == 0))
          return new Dictionary<string, string>();

        var res = new Dictionary<string, string>(unknown.Count);
        foreach (object unknownRequestHeader in unknown)
        {
          // ArrayMapping of 2 elements
          var keyValuePair = unknownRequestHeader as ArrayMappingModel;
          if ((keyValuePair == null) || (keyValuePair.Count != 2))
            continue;
          var key = (string)(keyValuePair[0] as StringModel);
          var value = (string)(keyValuePair[1] as StringModel);
          if (string.IsNullOrEmpty(key) || (string.IsNullOrWhiteSpace(value)))
            continue;
          res.Add(key, value);
        }

        return res;
      }
    }

    /// <summary>
    ///   Gets the Request URL.
    /// </summary>
    /// <value>
    ///   The URL.
    /// </value>
    [NotNull]
    public string URL
    {
      get
      {
        return string.IsNullOrEmpty(_cacheUrl) ? TextConstants.NoUrlText : _cacheUrl;
      }
    }

    public override string ToString()
    {
      return string.Concat(URL, Environment.NewLine, "Executed for ", SecondsExecuted, " seconds", Environment.NewLine,
        "Execution started at ", ExecutionStartTime.ToString("G"), Environment.NewLine, "Connection closed: ", this.ConnectionClosed);
    }
  }
}
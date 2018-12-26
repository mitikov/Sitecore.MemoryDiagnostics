namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  using System.Diagnostics;
  using System.Web;

  using Attributes;
  using BaseMappingModel;
  using ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  /// <summary>
  ///   HttpMethod, rawUrl
  /// </summary>
  [DebuggerDisplay("{_httpMethod} {_rawUrl}. {HexAddress}")]
  [ModelMapping(typeof(HttpRequest))]
  public class HttpRequestMappingModel : ClrObjectMappingModel
  {
    #region Fields to Inject
    [InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> _cookies;

    [InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> _form;

    [InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> _headers;

    [InjectFieldValue]
    public string _httpMethod;

    [InjectFieldValue]
    public string _rawUrl;

    #endregion

    #region ICaptionHolder

    public override string Caption => this.ToString();

    #endregion

    #region Properties
    public string HttpMethod => string.IsNullOrEmpty(this._httpMethod) ? "[NoMethodInfo]" : this._httpMethod;

    public string RawUrl => string.IsNullOrEmpty(this._rawUrl) ? "[NoUrl]" : this._rawUrl;

    public string AspNetSessionId => GetCookieValue(TextConstants.CookieNames.AspNetSession);

    #endregion
    public override string ToString()
    {
      return $"{this.HttpMethod}#{this.RawUrl}";
    }

    public virtual bool HasCookie(string name)
    {
      var allCookies = _cookies?.Value as HashtableMappingModel;
      return allCookies?.Elements?.ContainsKey(name) == true;
    }

    public virtual string GetCookieValue(string name)
    {
      var allCookies = _cookies?.Value as HashtableMappingModel;
      var needed = allCookies?[name] as HttpCookieModel;
      return needed?.Value;      
    } 
  }
}
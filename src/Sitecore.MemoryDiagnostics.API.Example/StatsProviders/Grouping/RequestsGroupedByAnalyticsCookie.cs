using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Diagnostics.Runtime;
using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
using Sitecore.MemoryDiagnostics.ClrObjectEnumerators.Base;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
using Sitecore.MemoryDiagnostics.SourceFactories;

namespace Sitecore.MemoryDiagnostics.API.Example.DataProviders
{
  /// <summary>
  /// Groups requests by Analytics <see cref="TextConstants.CookieNames.SitecoreAnalyticsGlobal"/>.
  /// <para>Allows to figure out if multiple requests with same analytics cookie are bashing Shared Session State.</para>
  /// </summary>
  public class RequestsGroupedByAnalyticsCookie
  {

    protected readonly IEnumerateClrObjectsFromClrRuntime ObjectEnumerator;

    protected readonly IFilteredObjectsProvider ObjectFilter;

    #region Constructors
    public RequestsGroupedByAnalyticsCookie() : this(new ThreadStackObjects())
    {

    }

    public RequestsGroupedByAnalyticsCookie(IEnumerateClrObjectsFromClrRuntime objectEnumerator) : this(objectEnumerator, new FilteredObjectProviderByTypeName(typeof(HttpContext)))
    {
    }

    public RequestsGroupedByAnalyticsCookie(IEnumerateClrObjectsFromClrRuntime objectEnumerator, IFilteredObjectsProvider filter)
    {
      ObjectEnumerator = objectEnumerator;
      ObjectFilter = filter;
    }

    #endregion

    public virtual StringBuilder DoProcessing(MDFileConnection memoryDumpFileConnection)
    {
      var contacts = ExtractRequestsByContact(memoryDumpFileConnection);
      var sb = new StringBuilder();


      foreach (var contact in contacts)
      {
        sb.AppendLine(new string(Enumerable.Repeat('*', 40).ToArray()));

        sb.AppendLine($"{contact.ContactId} has {contact.Count} requests with {contact.DistinctAspNet} asp.Net sessions:");
        foreach (var request in contact.Requests)
        {
          sb.AppendLine(request.ToString());
        }

        sb.AppendLine();

      }

      return sb;
    }

    public virtual IOrderedEnumerable<ContactInfo> ExtractRequestsByContact(ClrRuntime runtime, IModelMapperFactory modelMapperFactory)
    {
      var httpContextStreamClrObject = ObjectFilter.ExtractFromRuntime(runtime, ObjectEnumerator);


      var requestsGroupedByContactID = from clrObject in httpContextStreamClrObject
                                       let context = modelMapperFactory.BuildModel(clrObject) as HttpContextMappingModel
                                       where IsValid(context)

                                       let request = context._request as HttpRequestMappingModel
                                       where request != null

                                       let cookies = request._cookies.Value as HashtableMappingModel
                                       where cookies != null

                                       where cookies.Elements.ContainsKey(TextConstants.CookieNames.SitecoreAnalyticsGlobal)
                                      // where cookies.Elements.ContainsKey(TextConstants.CookieNames.AspNetSession)

                                       let analyticsCookie = cookies[TextConstants.CookieNames.SitecoreAnalyticsGlobal] as HttpCookieModel

                                       let sessionCookie = cookies[TextConstants.CookieNames.AspNetSession] as HttpCookieModel

                                       where !string.IsNullOrEmpty(analyticsCookie.Value)
                                       // where !string.IsNullOrEmpty(sessionCookie.Value)

                                       let workerRequest = context._wr as IIS7WorkerRequestModel

                                       where workerRequest != null                                       

                                       let metadata = new
                                       {
                                         context.URL,
                                         TotalSeconds = Math.Round(context.ExecutionDuration.TotalSeconds, 2),
                                         analyticsId = analyticsCookie.Value,
                                         aspSession = sessionCookie?.Value ?? "[NoSession]",
                                         context = context.HexAddress,
                                         request = request.HexAddress
                                       }

                                       where metadata.TotalSeconds > 0

                                       group metadata by metadata.request into uniqueRequests

                                       let uniqueRequest = uniqueRequests.First()

                                       group uniqueRequest by uniqueRequest.analyticsId into grouped

                                       orderby grouped.Count() descending

                                       let contactInfo = new ContactInfo
                                       {
                                         ContactId = grouped.Key,
                                         Count = grouped.Count(),
                                         DistinctAspNet = grouped.Select(g => g.aspSession).Distinct().Count(),
                                         Requests = grouped.Select(requestInfo => new RequestInfo
                                         {
                                           URL = requestInfo.URL,
                                           TotalSeconds = requestInfo.TotalSeconds,
                                           aspSession = requestInfo.aspSession,
                                           context = requestInfo.context,
                                           request = requestInfo.request
                                         }).OrderByDescending(g => g.TotalSeconds),
                                       }

                                       select contactInfo;

      return requestsGroupedByContactID.OrderByDescending(t => t.Count);
    }


    public virtual IOrderedEnumerable<ContactInfo> ExtractRequestsByContact(MDFileConnection instance)
    {
      var runtime = MDClrRuntimeFactory.Instance.BuildClrRuntime(instance);

      var modelMapperFactory = ModelMapperManager.NewMapperFactory;

      return ExtractRequestsByContact(runtime, modelMapperFactory);
    }

    protected virtual bool IsValid(HttpContextMappingModel context)
    {
      return (context?.HasURL == true);//&& context.HasThreadAssigned;
    }

    public class RequestInfo
    {
      public string URL { get; set; }

      public double TotalSeconds { get; set; }

      public string aspSession { get; set; }

      public string context { get; set; }

      public string request { get; set; }

      public string ip { get; set; }


      public override string ToString() => $"Url={URL} executed {TotalSeconds} sec. Session ID:{aspSession}. IP: {ip} Context:{context} Request:{request}";
    }
  }

  public class ContactInfo
  {
    public string ContactId { get; set; }
    public int Count { get; set; }
    public int DistinctAspNet { get; set; }

    public IOrderedEnumerable<RequestsGroupedByAnalyticsCookie.RequestInfo> Requests { get; set; }

    public override string ToString() => $"Contact {ContactId} has {Count} requests with {DistinctAspNet} distinct AspNet sessions.";
  }

}

using System.Collections.Generic;

namespace Sitecore.MemoryDiagnostics.ModelFilters
{
  public class BlacklistObjectFilter : SkipsObjectsOfKnownType
  {

    public static readonly IReadOnlyList<string> typesToIgnore = new[]
    {
      "System.RuntimeMethodInfoStub",
      "Sitecore.Diagnostics.PerformanceCounters.PerformanceCounter+<>c__DisplayClass2",
      "System.Func<Sitecore.Pipelines.ItemProvider.GetItem.GetItemArgs>",
      "Sitecore.ContentTesting.Caching.VersionRedirectionRequestCache",
      "System.RuntimeType",
      "Sitecore.Caching.Generics.ContextCache<Sitecore.Data.Templates.TemplateField>",
      "System.Func<Sitecore.Data.Items.Item>",
      "Sitecore.Data.Managers.DefaultItemManager+<>c__DisplayClass2f",
      "System.RuntimeMethodInfoStub",
      "System.RuntimeFieldInfoStub",
      "Sitecore.Mvc.Common.ContextService",
      "System.Reflection.RuntimeModule",
      "System.Web.Mvc.IResultFilter",
      "Sitecore.Mvc.Pipelines.PipelineService",
      "System.Runtime.CompilerServices.CallSite<System.Func<System.Runtime.CompilerServices.CallSite,System.Object,System.Object>>",
      "Sitecore.Diagnostics.PerformanceCounters.PerformanceCounter",
      "Sitecore.Diagnostics.Profiling.NullPipelineProfilingScope",
      "Sitecore.Mvc.Filters.PipelineBasedRequestFilter",
      "Sitecore.Security.AccessControl.ItemAccess",
      "Sitecore.Security.Accounts.AccountType",
      "Sitecore.Security.AccessControl.PropagationType",
      "Sitecore.Common.Reference<Sitecore.Data.ItemUri>",
      "Sitecore.SecurityModel.SecurityDisabler",
      "Sitecore.Data.Comparers.KeyObj",
      "Sitecore.SecurityModel.SecurityState",
      "Sitecore.Data.Items.ItemAppearance",
      "System.Func<Sitecore.ContentSearch.Linq.Methods.QueryMethod,System.Boolean>",
      "Newtonsoft.Json.Linq.JToken+LineInfoAnnotation",
      "Newtonsoft.Json.Linq.JProperty+JPropertyList"
    };

    public BlacklistObjectFilter() : base(typesToIgnore)
    {

    }
  }
}

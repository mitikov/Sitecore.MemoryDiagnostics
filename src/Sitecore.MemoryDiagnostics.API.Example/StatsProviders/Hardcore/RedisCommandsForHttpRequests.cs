using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Diagnostics.Runtime;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.StringBasedEnumerators;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.Extensions;
using Sitecore.MemoryDiagnostics.Helpers;
using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;
using Sitecore.MemoryDiagnostics.SourceFactories;
using Sitecore.MemoryDiagnostics.ThreadStackEnumerators;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.API.Example.StatsProviders.Hardcore
{
  /// <summary>
  /// Scans user thread stacks for Redis commands, and <see cref="HttpContext"/> objects.
  /// <para>Provides information for how long given request was processed, and for what URL.</para>
  /// </summary>
  public class RedisCommandsForHttpRequests
  {

    protected UserThreadsFilter ThreadFilter;

    protected FilteredObjectsProviderBase ThreadStackObjectFilter;


    public RedisCommandsForHttpRequests(UserThreadsFilter threadFilter = null, FilteredObjectsProviderBase threadStackObjectsFilter = null)
    {
      ThreadFilter = threadFilter ?? new UserThreadsFilter();

      if (threadStackObjectsFilter == null)
      {
        var httpContextFilter = new FilteredObjectProviderByTypeName(typeof(HttpContext));
        var stringFilter = new PredicateBasedStringFilter(predicate: s => s.Contains("redis.call"));
        ThreadStackObjectFilter = new MultipleConditionFilteredObjectsProvider(httpContextFilter, stringFilter);
      }
    }

    /// <summary>
    /// Provides text statistics for pending <see cref="HttpContext"/> that wait for Redis commands.
    /// <para>Scans thread stacks for <see cref="HttpContext"/> instances, and <see cref="string"/>s that contain 'redis.call' text inside.</para>
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="connectionFactory"></param>
    /// <param name="threadFilter"></param>
    /// <param name="threadStackObjectsFilter"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public virtual StringBuilder ExtractRedisPendingCommandsForRequests(IMemoryDumpConnectionPath connection, IClrRuntimeFactory connectionFactory = null,
       IModelMapperFactory factory = null)
    {
      var runtime = (connectionFactory ?? MDClrRuntimeFactory.Instance).BuildClrRuntime(connection);


      return ExtractRedisPendingCommandsForRequests(runtime, factory ?? ModelMapperManager.NewMapperFactory);
    }

    public virtual StringBuilder ExtractRedisPendingCommandsForRequests(ClrRuntime runtime, IModelMapperFactory clrObjectToModelFactory)
    {
      var knownObjects = new HashSet<ulong>();

      var threadStackObjectEnumerator = new ThreadStackEnumerator(
        includePossiblyDead: true,
        filter: ThreadStackObjectFilter);

      List<string> commands = new List<string>();

      int totalContexts = 0;
      IDictionary<string, List<HttpContextMappingModel>> httpContextsMappedByRedisCommand = new Dictionary<string, List<HttpContextMappingModel>>();
      foreach (var contextThread in ThreadFilter.ExtactAliveUserThreads(runtime))
      {
        ExtractContextAndRedisCommand(contextThread, threadStackObjectEnumerator, clrObjectToModelFactory, httpContextsMappedByRedisCommand);
      }

      var differentRedisCommands = httpContextsMappedByRedisCommand.OrderByDescending(grouping => grouping.Value.Count);

      var aspNetIDs = (from requestGroup in httpContextsMappedByRedisCommand
                       let httpContexts = requestGroup.Value
                       from httpContext in httpContexts
                       let requestInfo = new { httpContext, httpContext?.Request?.AspNetSessionId }
                       group requestInfo by requestInfo.AspNetSessionId into sameSessions
                       select sameSessions).OrderByDescending(t => t.Count()).ToArray();

      var sb = new StringBuilder();

      sb.AppendLine($"Different Commands found: {differentRedisCommands.Count()}");

      totalContexts = httpContextsMappedByRedisCommand.Sum(elem => elem.Value.Count);

      sb.AppendLine($"Total requests {totalContexts} VS unique asp.net sessions {aspNetIDs.Length}");

      foreach (var cmd in differentRedisCommands)
      {
        sb.AppendLine($"Hits: {cmd.Value.Count}");

        sb.AppendLine();
        sb.AppendLine(cmd.Key);

        foreach (var context in cmd.Value.OrderByDescending(request => request.ExecutionDuration))
        {
          sb.AppendLine(FormatLine(context));
        }
        sb.AppendLine("**************");
      }

      foreach (var session in aspNetIDs)
      {
        sb.AppendLine($"Session: {session.Key}:");
        foreach (var request in session)
        {
          sb.AppendLine($"{request.httpContext.URL} executed for {request.httpContext.ExecutionDuration.TotalSeconds:F2} sec.");
        }
        sb.AppendLine("**************");
        sb.AppendLine();
      }
      return sb;
    }

    public virtual void ExtractContextAndRedisCommand(ClrThread thread, ThreadStackEnumerator threadStackObjectEnumerator, IModelMapperFactory factory, IDictionary<string, List<HttpContextMappingModel>> contextsByRedisCommands)
    {
      string command = null;
      HttpContextMappingModel httpContext = null;
      foreach (var clrObj in threadStackObjectEnumerator.Enumerate(thread))
      {
        ExtractCommandOrContext(clrObj, factory, ref command, ref httpContext);

        // already extracted both from the thread stack.
        if (!string.IsNullOrEmpty(command) && (httpContext != null))
        {
          break;
        }
      }

      List<HttpContextMappingModel> httpContextsForGivenRedisCommand = null;
      if (!string.IsNullOrEmpty(command))
      {
        if (!contextsByRedisCommands.ContainsKey(command))
        {
          httpContextsForGivenRedisCommand = new List<HttpContextMappingModel>();
          contextsByRedisCommands.Add(command, httpContextsForGivenRedisCommand);
        }
        else
        {
          httpContextsForGivenRedisCommand = contextsByRedisCommands[command];
        }
      }

      if (httpContext?.HasURL == true && httpContext?.Request != null)
      {
        if (httpContextsForGivenRedisCommand != null)
        {
          httpContextsForGivenRedisCommand.Add(httpContext);
        }
        else
        {
          if (!contextsByRedisCommands.ContainsKey("NoCommand"))
          {
            contextsByRedisCommands.Add("NoCommand", new List<HttpContextMappingModel>());
          }

          var noCommand = contextsByRedisCommands["NoCommand"];
          noCommand.Add(httpContext);
        }
      }
    }

    protected virtual void ExtractCommandOrContext(ClrObject clrObj, IModelMapperFactory factory, ref string command, ref HttpContextMappingModel httpContext)
    {
      if (clrObj.Type?.IsString == true)
      {
        command = clrObj.GetStringSafeFromSelf();
      }
      else
      {
        httpContext = factory.BuildModel(clrObj) as HttpContextMappingModel;
      }

    }

    private static string FormatLine(HttpContextMappingModel context)
    {
      return $"{context.URL} executed for {context.ExecutionDuration.TotalSeconds:F2} sec. AspNet session: {context.Request.AspNetSessionId}";
    }
  }
}

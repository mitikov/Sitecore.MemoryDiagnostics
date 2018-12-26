using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Diagnostics.Runtime;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.SourceFactories;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;
using Sitecore.MemoryDiagnostics.Extensions;
using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
using Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders;

namespace Sitecore.MemoryDiagnostics.API.Example
{
  public class Program
  {

    public static string MemoryDumpFile => @"C:\pass\to\mememory\snapshot\w3wp.dmp";

    public static string MsCorDacWrks => @"C:\pass\to\mememory\snapshot\mscordacwks_AMD64_AMD64_4.7.3163.00.dll";

    public static string OutputStatsFile => @"C:\pass\to\mememory\snapshot\statistics.txt";



    public static MDFileConnection Connection => new MDFileConnection(MemoryDumpFile, MsCorDacWrks);

    public static ClrRuntime Runtime => MDClrRuntimeFactory.Instance.BuildClrRuntime(Connection);

    static void Main(string[] args)
    {

      // new PrintStringBuilders().DoGrouping(MemoryDumpFile, MsCorDacWrks, typeof(StringBuilder).FullName);

      LocateHowManyPagesInProcessedSessionEnd();

      //var data = new StringBuilder();

      //var sb = new RequestsThatBeingProcessed().DoProcessing(Connection);  
      //var sb = new ExtractCustomModules().DoProcessing(Connection);


      //var all = new AllHttpWebRequest();

      // var all = new SitecoreLogsBufferContent();

      //  var data = new RequestsGroupedByAnalyticsCookie().DoProcessing(Connection);

      // File.WriteAllText(OutputStatsFile, data.ToString());


      //var sb = new MvcRenderdingsWithoutCache().DoProcessing(Connection);

      //  new DynamicAssemblyGeneration.DoGrouping(
      //  pathToMemoryDump: MemoryDumpFile,
      //  pathToMscorDac: MsCorDacWrks);

      //new JScriptDocumentContextGrouper().DoGrouping
      //  (
      //  pathToMemoryDump: MemoryDumpFile,
      //  pathToMscorDac: MsCorDacWrks);        

      //  var stringFilter = new PredicateBasedStringFilter((text) => text.StartsWith("Timeout performing EVAL"));

      //  var timeoutMessageEnumerator = new HeapBasedFacadeObjectEnumerator(stringFilter);

      //  var timeoutMessages = from obj in timeoutMessageEnumerator.ExtractFromRuntime(Runtime)
      //                        select obj.GetStringSafeFromSelf();


      //  File.WriteAllLines(OutputStatsFile, timeoutMessages);
      ///*
      // var requestsGroupedByRedisCommand = new RedisCommandsForHttpRequests()

      ///.ExtractRedisPendingCommandsForRequests(connection);

      //File.WriteAllText(OutputStatsFile, sb.ToString());

      Console.WriteLine("Done");
    }

    private static void LocateHowManyPagesInProcessedSessionEnd()
    {
      IFilteredObjectsProvider sessionArgs = new FilteredObjectProviderByTypeName("Sitecore.Pipelines.EndSession.PostSessionEndArgs");

      var factory = ModelMapperManager.NewMapperFactory;
      IMemoryDumpConnectionPath connection = Connection;

      var visitPageIndexName = StringUtil.ProduceAutoPropertyName("VisitPageIndex");

      var guidReader = new GuidReader();

      var stream = from rawSessionClrObject in sessionArgs.EnumerateObjectsFromHeap(connection)
                   let stubHttpContextForSessionEnd = rawSessionClrObject.GetRefFld("m_context")

                   let stubHttpContextModel = factory.BuildOfType<HttpContextMappingModel>(stubHttpContextForSessionEnd)

                   let rawAnalyticSessionObjects = stubHttpContextModel?.Items?["SessionSwitcher_State"] as IEnumerable<ClrObject>
                   where rawAnalyticSessionObjects != null
                   let currentRawSession = rawAnalyticSessionObjects.FirstOrDefault()
                   where !currentRawSession.IsNullObj

                   let sessionId = currentRawSession.GetStringFldOrAutoProp("Id")
                   where !string.IsNullOrEmpty(sessionId)

                   let currentVisitContext = currentRawSession.GetRefFld("_currentInteraction")
                   let visitInfo = currentVisitContext.GetRefFld("_visitData")
                   let contactId = guidReader.Read(visitInfo, "contactid")
                   let referrer = visitInfo.GetStringFldOrAutoProp("Referrer")
                   let userAgent = visitInfo.GetStringFldOrAutoProp("UserAgent")

                   let pages = factory.BuildOfType<ArrayMappingModel>(currentVisitContext.GetRefFld("_pages"))
                   where (pages?.IsEmpty == false)

                   from currentPageContext in pages.Elements
                   let rawPage = currentPageContext.Obj.GetRefFld("data")
                   let visitPageIndex = rawPage.GetInt32Fld(visitPageIndexName)
                   let url = rawPage.GetRefFldOrAutoProperty("Url").GetStringFldOrAutoProp("Path")

                   select new
                   {
                     contactId,
                     sessionEndTriggered = stubHttpContextModel.ContextCreationTime,
                     analyticsSessionId = sessionId,
                     totalPages = pages.Count,
                     rawPage = rawPage.HexAddress,
                     currentVisitContext.HexAddress,
                     visitObject = visitInfo.HexAddress,
                     userAgent,
                     referrer,
                     url,
                   };



      var sb = new StringBuilder();

      foreach (var pageStats in stream.OrderByDescending(t => t.totalPages).ThenBy(t => t.sessionEndTriggered))
      {
        sb.AppendLine("");
        sb.AppendLine(pageStats.ToString());
        sb.AppendLine("");
      }


      File.WriteAllText(OutputStatsFile, sb.ToString());

    }

  }
}

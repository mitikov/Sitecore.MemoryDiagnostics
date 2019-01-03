using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.MemoryDiagnostics.Extensions;
using Sitecore.MemoryDiagnostics.ModelFactory;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;

namespace Sitecore.MemoryDiagnostics.API.Example
{
  /// <summary>
  /// Groups <see cref="IIS7WorkerRequestModel"/> from snapshot by url, and gives stats.
  /// </summary>
  public class HttpRequestsGrouper:BaseGrouper
  {

    protected override ModelMapperFactory CreateObjectMappingFactory()
    {
      var factory = base.CreateObjectMappingFactory();

      factory.AddOrUpdate(modelType: typeof(IIS7WorkerRequestModel));

      return factory;
    }
    protected override void DoProcessing(IEnumerable<IClrObjMappingModel> clrObjectMappingModelStream)
    {
      var grouped = from iisWorkerRequest in clrObjectMappingModelStream.OfType<IIS7WorkerRequestModel>()
                    group iisWorkerRequest by iisWorkerRequest.URL into urlGrouping
                    let final = new
                    {
                      Url = urlGrouping.Key,
                      Count = urlGrouping.Count(),
                      AvgDuration = new TimeSpan((long)urlGrouping.Average(request => request.ExecutionDuration.Ticks)),
                      Earliest = urlGrouping.Min(request => request._startTime),
                      Last = urlGrouping.Max(request => request._startTime),
                      RandomAddress = urlGrouping.FirstOrDefault().HexAddress
                    }
                    orderby final.Count descending
                    select final;

      foreach (var group in grouped)
      {
        Console.WriteLine(group);
        Console.WriteLine("============");
      }

    }
  }
}

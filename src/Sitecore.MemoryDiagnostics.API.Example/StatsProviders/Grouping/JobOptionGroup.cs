using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;

namespace Sitecore.MemoryDiagnostics.API.Example.StatsProviders.Grouping
{
  public class JobOptionGroup : BaseGrouper
  {
    protected override void DoProcessing(IEnumerable<IClrObjMappingModel> clrObjectMappingModelStream)
    {
      var data = (from clrModel in clrObjectMappingModelStream
                  let jobOptionsModel = clrModel as JobOptionsModelMapping
                  where jobOptionsModel != null
                  select jobOptionsModel).Take(2);

      foreach (var t in data)
      {
        GC.KeepAlive(t);
      }
    }
  }
}

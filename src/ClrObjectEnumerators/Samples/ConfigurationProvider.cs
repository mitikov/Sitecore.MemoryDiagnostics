using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators.Samples
{
  public class ConfigurationProvider : IEnumerateClrObjectsFromClrRuntime
  {
    public IEnumerable<ClrObject> EnumerateObjectsFromSource([NotNull] ClrRuntime runtime)
    {
      Type classCarryingConfig;
      string[] fieldNames;

      if (this.IsOldSitecore(runtime))
      {
        classCarryingConfig = typeof(Configuration.Factory);

        fieldNames = new[] { "configuration" };
      }
      else
      {
        classCarryingConfig = typeof(Configuration.ConfigReader);
        fieldNames = new[] { "config" };        
      }

      var enumartor = new StaticFieldValuesEnumerator(classCarryingConfig.FullName, fieldNames);

      return enumartor.EnumerateObjectsFromSource(runtime);
    }

    protected virtual bool IsOldSitecore(ClrRuntime runtime)
    {
      var heap = runtime.GetHeap();

      var factoryType = heap.GetTypeByName(typeof(Configuration.Factory).FullName);

      // Config was moved to a new class in 8.2 at same time with DI.
      var diPlaceholder = factoryType.GetStaticFieldByName("Instance");

      return diPlaceholder == null;
    }
  }
}

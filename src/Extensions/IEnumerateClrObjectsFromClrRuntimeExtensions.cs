using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
using Sitecore.MemoryDiagnostics.ConnectionDetails;
using Sitecore.MemoryDiagnostics.SourceFactories;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.Extensions
{
  public static class IEnumerateClrObjectsFromClrRuntimeExtensions
  {

    public static IEnumerable<ClrObject> EnumerateObjects(this IEnumerateClrObjectsFromClrRuntime objectEnumerator, string pathToMemoryDump, string pathToMsCord)
    {
      return IEnumerateClrObjectsFromClrRuntimeExtensions.EnumerateObjects(objectEnumerator, new MDFileConnection(pathToMemoryDump, pathToMsCord));
    }

    public static IEnumerable<ClrObject> EnumerateObjects(this IEnumerateClrObjectsFromClrRuntime objectEnumerator, IMemoryDumpConnectionPath connectionDetails)
    {
      return IEnumerateClrObjectsFromClrRuntimeExtensions.EnumerateObjects(objectEnumerator, connectionDetails, MDClrRuntimeFactory.Instance);
    }

    public static IEnumerable<ClrObject> EnumerateObjects(this IEnumerateClrObjectsFromClrRuntime objectEnumerator, IMemoryDumpConnectionPath connectionDetails, IClrRuntimeFactory factory)
    {
      var runtime = factory.BuildClrRuntime(connectionDetails);

      return objectEnumerator.EnumerateObjectsFromSource(runtime);
    }
  }
}

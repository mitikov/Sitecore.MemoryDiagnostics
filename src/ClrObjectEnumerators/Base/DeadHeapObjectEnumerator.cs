namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators.Base
{
  using System.Collections.Generic;
  using Microsoft.Diagnostics.Runtime;
  using Attributes;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Enumerates dead objects from heap that will be collected by <see cref="System.GC"/> on next run.
  /// </summary>
  public class DeadHeapObjectEnumerator : LiveObjectEnumerator
  {
    /// <summary>
    /// Enumerates only dead objects with type from heap.
    /// </summary>
    /// <param name="runtime">The runtime to extract objects from.</param>
    /// <returns>Stream of objects that will be collected during next <see cref="System.GC"/>.</returns>
    [ClrObjAndTypeNotEmpty]
    public override IEnumerable<ClrObject> EnumerateObjectsFromSource(ClrRuntime runtime)
    {
      var heap = runtime.GetHeap();

      var aliveObjects = this.GetAliveObjects(runtime);

      foreach (var clrObject in heap.EnumerateObjectAddresses())
      {
        if (aliveObjects.Contains(clrObject))
        {
          continue;
        }

        ClrType objType = heap.GetObjectType(clrObject);

        if (objType == null)
        {
          continue;
        }

        yield return new ClrObject(clrObject, objType);
      }
    }
  }
}
namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators
{
  using System.Collections.Generic;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Diagnostics;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Yields objects from heap that have type.
  /// </summary>
  public class HeapBasedClrObjectEnumerator : IEnumerateClrObjectsFromClrRuntime
  {
    /// <summary>
    /// A single instance of state-less object.
    /// </summary>
    public readonly static HeapBasedClrObjectEnumerator Instance = new HeapBasedClrObjectEnumerator();

    [NotNull]
    public virtual IEnumerable<ClrObject> EnumerateObjectsFromSource([NotNull] ClrRuntime runtime)
    {
      Assert.ArgumentNotNull(runtime, "runtime");

      ClrHeap heap = runtime.GetHeap();

      Assert.IsNotNull(heap, "heap");
#if LimitEnumerator
      var limit = 10000;
      var current = 0;
#endif
      foreach (ulong objAddress in heap.EnumerateObjectAddresses())
      {
        var tp = heap.GetObjectType(objAddress);
        if (tp == null)
        {
          continue;
        }

#if LimitEnumerator
        ++current;
        if (current>limit)
          yield break;
#endif
        yield return new ClrObject(objAddress, tp);
      }
    }
  }
}
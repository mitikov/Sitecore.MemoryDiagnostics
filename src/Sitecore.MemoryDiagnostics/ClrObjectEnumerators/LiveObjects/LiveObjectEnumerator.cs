namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators.Base
{
  using System.Collections.Generic;
  using System.Linq;

  using Helpers;
  using LiveObjects;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.MemoryDiagnostics.Attributes;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Enumerates live <see cref="ClrObject"/>s from heap with type.
  /// </summary>
  /// <seealso cref="HeapBasedClrObjectEnumerator" />
  public class LiveObjectEnumerator : IEnumerateClrObjectsFromClrRuntime, IGetAliveObjects
  {
    /// <summary>
    /// Enumerates alive objects from source.    
    /// </summary>
    /// <param name="runtime">The datasource.</param>
    /// <returns>
    /// NonEmpty ClrObjects.
    /// </returns>
    [ClrObjAndTypeNotEmpty]
    public virtual IEnumerable<ClrObject> EnumerateObjectsFromSource(ClrRuntime runtime)
    {
      var alive = this.GetAliveObjects(runtime);
      var heap = runtime.GetHeap();
      foreach (var objAddress in alive)
      {
        ClrType objtype = heap.GetObjectType(objAddress);

        if (objtype == null)
        {
          continue;
        }

        yield return new ClrObject(objAddress, objtype);
      }
    }

    public virtual HashSet<ulong> GetAliveObjects([NotNull] ClrRuntime runtime)
    {
      var pinnedRoots = this.ExtractRootsFromRuntime(runtime);

      var heap = runtime.GetHeap();
      var uniqueObjectsToStay = HashSetHelper.GetHashSet<ulong>(10 * 1000 * 1000);

      foreach (var pinnedRoot in pinnedRoots)
      {
        var type = heap.GetObjectType(pinnedRoot);
        if (type == null)
        {
          continue;
        }

        this.EnumerateObjectReferences(pinnedRoot, uniqueObjectsToStay, heap);
      }

      uniqueObjectsToStay.TrimExcess();

      return uniqueObjectsToStay;
    }

    /// <summary>
    /// Enumerates the object references recursively.
    /// </summary>
    /// <param name="currentObjPoi">The current object poi.</param>
    /// <param name="references">The references.</param>
    /// <param name="heap">The heap.</param>
    protected virtual void EnumerateObjectReferences(ulong currentObjPoi, [NotNull] HashSet<ulong> references, [NotNull] ClrHeap heap)
    {
      // Empty pointer is passed or we have already checked the object
      if ((currentObjPoi == 0) || references.Contains(currentObjPoi))
      {
        return;
      }

      // Mark that we have processed the node
      references.Add(currentObjPoi);
      var enterCount = references.Count;

      var type = heap.GetObjectType(currentObjPoi);

      if (type == null)
      {
        return;
      }

      // Primitive types cannot have any references
      if (type.IsString || type.IsPrimitive)
      {
        return;
      }

      // c.f("Finding refs for the {0} obj, type {1}", currentObjPoi.ToString("x8"),type.Name);
      type.EnumerateRefsOfObject(
        currentObjPoi,
        delegate(ulong arg1, int i)
        {
          if (references.Contains(arg1))
          {
            return;
          }

          this.EnumerateObjectReferences(arg1, references, heap);
        });
      var found = references.Count - enterCount;
      if (found > 1000)
      {
        this.Log1kFound(found, enterCount, references.Count);
      }
    }

    protected virtual IEnumerable<ulong> ExtractRootsFromRuntime(ClrRuntime ds) => new HashSet<ulong>(from root in ds.GetHeap().EnumerateRoots(enumerateStatics: true)
      select root.Object);

    protected virtual void Log1kFound(int found, int enterCount, int count)
    {
      // think about some magic.
    }
  }
}
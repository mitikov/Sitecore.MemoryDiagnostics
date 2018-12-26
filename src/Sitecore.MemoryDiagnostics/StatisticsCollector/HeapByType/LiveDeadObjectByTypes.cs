namespace Sitecore.MemoryDiagnostics.StatisticsCollector.HeapByType
{
  using System.Collections.Generic;
  using System.Runtime.CompilerServices;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Diagnostics;

  public class LiveDeadObjectByTypes
  {
    public readonly HashSet<ulong> AliveObjectAdresses;

    /// <summary>
    ///   Key - type name, value - set of objects
    /// </summary>
    public readonly Dictionary<string, HashSet<ulong>> AliveObjectsByType;

    public readonly HeapTypeStatisticsCollection AllObjStatByType;
    public readonly HeapTypeStatisticsCollection LiveObjStatsByType;

    protected readonly ClrRuntime Runtime;

    public LiveDeadObjectByTypes([NotNull] ClrRuntime runtime)
    {
      Assert.ArgumentNotNull(runtime, "runtime");

      this.Runtime = runtime;

      this.LiveObjStatsByType = new HeapTypeStatisticsCollection();

      this.AllObjStatByType = new HeapTypeStatisticsCollection();

      this.AliveObjectsByType = new Dictionary<string, HashSet<ulong>>(10000);

      this.AliveObjectAdresses = new HashSet<ulong>();
    }

    public static void EnumerateObjectsInHeapByType(
      [NotNull] ClrRuntime rn,
      out HeapTypeStatisticsCollection allObjStatByType,
      out HeapTypeStatisticsCollection liveObjStatByType)
    {
      var instance = new LiveDeadObjectByTypes(rn);

      instance.CalculateHeapStatsByType();

      allObjStatByType = instance.AllObjStatByType;

      liveObjStatByType = instance.LiveObjStatsByType;
    }

    public static void GetAliveObjectAddresses([NotNull] ClrRuntime rn, out HashSet<ulong> aliveObjectAdresses,
      out Dictionary<string, HashSet<ulong>> aliveObjectsByType)
    {
      var instance = new LiveDeadObjectByTypes(rn);
      instance.FindAliveObjects();
      aliveObjectAdresses = instance.AliveObjectAdresses;
      aliveObjectsByType = instance.AliveObjectsByType;
    }

    public void CalculateHeapStatsByType()
    {
      this.FindAliveObjects();

      ClrHeap heap = this.Runtime.GetHeap();

      foreach (ulong objRef in heap.EnumerateObjectAddresses())
      {
        ClrType tp = heap.GetObjectType(objRef);
        if (tp == null)
        {
          continue;
        }

        this.AddUsage(this.AllObjStatByType, objRef, tp);

        if (AliveObjectAdresses.Contains(objRef))
        {
          this.AddUsage(this.LiveObjStatsByType, objRef, tp);
        }
      }
    }

    protected void FindAliveObjects()
    {
      ClrHeap heap = Runtime.GetHeap();

      this.FindAliveObjects(heap);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddUsage(HeapTypeStatisticsCollection collection, HeapTypeStatisticsEntry candidate)
    {
      Assert.ArgumentNotNull(collection, "collection");
      Assert.ArgumentNotNull(candidate, "candidate");
      collection.AddOrMerge(candidate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddUsage(HeapTypeStatisticsCollection collection, ulong objectReference, ClrType tp)
    {
      Assert.ArgumentNotNull(collection, "collection");
      Assert.ArgumentNotNull(tp, "type");
      collection.Add(objectReference, tp);
    }

    /// <summary>
    ///   Gets all object references
    /// </summary>
    /// <param name="currentObjPoi">Objects whose references are checked</param>
    /// <param name="visitedObjects">Already checked objects</param>
    /// <param name="objCountByType">Keeping track of type distribution</param>
    /// <param name="heap"></param>
    private void EnumerateRootReferences(
      ulong currentObjPoi, 
      HashSet<ulong> visitedObjects,
      IDictionary<string, HashSet<ulong>> objCountByType,
      ClrHeap heap)
    {
      // Empty pointer is passed or we have already checked the object
      if ((currentObjPoi == 0) || visitedObjects.Contains(currentObjPoi))
      {
        return;
      }

      // Mark that we have processed the node
      visitedObjects.Add(currentObjPoi);

      // Get object type
      ClrType type = heap.GetObjectType(currentObjPoi);

      if (type == null)
      {
        return; // Cannot do anything
      }

      if (objCountByType.ContainsKey(type.Name))
      {
        objCountByType[type.Name].Add(currentObjPoi);
      }
      else
      {
        objCountByType[type.Name] = new HashSet<ulong>
        {
          currentObjPoi
        };
      }

      type.EnumerateRefsOfObject(currentObjPoi, delegate (ulong arg1, int i)
      {
        if (visitedObjects.Contains(arg1))
        {
          return;
        }

        this.EnumerateRootReferences(arg1, visitedObjects, objCountByType, heap);
      });
    }

    private void FindAliveObjects([CanBeNull] ClrHeap heap)
    {
      if (heap == null)
      {
        return;
      }

      foreach (ClrRoot clrRoot in heap.EnumerateRoots())
      {
        this.EnumerateRootReferences(clrRoot.Object, this.AliveObjectAdresses, this.AliveObjectsByType, heap);
      }
    }
  }
}
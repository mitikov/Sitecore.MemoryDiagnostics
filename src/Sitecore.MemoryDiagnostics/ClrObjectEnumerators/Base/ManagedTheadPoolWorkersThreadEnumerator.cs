namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore.Diagnostics;  
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Enumerates thread pool workers.
  /// </summary>
  public class ManagedTheadPoolWorkersThreadEnumerator : HeapBasedClrObjectEnumerator
  {
    /// <summary>
    /// The instance
    /// </summary>
    public static ManagedTheadPoolWorkersThreadEnumerator Instance = new ManagedTheadPoolWorkersThreadEnumerator();

    protected virtual string ThreadTypeName => @"System.Threading.Thread";

    /// <summary>
    /// Gets the <see cref="ClrThread"/> by managed ThreadID specified in <paramref name="threadObj"/>.
    /// </summary>
    /// <param name="threadObj">The thread object.</param>
    /// <returns></returns>
    public static ClrThread GetByAddress([ClrObjAndTypeNotEmpty] ClrObject threadObj)
    {
      ClrAssert.ObjectNotNullTypeNotEmpty(threadObj);
      var tp = threadObj.Type;
      var id = threadObj.GetInt32Fld("m_ManagedThreadId");
      return tp.Heap.Runtime.Threads.FirstOrDefault(t => t.ManagedThreadId == id);
    }

    /// <summary>
    /// Enumerates the objects from source.
    /// </summary>
    /// <param name="runtime">The runtime to extract data from.</param>
    /// <returns>
    /// NonEmpty ClrObjects.
    /// </returns>
    [NotNull]
    public override IEnumerable<ClrObject> EnumerateObjectsFromSource([NotNull] ClrRuntime runtime)
    {
      Assert.ArgumentNotNull(runtime, "runtime");

      var heap = runtime.GetHeap();

      var tp = heap.GetTypeByName(this.ThreadTypeName);

      var fld = tp.GetFieldByName("DONT_USE_InternalThread");

      foreach (var thread in runtime.Threads)
      {
        if (!thread.IsThreadpoolWorker || (thread.StackTrace == null))
        {
          continue;
        }

        var breakInnerLoopFlag = false;
        foreach (var stackObj in thread.EnumerateStackObjects(true))
        {
          if (breakInnerLoopFlag)
          {
            break;
          }

          var stackObjType = heap.GetObjectType(stackObj.Object);

          if ((stackObjType == null) || (stackObjType.MetadataToken != tp.MetadataToken))
          {
            continue;
          }

          var reference = (ulong)(long)fld.GetValue(stackObj.Object);
          if (thread.Address == reference)
          {
            // Only one thread object can correspond to Clr Thread
            breakInnerLoopFlag = true;
            yield return new ClrObject(stackObj.Object, tp);
          }
        }
      }
    }
  }
}
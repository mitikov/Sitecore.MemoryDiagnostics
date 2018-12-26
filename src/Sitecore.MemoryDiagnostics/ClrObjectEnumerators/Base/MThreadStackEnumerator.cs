namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators
{
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Diagnostics;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Yields unique objects from ThreadPoolWorker Thread stacks that have type.
  /// </summary>
  public class MThreadStackEnumerator : ManagedTheadPoolWorkersThreadEnumerator
  {
    // TODO: implement property to avoid null value being assigned
    public static readonly MThreadStackEnumerator Instance = new MThreadStackEnumerator();

    /// <summary>
    /// The include possibly dead.
    /// </summary>
    protected bool IncludePossiblyDead;

    /// <summary>
    /// Initializes a new instance of the <see cref="MThreadStackEnumerator"/> class.
    /// </summary>
    /// <param name="includePossiblyDeadClrObjects">if set to <c>true</c> [include possibly dead color objects].</param>
    public MThreadStackEnumerator(bool includePossiblyDeadClrObjects = true)
    {
      this.IncludePossiblyDead = includePossiblyDeadClrObjects;
    }

    /// <summary>
    /// Enumerates the objects from source.
    /// </summary>
    /// <param name="runtime">The datasource.</param>
    /// <returns>
    /// NonEmpty ClrObjects.
    /// </returns>
    [NotNull]
    public override IEnumerable<ClrObject> EnumerateObjectsFromSource([NotNull] ClrRuntime runtime)
    {
      Assert.ArgumentNotNull(runtime, "runtime");

      var yielded = new HashSet<ulong>();

      ClrHeap heap = runtime.GetHeap();

      foreach (var threadObj in base.EnumerateObjectsFromSource(runtime))
      {
        
        var thread = runtime.Threads.FirstOrDefault(t => t.Address == threadObj.Address);
        if (thread == null)
        {
          continue;
        }

        foreach (ClrRoot root in thread.EnumerateStackObjects(this.IncludePossiblyDead))
        {
          var clrObj = new ClrObject(root.Object, heap);
          clrObj.ReEvaluateType();

          if (clrObj.IsNullObj || (clrObj.Type == null))
          {
            continue;
          }

          if (yielded.Contains(clrObj.Address))
          {
            continue;
          }

          yielded.Add(clrObj.Address);

          yield return clrObj;
        }
      }
    }    
  }
}
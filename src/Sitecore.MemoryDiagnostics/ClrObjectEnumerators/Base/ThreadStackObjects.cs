using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using Sitecore.MemoryDiagnostics.Helpers;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators.Base
{
  /// <summary>
  /// Returns objects from all thread stacks.
  /// </summary>
  public class ThreadStackObjects : IEnumerateClrObjectsFromClrRuntime
  {

    protected readonly UserThreadsFilter ThreadFilterHelper;

    public ThreadStackObjects(): this(new UserThreadsFilter())
    {

    }

    public ThreadStackObjects(UserThreadsFilter threadFilterHelper)
    {
      this.ThreadFilterHelper = threadFilterHelper;
    }

    public IEnumerable<ClrObject> EnumerateObjectsFromSource([NotNull] ClrRuntime runtime)
    {
      var threads = runtime.Threads;
      var heap = runtime.GetHeap();
      var hashSet = new HashSet<ulong>();
      foreach (var thread in threads)
      {
        if (this.ThreadFilterHelper.ShouldSkip(thread))
        {
          continue;
        }

        foreach(var obj in thread.EnumerateStackObjects(includePossiblyDead: true))
        {
          var type = heap.GetObjectType(obj.Object);
          if (type == null)
            continue;

          if (hashSet.Add(obj.Object))
          {
            yield return new ClrObject(obj.Object, type);
          }
        }
      }
    }
  }
}

namespace Sitecore.MemoryDiagnostics.ThreadStackEnumerators
{
  using System.Collections.Generic;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class ThreadStackEnumerator
  {
    #region fields
    protected readonly bool IncludePossiblyDead;

    protected readonly FilteredObjectsProviderBase FilteredObjectsProvider;

    #endregion
    #region Constructors
    public ThreadStackEnumerator() : this(includePossiblyDead: true, filter: new NullFilterClrObjectProvider())
    {

    }

    public ThreadStackEnumerator(bool includePossiblyDead, FilteredObjectsProviderBase filter)
    {
      FilteredObjectsProvider = filter;
      this.IncludePossiblyDead = includePossiblyDead;
    }
    #endregion

    /// <summary>
    /// Enumerates objects that are either located in thread stack (f.e. structures), or mentioned there by pointer (reference types). 
    /// </summary>
    /// <param name="thread">A thread to have stack inspected.</param>
    /// <returns>A stream of non-duplicated objects that match criterias.</returns>
    public virtual IEnumerable<ClrObject> Enumerate(ClrThread thread)
    {
      if (thread == null)
      {
        yield break;
      }

      var clrRoots = thread.EnumerateStackObjects(this.IncludePossiblyDead);
      var processed = new HashSet<ulong>();

      var heap = thread.Runtime.GetHeap();

      foreach (var root in clrRoots)
      {
        var clrObject = new ClrObject(root.Object, heap);

        if (processed.Contains(clrObject.Address))
        {
          continue;
        }

        if (this.ShouldSkip(clrObject, thread))
        {
          continue;
        }

        if (processed.Add(clrObject.Address))
        {
          yield return clrObject;
        }
      }
    }

    protected virtual bool ShouldSkip(ClrObject clrObject, ClrThread thread)
    {
      return !FilteredObjectsProvider.MatchesExtractCriteria(clrObject);
    }
  }
}

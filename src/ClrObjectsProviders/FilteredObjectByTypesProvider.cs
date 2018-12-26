namespace Sitecore.MemoryDiagnostics.ClrObjectsProviders
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;
  using System.Linq;
  using System;

  public class FilteredObjectByTypesProvider : FilteredObjectsProviderBase
  {
    #region Fields

    private List<ClrType> enumeratedTypes;

    private bool initialized = false;

    #endregion

    #region Constructors  
    public FilteredObjectByTypesProvider(params string[] typeNames) : this((IEnumerable<string>)typeNames)
    {
    }

    public FilteredObjectByTypesProvider([NotNull] IEnumerable<string> typeNames)
    {
      var elems = typeNames.ToArray();

      Array.Sort(elems);
      this.EnumeratedTypesNames =  new System.ArraySegment<string>(elems);
    }

    #endregion

    #region Properties
    [NotNull]
    protected IReadOnlyList<ClrType> EnumeratedTypes => enumeratedTypes;

    [NotNull]
    protected IReadOnlyList<string> EnumeratedTypesNames { get; private set; }

    #endregion
    protected override IEnumerable<ClrObject> ExtractObjectsFromRuntime(ClrRuntime ds, IEnumerateClrObjectsFromClrRuntime clrClrObjectEnumerator)
    {
      this.EnsureInitialized(ds);

      return base.ExtractObjectsFromRuntime(ds, clrClrObjectEnumerator);
    }

    public override bool MatchesExtractCriteria(ClrObject clrObj)
    {
      if (clrObj.Type == null)
      {
        return false;
      }

      if (!initialized)
      {
        this.EnsureInitialized(clrObj.Type.Heap.Runtime);
      }

      return (clrObj.Type != null) && this.EnumeratedTypes.Contains(clrObj.Type);
    }

    private void EnsureInitialized([NotNull] ClrRuntime runtime)
    {
      if (initialized)
      {
        return;
      }

      try
      {
        ClrHeap heap = runtime.GetHeap();
        enumeratedTypes = new List<ClrType>();
        foreach (string typename in EnumeratedTypesNames)
        {
          if (string.IsNullOrEmpty(typename))
          {
            continue;
          }

          ClrType tmp = heap.GetTypeByName(typename);
          if (tmp == null)
          {
            Trace.TraceWarning("{0} type was not found in runtime.", typename);
            continue;
          }

          enumeratedTypes.Add(tmp);
        }
      }
      finally
      {
        initialized = true;
      }
    }
  }
}
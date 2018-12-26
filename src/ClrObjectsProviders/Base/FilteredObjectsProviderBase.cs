namespace Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Diagnostics;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;
  using Sitecore.MemoryDiagnostics.Interfaces;
  using System;

  /// <summary>
  /// Providers <see cref="ClrObject"/> enumeration from provided <see cref="ClrRuntime"/>.
  /// <para>Must  <see cref="ClrObject"/> objects yielded by underlying <see cref="IEnumerateClrObjectsFromClrRuntime"/> based on inspected object fields or metadata.</para>  
  /// <para><example>Filter objects by type. <see cref="FilteredObjectProviderByTypeName"/></example></para>
  /// </summary>
  public abstract class FilteredObjectsProviderBase : IFilteredObjectsProvider, IFilter<ClrObject>
  {
    /// <summary>
    /// Extracts <see cref="ClrObject" /> from <see cref="ClrRuntime" /> using underlying <see cref="IEnumerateClrObjectsFromClrRuntime" />.
    /// <para>Current instance should add extra conditions to be checked (f.e. class name )</para>
    /// </summary>
    /// <param name="runtime">The clrRuntime.</param>
    /// <param name="clrObjectEnumerator">The object enumerator.</param>
    /// <returns>
    /// Enumerations of <see cref="ClrObject" /> that match custom logic.
    /// </returns>
    [NotNull]
    public IEnumerable<ClrObject> ExtractFromRuntime([CanBeNull] ClrRuntime runtime, [NotNull] IEnumerateClrObjectsFromClrRuntime clrObjectEnumerator)
    {
      if (runtime == null)
      {
        return new ClrObject[0];
      }

      IEnumerable<ClrObject> result = this.ExtractObjectsFromRuntime(runtime, clrObjectEnumerator);

      return result ?? new ClrObject[0];
    }

    /// <summary>
    /// Applies <see cref="MatchesExtractCriteria"/> API on every object yielded by <see cref="IEnumerateClrObjectsFromClrRuntime"/>
    /// </summary>
    /// <param name="ds">The ds.</param>
    /// <param name="clrClrObjectEnumerator">The ClrObject enumerator.</param>
    /// <returns>Enumeration of objects that match specified criteria.</returns>
    [CanBeNull]
    protected virtual IEnumerable<ClrObject> ExtractObjectsFromRuntime([NotNull] ClrRuntime ds, [NotNull] IEnumerateClrObjectsFromClrRuntime clrClrObjectEnumerator)
    {
      Assert.ArgumentNotNull(ds, "ClrRuntime");
      Assert.ArgumentNotNull(clrClrObjectEnumerator, "ClrObject enumerator must be set before enumeration begins.");

      return clrClrObjectEnumerator.EnumerateObjectsFromSource(ds).Where(this.MatchesExtractCriteria);
    }

    /// <summary>
    /// Checks object matches extract criteria.
    /// </summary>
    /// <param name="clrObj">The color object.</param>
    /// <returns></returns>
    public abstract bool MatchesExtractCriteria([CanBeNullObject] ClrObject clrObj);

    bool IFilter<ClrObject>.Matches(ClrObject tested)
    {
      return this.MatchesExtractCriteria(tested);
    }
  }
}
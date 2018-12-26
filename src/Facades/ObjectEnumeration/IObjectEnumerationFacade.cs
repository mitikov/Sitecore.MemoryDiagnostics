namespace Sitecore.MemoryDiagnostics.Facades.ObjectEnumeration
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Allows to enumerate <see cref="ClrObject"/>s from <see cref="ClrRuntime"/>.
  /// <para>Provides <see cref="IEnumerateClrObjectsFromClrRuntime"/> and <see cref="IFilteredObjectsProvider"/> pair.</para>
  /// <para><see cref="IEnumerateClrObjectsFromClrRuntime"/> yields objects from runtime (f.e. from heap, Finalize Queue)</para> 
  /// <para><see cref="IFilteredObjectsProvider"/> performs filtering on these objects (f.e. based on type, or GC generation).</para>
  /// </summary>
  public interface IObjectEnumerationFacade
  {
    /// <summary>
    /// Gets the <see cref="ClrObject"/> enumerator.
    /// </summary>
    /// <value>
    /// The <see cref="ClrObject"/> enumerator.
    /// </value>
    [NotNull]
    IEnumerateClrObjectsFromClrRuntime ClrObjectEnumerator { get; }

    /// <summary>
    /// Gets the filtered objects provider.
    /// </summary>
    /// <value>
    /// The filtered objects provider.
    /// </value>
    [NotNull]
    IFilteredObjectsProvider FilteredObjectsProvider { get; }

    /// <summary>
    /// Extracts from runtime.
    /// </summary>
    /// <param name="ds">The ds.</param>
    /// <returns>A <see cref="ClrObjectEnumerator"/> sequence of <see cref="ClrObject"/> matching <see cref="FilteredObjectsProvider"/> criteria.</returns>
    IEnumerable<ClrObject> ExtractFromRuntime([CanBeNull] ClrRuntime ds);
  }
}
namespace Sitecore.MemoryDiagnostics.Facades.ObjectEnumeration
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore.Diagnostics;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;
  using System.Diagnostics.Contracts;

  // TODO: Think if this can be simplified.

  /// <summary>
  /// Allows to enumerate <see cref="ClrObject"/>s from provided <see cref="ClrRuntime"/>.
  /// <para>Combination of <see cref="IEnumerateClrObjectsFromClrRuntime"/> and <see cref="IFilteredObjectsProvider"/> pair.</para>
  /// <para><see cref="IEnumerateClrObjectsFromClrRuntime"/> yields objects from runtime (f.e. from heap, thread stacks, and so on)</para> 
  /// <para><see cref="IFilteredObjectsProvider"/> performs filtering on these objects (f.e. based on type, or GC generation).</para>
  /// </summary>
  /// <seealso cref="Facades.ObjectEnumeration.IObjectEnumerationFacade" />
  public class DefaultObjectEnumerationFacade : IObjectEnumerationFacade
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultObjectEnumerationFacade"/> class.
    /// </summary>
    /// <param name="clrObjectEnumerator">The color object enumerator.</param>
    /// <param name="filteredObjectsProvider">The filtered objects provider.</param>
    public DefaultObjectEnumerationFacade(IEnumerateClrObjectsFromClrRuntime clrObjectEnumerator, IFilteredObjectsProvider filteredObjectsProvider)
    {
      Contract.Requires(clrObjectEnumerator != null, nameof(clrObjectEnumerator));
      Contract.Requires(filteredObjectsProvider != null, nameof(filteredObjectsProvider));
      this.ClrObjectEnumerator = clrObjectEnumerator;
      this.FilteredObjectsProvider = filteredObjectsProvider;
    }

    /// <summary>
    /// Gets the color object enumerator.
    /// </summary>
    /// <value>
    /// The color object enumerator.
    /// </value>
    public virtual IEnumerateClrObjectsFromClrRuntime ClrObjectEnumerator { get; }

    /// <summary>
    /// Gets the filtered objects provider.
    /// </summary>
    /// <value>
    /// The filtered objects provider.
    /// </value>
    public virtual IFilteredObjectsProvider FilteredObjectsProvider { get; }

    /// <summary>
    /// Extracts objects from runtime that match conditions.
    /// </summary>
    /// <param name="ds">The ds.</param>
    /// <returns>A sequence of elements that match specified conditions.</returns>
    public IEnumerable<ClrObject> ExtractFromRuntime(ClrRuntime ds)
    {
      Assert.Required(this.ClrObjectEnumerator, nameof(this.ClrObjectEnumerator));

      Assert.Required(this.FilteredObjectsProvider, nameof(this.FilteredObjectsProvider));

      foreach (var matchingObject in this.FilteredObjectsProvider.ExtractFromRuntime(ds, this.ClrObjectEnumerator))
      {
        yield return matchingObject;
      }
    }

  }
}
namespace Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
  using Microsoft.Diagnostics.Runtime;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Providers <see cref="ClrObject"/> enumeration from provided <see cref="ClrRuntime"/>.
  /// <para>Must filter or extend <see cref="ClrObject"/> yielded by underlying <see cref="IEnumerateClrObjectsFromClrRuntime"/></para>
  /// <para>First filter that is applied. Must filter <see cref="ClrObject"/> based on inspected object fields or metadata.</para>
  /// <para><example>Filter objects by type. <see cref="FilteredObjectProviderByTypeName"/></example></para>
  /// </summary>
  public interface IFilteredObjectsProvider
  {
    /// <summary>
    /// Extracts <see cref="ClrObject" /> from <see cref="ClrRuntime" /> using underlying <see cref="IEnumerateClrObjectsFromClrRuntime" />.
    /// <para><see cref="IEnumerateClrObjectsFromClrRuntime" /> is supposed to enumerate all objects with types in f.e. Heap, or Thread Stacks</para><para>Whereas current instance should add extra conditions to be checked (f.e. class name )</para>
    /// </summary>
    /// <param name="ds">The ds.</param>
    /// <param name="clrObjectEnumerator">The  object enumerator.</param>
    /// <returns>
    /// A filtered sequence of <see cref="ClrObject" /> that match criteria specified by the instance.
    /// </returns>
    [NotNull]
    IEnumerable<ClrObject> ExtractFromRuntime([CanBeNull] ClrRuntime ds, [NotNull] IEnumerateClrObjectsFromClrRuntime clrObjectEnumerator);
  }
}
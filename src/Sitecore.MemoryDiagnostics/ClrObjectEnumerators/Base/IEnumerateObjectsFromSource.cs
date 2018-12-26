namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
  using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
  using Microsoft.Diagnostics.Runtime;  
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Yields all <see cref="ClrObject"/> with type from <see cref="ClrRuntime"/>, whereas <see cref="IFilteredObjectsProvider"/> must perform data filtering.
  /// <example>
  /// <para><seealso cref="HeapBasedClrObjectEnumerator"/>, <seealso cref="MThreadStackEnumerator"/>, or static field value parsed via <see cref="IModelMapperFactory"/> and treated as <see cref="IEnumerable{T}"/> of <see cref="ClrObject"/>.</para>
  /// </example>
  /// </summary>
  public interface IEnumerateClrObjectsFromClrRuntime
  {
    /// <summary>
    /// Enumerates the objects from source.
    /// <para>F.e. all Heap objects, Thread, or custom array enumeration. </para>
    /// </summary>
    /// <param name="runtime">The datasource.</param>
    /// <returns>NonEmpty ClrObjects. </returns>
    [NotNull]
    [ClrObjAndTypeNotEmpty]
    IEnumerable<ClrObject> EnumerateObjectsFromSource([NotNull] ClrRuntime runtime);
  }
}
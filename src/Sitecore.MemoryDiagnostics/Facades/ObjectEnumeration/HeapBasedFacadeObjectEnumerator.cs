namespace Sitecore.MemoryDiagnostics.Facades.ObjectEnumeration
{
  using System;
  using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders;
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;

  /// <summary>
  /// Enumerates objects from heap filtered by <see cref="IFilteredObjectsProvider"/>. 
  /// </summary>
  /// <seealso cref="Facades.ObjectEnumeration.DefaultObjectEnumerationFacade" />
  public class HeapBasedFacadeObjectEnumerator : DefaultObjectEnumerationFacade
  {
    public HeapBasedFacadeObjectEnumerator(Type type): this(type.FullName)
    {
    }

    public HeapBasedFacadeObjectEnumerator(string typeName) : this(new FilteredObjectProviderByTypeName(typeName))
    {
    }
     /// <summary>
     /// Initializes a new instance of heap-based object enumerator with specific object filter.
     /// </summary>
     /// <param name="filteredObjectsProvider"></param>
    public HeapBasedFacadeObjectEnumerator(IFilteredObjectsProvider filteredObjectsProvider) : base(HeapBasedClrObjectEnumerator.Instance, filteredObjectsProvider)
    {
    }
  }
}
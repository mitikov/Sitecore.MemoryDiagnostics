namespace Sitecore.MemoryDiagnostics.Facades.ObjectEnumeration
{
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders;

  /// <summary>
  /// Enumerates all objects from heap (without filtering) with type.
  /// </summary>
  /// <seealso cref="HeapBasedFacadeObjectEnumerator" />
  public class AllHeapObjects : HeapBasedFacadeObjectEnumerator
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AllHeapObjects"/> class.
    /// </summary>
    public AllHeapObjects() : base(NullFilterClrObjectProvider.Instance)
    {
    }
  }
}
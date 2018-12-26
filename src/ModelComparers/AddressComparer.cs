namespace Sitecore.MemoryDiagnostics.ModelComparers
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore;

  /// <summary>
  /// Compares models by address in memory.
  /// </summary>
  /// <seealso cref="System.Collections.Generic.IComparer{ClrObjectMappingModel}" />
  public class AddressComparer : IComparer<ClrObjectMappingModel>
  {
    public int Compare([CanBeNull] ClrObjectMappingModel x, [CanBeNull] ClrObjectMappingModel y)
    {
      if ((x == null) && (y == null))
      {
        return 0;
      }

      if (x == null)
      {
        return -1;
      }
      if (y == null)
      {
        return 1;
      }
      return x.Obj.Address.CompareTo(y.Obj.Address);
    }
  }
}
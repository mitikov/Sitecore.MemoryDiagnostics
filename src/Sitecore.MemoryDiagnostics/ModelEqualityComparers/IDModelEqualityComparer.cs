namespace Sitecore.MemoryDiagnostics.ModelEqualityComparers
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;
  using Sitecore.Data;

  /// <summary>
  /// Compares instances of <see cref="IDModel"/> by <see cref="ID"/> equality.
  /// </summary>
  public class IDModelEqualityComparer : IEqualityComparer<IDModel>, IEqualityComparer<IClrObjMappingModel>
  {
    public bool Equals(IDModel x, IDModel y)
    {
      if (ReferenceEquals(x, y))
        return true;
      if ((x == null) || (y == null))
        return false;
      return x.Id == y.Id;
    }

    public bool Equals(IClrObjMappingModel x, IClrObjMappingModel y)
    {
      return Equals(x as IDModel, y as IDModel);
    }

    public int GetHashCode(IDModel obj)
    {
      if ((obj == null) || ID.IsNullOrEmpty(obj.Id))
        return 0;
      return obj.Id.GetHashCode();
    }

    public int GetHashCode(IClrObjMappingModel obj)
    {
      return this.GetHashCode(obj as IDModel);
    }
  }
}
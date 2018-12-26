namespace Sitecore.MemoryDiagnostics.ModelEqualityComparers
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack;

  public class RegexEqComparer : IEqualityComparer<RegexMappingModel>, IEqualityComparer<IClrObjMappingModel>
  {
    public bool Equals(RegexMappingModel x, RegexMappingModel y)
    {
      if (ReferenceEquals(x, y))
        return true;
      if ((x == null) || (y == null))
        return false;
      return string.Equals(x.Pattern, y.Pattern);
    }

    public bool Equals(IClrObjMappingModel x, IClrObjMappingModel y)
    {
      return Equals(x as RegexMappingModel, y as RegexMappingModel);
    }

    public int GetHashCode(RegexMappingModel obj)
    {
      return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IClrObjMappingModel obj)
    {
      return this.GetHashCode(obj as RegexMappingModel);
    }
  }
}
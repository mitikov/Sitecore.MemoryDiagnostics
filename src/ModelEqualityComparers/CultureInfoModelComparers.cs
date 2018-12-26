namespace Sitecore.MemoryDiagnostics.ModelEqualityComparers
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;

  public class CultureInfoModelComparers : IEqualityComparer<CultureInfoMapping>, IEqualityComparer<IClrObjMappingModel>
  {
    public bool Equals(CultureInfoMapping x, CultureInfoMapping y)
    {
      if (ReferenceEquals(x, y))
        return true;
      if ((x == null) || (y == null))
        return false;
      return (x.cultureID == y.cultureID) && string.Equals(x.m_name, y.m_name);
    }

    public bool Equals(IClrObjMappingModel x, IClrObjMappingModel y)
    {
      return Equals(x as CultureInfoMapping, y as CultureInfoMapping);
    }

    public int GetHashCode(CultureInfoMapping obj)
    {
      if (obj == null)
        return 0;
      return obj.cultureID << (5 + (obj.m_name).FallbackTo(string.Empty).GetHashCode());
    }

    public int GetHashCode(IClrObjMappingModel obj)
    {
      return GetHashCode(obj as CultureInfoMapping);
    }
  }
}
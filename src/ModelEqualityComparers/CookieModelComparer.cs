namespace Sitecore.MemoryDiagnostics.ModelEqualityComparers
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;

  /// <summary>
  /// Compares <see cref="HttpCookieModel"/> by name, and value.
  /// <para>Two <see cref="HttpCookieModel"/> are considered to be equal when have same name, and value.</para>
  /// </summary>
  public class CookieModelComparer : IEqualityComparer<HttpCookieModel>, IEqualityComparer<IClrObjMappingModel>
  {
    public bool Equals(HttpCookieModel left, HttpCookieModel right)
    {
      if (ReferenceEquals(left, right))
      {
        return true;
      }

      if (((left == null) || (right == null))
          || !string.Equals(left._name, right._name)
          || !string.Equals(left.Value, right.Value))
      {
        return false;
      }

      return true;
    }

    public int GetHashCode(HttpCookieModel obj)
    {
      if ((obj == null) || string.IsNullOrEmpty(obj.Name))
      {
        return 0;
      }
      else
      {
        return obj.Name.GetHashCode();
      }
    }

    bool IEqualityComparer<IClrObjMappingModel>.Equals(IClrObjMappingModel x, IClrObjMappingModel y)
    {
      return Equals(x as HttpCookieModel, y as HttpCookieModel);
    }

    int IEqualityComparer<IClrObjMappingModel>.GetHashCode(IClrObjMappingModel x)
    {
      return this.GetHashCode(x as HttpCookieModel);
    }
  }
}
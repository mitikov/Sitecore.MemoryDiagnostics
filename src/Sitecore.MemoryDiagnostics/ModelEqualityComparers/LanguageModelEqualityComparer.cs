namespace Sitecore.MemoryDiagnostics.ModelEqualityComparers
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;
  using Sitecore.MemoryDiagnostics.Utils;

  public class LanguageModelEqualityComparer : IEqualityComparer<LanguageModel>, IEqualityComparer<IClrObjMappingModel>
  {
    public static readonly LanguageModelEqualityComparer Instance = new LanguageModelEqualityComparer();


    protected LanguageModelEqualityComparer()
    {
    }

    public bool Equals(LanguageModel x, LanguageModel y)
    {
      if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
        return true;
      if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
        return false;
      return x.Equals(y);
    }

    public bool Equals(IClrObjMappingModel x, IClrObjMappingModel y)
    {
      return Equals(x as LanguageModel, y as LanguageModel);
    }

    public int GetHashCode(LanguageModel obj)
    {
      if (obj == null)
        return 0;
      return obj.HasName ? this.ComputeHashCodeForCulture(obj._name) : obj.GetHashCode();
    }

    public int GetHashCode(IClrObjMappingModel obj)
    {
      if (obj is LanguageModel)
        return this.GetHashCode((LanguageModel)obj);
      return this.GetHashCode();
    }

    protected int ComputeHashCodeForCulture(string name)
    {
      return CultureInfoUtils.FastHash(name);
    }
  }
}
namespace Sitecore.MemoryDiagnostics.ModelEqualityComparers
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;
  using Sitecore;

  /// <summary>
  /// Ought be used to find duplicate models in memory - objects have exactly same set of fields, but many instances are produced, thereby increasing pressure for GC.
  /// </summary>
  public class VersionUriModelComparer : IEqualityComparer<VersionUriModel>, IEqualityComparer<IClrObjMappingModel>
  {
    /// <summary>
    /// Singleton of <see cref="VersionUriModelComparer"/>
    /// </summary>
    public static readonly VersionUriModelComparer Instance = new VersionUriModelComparer();

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionUriModelComparer"/> class.
    /// Access modifier to enable unit-tests.
    /// </summary>
    protected VersionUriModelComparer()
    {
    }

    public bool Equals(VersionUriModel x, VersionUriModel y)
    {
      if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
        return true;
      if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
        return false;
      return x == y;
    }

    public bool Equals(IClrObjMappingModel x, IClrObjMappingModel y)
    {
      return this.Equals(x as VersionUriModel, y as VersionUriModel);
    }

    public int GetHashCode([CanBeNull] VersionUriModel obj)
    {
      if (obj == null)
        return 0;
      return ((obj.m_language != null) ? obj.m_language.CultureHashCode : 0) | (obj.m_version != null ? obj.m_version.ShiftedHashCode : 0);
    }

    public int GetHashCode(IClrObjMappingModel obj)
    {
      if (obj is VersionUriModel)
      {
        return this.GetHashCode(obj as VersionUriModel);
      }

      return obj.GetHashCode();
    }
  }
}
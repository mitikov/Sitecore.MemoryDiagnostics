namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System;
  using System.Diagnostics;

  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Data;

  [DebuggerDisplay("{Caption}")]
  [ModelMapping(typeof(VersionUri))]
  public class VersionUriModel : ClrObjectMappingModel, IEquatable<VersionUriModel>, IEquatable<VersionUri>
  {
    [InjectFieldValue]
    public LanguageModel m_language;

    [InjectFieldValue]
    public VersionModel m_version;

    public override string Caption
    {
      get
      {
        return ((m_language == null) ? "[NoLang]" : m_language.ToString()) + "¤" + ((m_version == null) ? "[NoVersion]" : m_version.ToString());
      }
    }

    public static bool operator ==(VersionUriModel uno, VersionUriModel duo)
    {
      if (ReferenceEquals(uno, null) && ReferenceEquals(duo, null))
      {
        return true;
      }
      if (ReferenceEquals(uno, null) || ReferenceEquals(duo, null))
      {
        return false;
      }
      return uno.Equals(duo);
    }

    public static bool operator !=(VersionUriModel uno, VersionUriModel duo)
    {
      return !(uno == duo);
    }

    public bool Equals(VersionUriModel other)
    {
      return (other != null) && (other.m_version.Equals(this.m_version)) && (other.m_language.Equals(m_language));
    }

    public bool Equals(VersionUri other)
    {
      return (other != null) && (m_version.Equals(other.Version)) && (m_language.Equals(other.Language));
    }

    public override string ToString()
    {
      return this.Caption;
    }
  }
}
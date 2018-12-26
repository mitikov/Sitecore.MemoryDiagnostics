using CultureInfo = System.Globalization.CultureInfo;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(typeof(CultureInfo))]
  public class CultureInfoMapping : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public int cultureID;

    [InjectFieldValue]
    public bool m_isReadOnly;

    [InjectFieldValue]
    public string m_name;

    public override string Caption
    {
      get
      {
        return base.Caption + (string.IsNullOrEmpty(m_name) ? "[NoCultureName]" : m_name);
      }
    }

    public override string ToString()
    {
      return Caption;
    }
  }
}
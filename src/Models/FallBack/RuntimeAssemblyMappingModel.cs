namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(@"System.Reflection.RuntimeAssembly")]
  public class RuntimeAssemblyMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string m_fullname;

    public string AssemblyName
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_fullname) || (this.m_fullname.IndexOf(',') < 0))
        {
          return string.Empty;
        }

        return this.m_fullname.Substring(0, this.m_fullname.IndexOf(','));
      }
    }

    public override string Caption => base.Caption + (string.IsNullOrEmpty(this.AssemblyName) ? "No" : this.AssemblyName + " Assembly");
  }
}
namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  using global::System.Diagnostics;
  using Attributes;
  using BaseMappingModel;
  using SitecoreMemoryInspectionKit.Core.AppHelpers;

  [DebuggerDisplay("{m_fullname} class. [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping(@"System.RuntimeType+RuntimeTypeCache")]
  public class RuntimeTypeCacheModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string m_fullname;

    public override string ToString()
    {
      return "{0} type. Address {1}".FormatWith(m_fullname ?? "[NoType", Obj.Address.ToString("X"));
    }
  }
}
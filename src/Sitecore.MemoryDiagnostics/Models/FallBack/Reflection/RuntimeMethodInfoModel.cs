namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  using System.Diagnostics;
  using System.Reflection;

  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [DebuggerDisplay(
     "{m_reflectedTypeCache.m_fullname}.{m_name}. {m_bindingFlags} [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping(@"System.Reflection.RuntimeMethodInfo")]
  public class RuntimeMethodInfoModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public BindingFlags m_bindingFlags;

    [InjectFieldValue]
    public MethodAttributes m_methodAttributes;

    [InjectFieldValue]
    public string m_name;

    [InjectFieldValue]
    public RuntimeTypeCacheModel m_reflectedTypeCache;

    public override string Caption => $"{base.Caption} {m_name}";
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.Reflection
{
  using System.Reflection;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping("System.Reflection.Emit.DynamicMethod+RTDynamicMethod")]
  public class DynamicMethodRTD: ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string m_name;

    [InjectFieldValue]
    public MethodAttributes m_attributes;
  }
}

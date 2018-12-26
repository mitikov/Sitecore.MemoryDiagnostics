using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.Newtonsoft
{
  [ModelMapping(@"Newtonsoft.Json.Linq.JValue")]
  public class JValueMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    protected string _value;

    public string Value => IsEmpty ? "[NoValue]": _value;

    public override string Caption => Value;

    public bool IsEmpty => string.IsNullOrEmpty(_value);
  }
}

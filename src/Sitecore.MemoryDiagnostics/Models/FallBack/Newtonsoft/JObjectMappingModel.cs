using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.Newtonsoft
{
  [ModelMapping("Newtonsoft.Json.Linq.JObject")]
  public class JObjectMappingModel :ClrObjectMappingModel
  {
    [InjectFieldValue]
    private IClrObjMappingModel _properties;

    public ArrayMappingModel Properties => _properties as ArrayMappingModel;

    public override void Compute()
    {
      if (IsEmpty())
      {
        return;
      }   
    }

    public bool IsEmpty() => _properties == null;
  }
}

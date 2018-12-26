using System;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.Newtonsoft
{
  [ModelMapping(@"Newtonsoft.Json.Linq.JProperty")]
  public class JPropertyMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    protected string _name = string.Empty;

    [InjectFieldValue]
    protected IClrObjMappingModel _content;

    public string Name => _name;

    public override string ToString()
      => $"{HexAddress}: {Name}{Environment.NewLine}{_content?.ToString() ?? "[No content found]"}";


    public string AttemptPropertyText()
    {
      return (_content as JPropertyListMappingModel)?.Token?.Value ?? "[NoValue]";
    }
  }

  [ModelMapping(@"Newtonsoft.Json.Linq.JToken")]
  public class JTokenMappingModel: JPropertyMappingModel
  {
  }
}

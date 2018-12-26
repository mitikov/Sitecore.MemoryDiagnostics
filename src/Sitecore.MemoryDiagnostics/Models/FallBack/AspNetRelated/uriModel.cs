using System;
using System.Diagnostics;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  [DebuggerDisplay("{m_String")]
  [ModelMapping(typeof(Uri))]
  public class UriModel: ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string m_String;

    public override string Caption => this.m_String;
  }
}

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System;
  using System.Collections.Generic;
  using BaseMappingModel;
  using Sitecore.Data;

  [Obsolete]
  // [ModelMapping(typeof(FieldListReader))]
  public class FieldListReader : ClrObjectMappingModel
  {
    public Dictionary<ID, string> values;
  }
}
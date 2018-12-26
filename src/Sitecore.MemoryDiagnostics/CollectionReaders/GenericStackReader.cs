namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class GenericStackReader : ArrayReader
  {
    public override IClrObjMappingModel ReadEntity(ClrObject clrObject, ModelMapperFactory factory)
    {
      return ReadTheOnlyArrayInsideType(clrObject, factory);
    }

    public override bool SupportTransformation(ClrObject obj)
    {
      return obj.Type.Name.StartsWith("System.Collections.Generic.Stack", StringComparison.OrdinalIgnoreCase);
    }
  }
}
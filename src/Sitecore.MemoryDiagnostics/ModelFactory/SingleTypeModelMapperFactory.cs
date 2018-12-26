namespace Sitecore.MemoryDiagnostics.ModelFactory
{
  using System;
  using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore.Diagnostics;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   Caches previously converted model type to eliminate model mapping lookup.
  /// </summary>
  public class SingleTypeModelMapperFactory : PropertyModelMapperFactory
  {
    private Tuple<string, Type> prevModelType;

    protected override IClrObjMappingModel GetModelByObjectType(ClrObject clrObject)
    {
      ClrType tp = clrObject.Type;
      if (tp == null)
      {
        return new ClrObjNoType
        {
          Obj = clrObject
        };
      }

      if ((this.prevModelType == null) || (!this.prevModelType.Item1.Equals(tp.Name, StringComparison.Ordinal)))
      {
        IClrObjMappingModel res = base.GetModelByObjectType(clrObject);
        this.prevModelType = new Tuple<string, Type>(tp.Name, res.GetType());
        return res;
      }

      Type modelType = this.prevModelType.Item2;
      var result = Activator.CreateInstance(modelType) as IClrObjMappingModel;
      return Assert.ResultNotNull(
        result,
        $"Could not cast {modelType.FullName} type to {typeof(IClrObjMappingModel).FullName}");
    }
  }
}
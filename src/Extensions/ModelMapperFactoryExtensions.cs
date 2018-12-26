using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.Extensions
{
  public static class ModelMapperFactoryExtensions
  {
    public static T BuildOfType<T>(this IModelMapperFactory factory, [CanBeNullObject] ClrObject obj) where T: class, IClrObjMappingModel
    {
      return factory.BuildModel(obj) as T;
    }
  }
}

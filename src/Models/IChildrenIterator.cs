namespace Sitecore.MemoryDiagnostics.ModelFactory
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  public interface IHaveChildEntries
  {
    IEnumerable<IClrObjMappingModel> ChildrenEntries { get; }
  }
}
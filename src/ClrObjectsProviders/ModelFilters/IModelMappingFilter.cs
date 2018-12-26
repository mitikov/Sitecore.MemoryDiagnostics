namespace Sitecore.MemoryDiagnostics.ModelFilters
{
  using Sitecore.MemoryDiagnostics.Interfaces;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  

  /// <summary>
  /// Allows to apply custom filtering logic on the <see cref="IClrObjMappingModel"/>.
  /// </summary>
  public interface IModelMappingFilter:IFilter<IClrObjMappingModel>
  {
    /// <summary>
    /// Checks if specified model matches given conditions.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    bool Matches([CanBeNull] IClrObjMappingModel model);
  }
}
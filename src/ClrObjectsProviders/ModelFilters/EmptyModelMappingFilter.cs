namespace Sitecore.MemoryDiagnostics.ModelFilters
{
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore;

  /// <summary>
  /// Filters null models. 
  /// </summary>
  public sealed class EmptyModelMappingFilter : IModelMappingFilter
  {
    /// <summary>
    /// Single instance of stateless object.
    /// </summary>
    public static readonly IModelMappingFilter Instance = new EmptyModelMappingFilter();

    /// <summary>
    /// Filters null <paramref name="model"/>.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns><value>false</value> in case model is <c>>null</c>; <value>true</value> otherwise</returns>
    public bool Matches([CanBeNull] IClrObjMappingModel model)
    {
      return model != null;
    }
  }
}
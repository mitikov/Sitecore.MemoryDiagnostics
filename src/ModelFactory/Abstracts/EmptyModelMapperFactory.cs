namespace Sitecore.MemoryDiagnostics.ModelFactory.Abstracts
{
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Does no mappings, always returns <c>null</c>.
  /// </summary>
  /// <seealso cref="ModelFactory.Abstracts.BaseModelMapperFactory" />
  public class EmptyModelMapperFactory : BaseModelMapperFactory
  {
    /// <summary>
    /// A single instance of stateless object.
    /// </summary>
    public static readonly EmptyModelMapperFactory Instance = new EmptyModelMapperFactory();

    public override IClrObjMappingModel BuildModel(ClrObject obj)
    {
      return null;
    }

    public override ArrayMappingModel ReadArray(ClrObject obj)
    {
      return null;
    }

    protected override IClrObjMappingModel DoBuildModelFreeOfRecursion(ClrObject obj)
    {
      return null;
    }
  }
}
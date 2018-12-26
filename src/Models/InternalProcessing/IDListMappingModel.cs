namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using Sitecore.Collections;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  public class IDListMappingModel : ClrObjectMappingModel
  {
    public IDList Collection;

    public static explicit operator IDList(IDListMappingModel model)
    {
      return model == null ? null : model.Collection;
    }
  }
}
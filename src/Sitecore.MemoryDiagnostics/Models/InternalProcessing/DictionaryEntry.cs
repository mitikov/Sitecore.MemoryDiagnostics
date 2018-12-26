namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  public class DictionaryEntryModel : ClrObjectMappingModel
  {
    public readonly object key;

    public readonly object value;

    public DictionaryEntryModel([NotNull] IClrObjMappingModel keyObj, [CanBeNull] IClrObjMappingModel valObj)
    {
      Assert.ArgumentNotNull(keyObj, "key");
      this.key = keyObj;
      this.value = valObj;
    }
  }
}
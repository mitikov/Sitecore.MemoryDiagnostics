namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  public sealed class NoConverterForType : ClrObjectMappingModel
  {
    public override string Caption => base.Caption + $"{this.ObjTypeFullName} not convertable";

    public override string ToString()
    {
      return $"[No Converter for] {this.ObjTypeFullName}. Please visit {@"http://referencesource.microsoft.com/"} site, or use iLSpy to get class fields";
    }
  }
}
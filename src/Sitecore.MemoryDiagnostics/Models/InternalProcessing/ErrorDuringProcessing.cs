namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using BaseMappingModel;
  using Environment = System.Environment;
  using Exception = System.Exception;

  public sealed class ErrorDuringProcessing : ClrObjectMappingModel
  {
    public Exception Ex;

    public override string Caption => base.Caption + this.HexAddress;

    public override string ToString()
    {
      return (this.HasBindingLog ? this.BindingLog.ToString() : "[NoDataInBindingLog]") + Environment.NewLine + (this.Ex == null ? " [ No Exception details]" : this.Ex.ToString());
    }
  }
}
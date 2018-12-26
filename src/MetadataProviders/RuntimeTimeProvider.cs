namespace Sitecore.MemoryDiagnostics.MetadataProviders
{
  using System;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Microsoft.Diagnostics.Runtime;
  using Microsoft.Diagnostics.Runtime.Interop;
  using Sitecore;
  using Sitecore.Diagnostics;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class RuntimeTimeProvider : IRuntimeMetadataProvider<DateTime>
  {
    private DateTime _lastDateTime;
    private int _lastHashCode;

    public static DateTime UnixZeroDate
    {
      get
      {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      }
    }

    public virtual DateTime ExtractData([NotNull] ClrRuntime runtime)
    {
      Assert.ArgumentNotNull(runtime, "runtime");
      try
      {
        DataTarget dt = runtime.DataTarget;

        if (dt.GetHashCode() == _lastHashCode)
          return _lastDateTime;


        uint secondsSinceUnix;
        var dbgCtrl2 = (IDebugControl2)dt.DebuggerInterface;

        dbgCtrl2.GetCurrentTimeDate(out secondsSinceUnix);

        _lastDateTime = UnixZeroDate.AddSeconds(secondsSinceUnix);

        _lastHashCode = dt.GetHashCode();

        return _lastDateTime;
      }
      catch (Exception)
      {
        return DateTime.MinValue;
      }
    }

    public DateTime ExtractData(ClrObject ClrObj)
    {
      ClrAssert.ObjectNotNullTypeNotEmpty(ClrObj);

      return ExtractData(ClrObj.Type.Heap.Runtime);
    }

    public DateTime ExtractData(IClrObjMappingModel model)
    {
      Assert.ArgumentNotNull(model, "model");
      return ExtractData(model.Obj);
    }
  }
}
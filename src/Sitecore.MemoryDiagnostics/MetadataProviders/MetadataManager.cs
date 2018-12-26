namespace Sitecore.MemoryDiagnostics.MetadataProviders
{
  using System;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Diagnostics;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class MetadataManager
  {
    private static IRuntimeMetadataProvider<DateTime> _DumpTimeProvider;

    static MetadataManager()
    {
      _DumpTimeProvider = new RuntimeTimeProvider();
    }

    public IRuntimeMetadataProvider<DateTime> DumpTimeProvider
    {
      set
      {
        Assert.IsNotNull(value, "value is null");
        _DumpTimeProvider = value;
      }
    }

    public static DateTime GetDumpTime([NotNull] ClrRuntime runtime)
    {
      Assert.ArgumentNotNull(runtime, "runtime");

      return _DumpTimeProvider.ExtractData(runtime);
    }

    public static DateTime GetDumpTime([ClrObjAndTypeNotEmpty] ClrObject clrObj)
    {
      ClrAssert.ObjectNotNullTypeNotEmpty(clrObj);

      return _DumpTimeProvider.ExtractData(clrObj);
    }
  }
}
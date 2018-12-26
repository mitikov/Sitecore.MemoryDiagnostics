namespace Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders
{
  using System;
  using Microsoft.Diagnostics.Runtime;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class TimeSpanReader : IPrimitiveEntityReader
  {
    public Type SupportedType
    {
      get
      {
        return typeof(TimeSpan);
      }
    }

    public static bool CanRead(ClrObject obj, string fld)
    {
      try
      {
        var clrFld = obj.Type.GetFieldByName(fld);
        if (clrFld == null)
          return false;
        var val = obj.GetValueFld(fld);
        if (val.IsNullObj)
          return false;
        var ticksFld = val.Type.GetFieldByName("_ticks");
        return ticksFld != null;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public static TimeSpan Read(ClrObject obj, string fldName)
    {
      ClrObject rs = obj.GetValueFld(fldName);
      return Read(rs);
    }

    public static TimeSpan Read(ClrObject obj)
    {
      ClrInstanceField dateDataField = obj.Type.GetFieldByName("_ticks");
      var rawDateTimeData = (long)dateDataField.GetValue(obj.Address, true);
      var ts = new TimeSpan(rawDateTimeData);
      return ts;
    }

    object IPrimitiveEntityReader.Read(ClrObject obj, string fldName)
    {
      return Read(obj, fldName);
    }
  }
}
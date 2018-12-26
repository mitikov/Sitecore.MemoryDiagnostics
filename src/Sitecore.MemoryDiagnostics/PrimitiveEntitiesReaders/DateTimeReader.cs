namespace Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders
{
  using System;
  using System.Reflection;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Helpers;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore.Diagnostics;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Reads <see cref="DateTime"/> from corresponding <see cref="ClrObject"/>.    
  /// </summary>
  /// <seealso cref="Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders.IPrimitiveEntityReader" />
  [ThreadSafeClass]
  public class DateTimeReader : IPrimitiveEntityReader
  {
    #region Fields

    public static string DtFormat;
    private static readonly FieldInfo DateDataFieldRefl;

    #endregion

    #region Constructors
    static DateTimeReader()
    {
      DtFormat = "s";
      DateDataFieldRefl = typeof(DateTime).GetField("dateData", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    #endregion

    #region Public API
    #region Static methods

    /// <summary>
    ///   Reads date from <see cref="ClrObject" /> which points on <see cref="DateTime" />.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public static DateTime FromObjToDate(ClrObject obj)
    {
      ClrAssert.ObjectNotNullTypeNotEmpty(obj);
      DateTime result;
      if (obj.Type.Name != typeof(DateTime).FullName)
      {
        result = DateTime.MinValue;
      }
      else
      {
        ClrInstanceField dateDataField = obj.Type.GetFieldByName("dateData");
        var rawDateTimeData = (ulong)dateDataField.GetValue(obj.Address, true);
        result = TicksToDt(rawDateTimeData);
      }

      return result;
    }

    /// <summary>
    /// Reads date from given object field.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="fld">The field.</param>
    /// <returns></returns>
    public static DateTime FromObjToDate(ClrObject obj, ClrField fld)
    {
      Assert.ArgumentNotNull(fld, "field");
      return FromObjToDate(obj, fld.Name);
    }

    /// <summary>
    ///   Reads date from object field.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="fldName">Name of the field.</param>
    /// <returns></returns>
    public static DateTime FromObjToDate(ClrObject obj, string fldName)
    {
      ClrObject val = obj.GetValueFld(fldName);
      return FromObjToDate(val);
    }

    public static DateTime TicksToDt(ulong ticks)
    {
      DateTime dt = DateTime.Now;
      ReflectionHelper.SetValueForValueType(DateDataFieldRefl, ref dt, ticks);
      return dt;
    }

    #endregion

    #region IPrimitiveEntityReader Implementation
    public Type SupportedType => typeof(DateTime);

    public object Read(ClrObject obj, string fldName)
    {
      return FromObjToDate(obj, fldName);
    }
    #endregion
    #endregion
  }
}
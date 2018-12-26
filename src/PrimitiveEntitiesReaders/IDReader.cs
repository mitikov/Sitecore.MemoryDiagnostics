namespace Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders
{
  using System;
  using Sitecore.Data;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class IDReader : IPrimitiveEntityReader
  {
    public Type SupportedType
    {
      get
      {
        return typeof(ID);
      }
    }

    /// <summary>
    ///   Reads the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Obj has other than ID type.</exception>
    public static ID Read(ClrObject obj)
    {
      if (obj.IsNullObj)
        return null;

      obj.ReEvaluateType();

      if (obj.Type == null)
        return null;

      if (!obj.Type.Name.Equals(typeof(ID).FullName, StringComparison.Ordinal))
        throw new ArgumentException("Obj has wrong " + obj.Type.Name + " instead of ID");
      ClrObject guidObject = obj.GetValueFld("_guid", false);
      if (guidObject.IsNullObj)
        return null;
      return new ID(ClrObjectValuesReader.ReadGuidValue(guidObject));
    }


    /// <summary>
    ///   Reads the identifier field.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="fldName">Name of the field to read <see cref="ID" /> from.</param>
    /// <returns></returns>
    public static ID ReadIdField(ClrObject obj, string fldName)
    {
      if (obj.IsNullObj)
        return null;

      ClrObject sitecoreIDref = obj.GetRefFld(fldName, false);

      return Read(sitecoreIDref);
    }

    public object Read(ClrObject obj, string fldName)
    {
      return ReadIdField(obj, fldName);
    }
  }
}
namespace Sitecore.MemoryDiagnostics.Extensions
{
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Runtime;
  using System.Runtime.CompilerServices;

  using Diagnostics;
  using PrimitiveEntitiesReaders;
  using Sitecore;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Handy methods for <see cref="ClrObject"/>.
  /// <para>Allows to read string in safe manner, or check field presence.</para>
  /// </summary>
  public static class ClrObjecttensions
  {
    /// <summary>
    /// Gets <see cref="string"/> from <paramref name="clrObject"/> in safe manner - no more than <paramref name="maxStringSize"/>.   
    /// </summary>
    /// <param name="clrObject">The pointer to object.</param>
    /// <param name="textFieldName">Name of the field to read text from.</param>
    /// <param name="maxStringSize">Maximum size of the string to read.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static string GetStringSafe(this ClrObject clrObject, string textFieldName, int maxStringSize = 5000)
    {
      if (clrObject.Type == null)
      {
        return TextConstants.NoType;
      }

      var fld = clrObject.Type.Fields.FirstOrDefault(t => t.Name.Equals(textFieldName, StringComparison.Ordinal));
      if (fld == null)
      {
        return "[NoMatchingFld]";
      }

      var strFld = clrObject.Type.GetFieldByName(textFieldName);
      if ((strFld.Type == null) || (!strFld.Type.IsString))
      {
        return "[FldIsNotOfStingType]";
      }

      var heap = clrObject.Type.Heap;
      ulong strAddress;
      if (clrObject.Type.Heap.ReadPointer(strFld.GetAddress(clrObject.Address), out strAddress))
      {
        return StringReader.ReadStringSafe(heap, strAddress, maxStringSize);
      }

      return TextConstants.CouldNotReadPointer;
    }

    /// <summary>
    /// Gets <see cref="string" /> from <paramref name="stringClrObject" /> in safe manner - no more than <paramref name="maxStringSize" />.
    /// </summary>
    /// <param name="stringClrObject">The string object to read from.</param>
    /// <param name="maxStringSize">Maximum size of the string to read.</param>
    /// <returns>No more than specified number of chars read from given object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static string GetStringSafeFromSelf(this ClrObject stringClrObject, int maxStringSize = 5000)
    {
      if (stringClrObject.Type == null)
      {
        return TextConstants.NoType;        
      }

      if (!stringClrObject.Type.IsString)
      {
        return "[IsNotOfStingType]";
      }

      var heap = stringClrObject.Type.Heap;
      return StringReader.ReadStringSafe(heap, stringClrObject.Address, maxStringSize);
    }

    /// <summary>
    /// Determines whether <paramref name="clrObject"/> has auto-property (backed field) that matches <paramref name="autoPropFldInfo"/>.
    /// </summary>
    /// <param name="clrObject">The object.</param>
    /// <param name="autoPropFldInfo">The automatic property field information.</param>
    /// <returns>
    ///   <c>true</c> if has same name automatic property as the specified automatic property field information; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSameNameAutoProperty(this ClrObject clrObject, [NotNull] FieldInfo autoPropFldInfo)
    {
      Assert.ArgumentNotNull(autoPropFldInfo, "fieldInfo");

      var expected = MemoryDiagnostics.StringUtil.ProduceAutoPropertyName(autoPropFldInfo.Name);
      return clrObject.HasSameNameField(expected);
    }

    /// <summary>
    /// Determines whether <paramref name="clrObject"/> has field with <paramref name="fieldName"/>.
    /// </summary>
    /// <param name="clrObject">The color object.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>
    ///   <c>true</c> if given object has field with provided name; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static bool HasSameNameField(this ClrObject clrObject, [NotNull] string fieldName)
    {
      Assert.ArgumentNotNullOrEmpty(fieldName, "fieldName");
      return (clrObject.Type != null) &&
              clrObject.Type.Fields.Any(t => t.Name.Equals(fieldName, StringComparison.Ordinal));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSameNameField(this ClrObject clrObject, [NotNull] FieldInfo fieldInfo)
    {
      Assert.ArgumentNotNull(fieldInfo, "fieldInfo");
      return clrObject.HasSameNameField(fieldInfo.Name);
    }


    public static ClrObject GetRefFldOrAutoProperty(this ClrObject clrObject, string fieldName)
    {
      if (clrObject.HasSameNameField(fieldName))
      {
        return clrObject.GetRefFld(fieldName);
      }

      var autoProp = MemoryDiagnostics.StringUtil.ProduceAutoPropertyName(fieldName);

      return clrObject.GetRefFld(autoProp);
    }

    public static string GetStringFldOrAutoProp(this ClrObject clrObject, string fieldName)
    {
      if (clrObject.HasSameNameField(fieldName))
      {
        return clrObject.GetStringFld(fieldName);
      }

      var autoProp = MemoryDiagnostics.StringUtil.ProduceAutoPropertyName(fieldName);

      return clrObject.GetStringFld(autoProp);
    }
  }
}
namespace Sitecore.MemoryDiagnostics.Exceptions
{
  using System;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.StringExtensions;

  [NotUsed]
  [Serializable]
  public class FieldMissingInStaticType : Exception
  {
    public readonly string CheckedType;
    public readonly string MissingFldName;

    public FieldMissingInStaticType([NotNull] string fldName, [NotNull] ClrType clrType) : this(fldName, clrType.Name)
    {
    }

    public FieldMissingInStaticType([NotNull] string fldName, [NotNull] string typeName)
    {
      Assert.ArgumentNotNullOrEmpty(fldName, "field name");
      Assert.ArgumentNotNullOrEmpty(typeName, "type name");
      MissingFldName = fldName;
      CheckedType = typeName;
    }

    public string Details
    {
      get
      {
        return "Could not find {0} field in {1} class".FormatWith(MissingFldName, CheckedType);
      }
    }
  }
}
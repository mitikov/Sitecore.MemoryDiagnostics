namespace Sitecore.MemoryDiagnostics.ModelFactory
{
  using System.Reflection;
  using Microsoft.Diagnostics.Runtime;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class ViolationFormatter
  {
    public static string NoFieldInSource(ClrObject obj, FieldInfo fld) => $"{obj.Type} obj does not have {fld.Name} field";

    public static string TypesAreNotCompatible(FieldInfo modelField, ClrInstanceField fld) => $"Could not inject {fld.ElementType} type into {modelField.FieldType.FullName} for {modelField.Name} field";
  }
}
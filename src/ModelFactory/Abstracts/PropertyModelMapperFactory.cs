namespace Sitecore.MemoryDiagnostics.ModelFactory.Abstracts
{
  using System.Reflection;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.Diagnostics;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class PropertyModelMapperFactory : ModelMapperFactory
  {
    /// <summary>
    /// Determines whether this instance [can inject field] the specified model field.
    /// </summary>
    /// <param name="modelField">The model field.</param>
    /// <param name="obj">The object.</param>
    /// <param name="violation">The violation.</param>
    /// <returns>
    ///   <c>true</c> if this instance [can inject field] the specified model field; otherwise, <c>false</c>.
    /// </returns>
    protected override bool CanInjectField(FieldInfo modelField, ClrObject obj, out string violation)
    {
      return base.CanInjectField(modelField, obj, out violation) ||
             obj.HasSameNameAutoProperty(modelField);
    }

    protected override bool ReadFieldValue(FieldInfo targetSetFldInfo, ClrObject sourceObj, out object val)
    {
      Assert.ArgumentNotNull(targetSetFldInfo, "targetSetFldInfo");

      if (sourceObj.HasSameNameAutoProperty(targetSetFldInfo))
      {
        /*Dirty hack: change field name on fly */
        var tp = targetSetFldInfo.GetType();

        var fldName = tp.GetField("m_name", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        Assert.IsNotNull(fldName, "Could not get fieldName");

        fldName.SetValue(targetSetFldInfo, StringUtil.ProduceAutoPropertyName(targetSetFldInfo.Name));
      }

      return base.ReadFieldValue(targetSetFldInfo, sourceObj, out val);
    }
  }
}
using StringBuilder = System.Text.StringBuilder;

namespace Sitecore.MemoryDiagnostics.Models.BaseMappingModel
{
  using ModelFactory.Abstracts;
  using Sitecore.MemoryDiagnostics.Attributes;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   Provides binding between class instance in memory dump and class model.
  ///   <para>Binding is done via marking derived class with <see cref="ModelMappingAttribute" />.</para>
  ///   <para>
  ///     Derived class must have fields with exact same names as source class fields, and fields must be marked with
  ///     <see cref="InjectFieldValueAttribute" /> to allow further binding.
  ///   </para>
  ///   <para>
  ///     <see cref="Compute" /> method would be called after injection of fields is done to allow some custom
  ///     processing.
  ///   </para>
  ///   <example>
  ///     <para>Create custom classes derived from <seealso cref="ClrObjectMappingModel" /> class.</para>
  ///   </example>
  /// </summary>
  public interface IClrObjMappingModel
  {
    /// <summary>
    /// Gets the binding log - errors, or messages related to mapping this model to a concrete type.
    /// </summary>
    /// <value>
    /// The binding log.
    /// </value>
    StringBuilder BindingLog { get; }

    /// <summary>
    /// Gets the name of the model which current instance represents via <see cref="ModelMappingAttribute"/> value.
    /// </summary>
    /// <value>
    /// The name of the model of type. <c>string.Empty in case attribute was not found.</c>
    /// </value>
    string ModelOfTypeName { get; }

    /// <summary>
    /// Gets or sets the object which current module describes.
    /// </summary>
    /// <value>
    /// The object.
    /// </value>
    ClrObject Obj { get; set; }

    /// <summary>
    /// Called after <see cref="IModelMapperFactory"/> injected fields decorated with <see cref="InjectFieldValueAttribute"/>.
    /// <para>Allows to set custom model fields based on <see cref="Obj"/>.</para>
    /// </summary>
    void Compute();
  }
}
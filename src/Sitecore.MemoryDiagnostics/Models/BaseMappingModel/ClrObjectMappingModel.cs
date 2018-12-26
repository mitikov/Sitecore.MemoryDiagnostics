namespace Sitecore.MemoryDiagnostics.Models.BaseMappingModel
{
  using System;
  using Sitecore;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelMetadataInterfaces;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  using CustomAttributeExtensions = System.Reflection.CustomAttributeExtensions;
  using Enumerable = System.Linq.Enumerable;
  using Environment = System.Environment;
  using MemberInfo = System.Reflection.MemberInfo;
  using StringBuilder = System.Text.StringBuilder;

  /// <summary>
  ///   Provides a binding between class instance in memory snapshot and its model, thereby allowing field injection.
  ///   <para>Custom models should derive from this class.</para>
  ///   <para>Binding to source class done via marking derived class with <see cref="ModelMappingAttribute" />.</para>
  ///   <para>
  ///     Derived class must have fields with exact same names as source class fields, and decorated with
  ///     <see cref="InjectFieldValueAttribute" /> to allow further binding.
  ///   </para>
  ///   <para>
  ///     <see cref="Compute" /> method would be called after injection of fields is done to allow some custom
  ///     processing.
  ///   </para>
  ///   <example>
  ///     <para>See <seealso cref="ItemDataModel" /> class as example</para>
  ///   </example>
  /// </summary>
  public abstract class ClrObjectMappingModel : IClrObjMappingModel, IComparable<ClrObjectMappingModel>,
    IEquatable<ClrObjectMappingModel>, IEquatable<ulong>, ICaptionHolder
  {
    #region private fields

    private StringBuilder _binderLog;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the address of the object in memory snapshot
    /// </summary>
    /// <value>
    /// The address.
    /// </value>
    public ulong Address => this.Obj.Address;

    /// <summary>
    /// Gets the binding log - errors, or messages related to mapping this model to a concrete type.
    /// </summary>
    /// <value>
    /// The binding log.
    /// </value>
    public StringBuilder BindingLog => this._binderLog ?? (this._binderLog = new StringBuilder());

    /// <summary>
    /// A brief instance description.
    /// </summary>
    public virtual string Caption
    {
      get
      {
#if DEBUG
        return string.Concat(HexAddress, " [", GetType().Name, "] ");
#else
        return string.Intern(MemoryDiagnostics.StringUtil.TrimTo(string.Concat("[", GetType().Name, "] ")));
#endif
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has binding log.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance has binding log; otherwise, <c>false</c>.
    /// </value>
    public bool HasBindingLog => this._binderLog != null;

    /// <summary>
    ///   Gets the hexadecimal address of underlying object.
    ///   <para>Syntax sugar.</para>
    /// </summary>
    /// <value>
    ///   The hexadecimal address.
    /// </value>
    [NotNull]
    public string HexAddress => this.Address.ToString("x8");

    /// <summary>
    /// Gets the name of the current model type.
    /// </summary>
    /// <value>
    /// The name of the instance type.
    /// </value>
    [NotNull]
    public string InstanceTypeName => this.GetType().Name;

    /// <summary>
    /// Gets the name of the model which current object represents
    /// </summary>
    /// <value>
    /// The name of the model of type; <c>string.Empty</c> in case attribute is not applied.
    /// </value>
    public virtual string ModelOfTypeName => ModelMappingAttribute.GetTypeToMapOn(this.GetType());

    /// <summary>
    /// Gets or sets the object from memory snapshot represented by current instance.
    /// </summary>
    /// <value>
    /// The object.
    /// </value>
    public ClrObject Obj { get; set; }

    /// <summary>
    /// Gets the full name of the inner <see cref="ClrObject"/> type.
    /// </summary>
    /// <value>
    /// The full name of the object type.
    /// </value>
    [NotNull]
    public string ObjTypeFullName => this.Obj.Type == null ? "[NoType]" : this.Obj.Type.Name;

    /// <summary>
    /// Gets the type for <see cref="Obj"/> WITHOUT namespace.
    /// </summary>
    /// <value>
    /// The type only.
    /// </value>
    [NotNull]
    public string TypeOnly
    {
      get
      {
        var classNameStartsIndex = this.ObjTypeFullName.LastIndexOf('.');
        return classNameStartsIndex > 0 ? this.ObjTypeFullName.Substring(classNameStartsIndex + 1) : this.ObjTypeFullName;
      }
    }

    #endregion

    #region IComparable implementation

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// <para>Objects are compared by <see cref="Address"/>.</para>
    /// </summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows <paramref name="other" /> in the sort order.
    /// </returns>
    public int CompareTo([CanBeNull] ClrObjectMappingModel other)
    {
      return other == null ? 1 : this.Obj.Address.CompareTo(other.Obj.Address);
    }
    #endregion

    /// <summary>
    /// Called after <see cref="IClrObjMappingModel"/> fields are injected.
    /// <para>Placeholder for custom logic execution once field injection is over.</para>
    /// </summary>
    public virtual void Compute()
    {
    }

    #region Equals overloads
    public bool Equals(ClrObjectMappingModel other)
    {
      return other != null && this.Equals(other.Address);
    }

    public bool Equals(ulong other)
    {
      return this.Obj.Address.Equals(other);
    }

    #endregion

    public override string ToString()
    {
      var fields =
        Enumerable.Where(this.GetType()
          .GetFields(), field =>
          CustomAttributeExtensions.IsDefined((MemberInfo)field, typeof(InjectFieldValueAttribute)) &&
          ((field.FieldType == typeof(string)) || field.FieldType.IsPrimitive));

      var objectStringRepresentation = new StringBuilder(capacity: 6000);

      objectStringRepresentation.AppendFormat("{0} {1}", this.GetType().Name, Environment.NewLine);
      foreach (var field in fields)
      {
        var fldValue = field.GetValue(this);
        if (field.FieldType.IsClass && (fldValue == null))
        {
          objectStringRepresentation.AppendFormat("{0} --> {1}{2}", field.Name, "[Null]", Environment.NewLine);
          continue;
        }

        if ((field.FieldType == typeof(string)) && string.IsNullOrEmpty((string)fldValue))
        {
          objectStringRepresentation.AppendFormat("{0} --> {1}{2}", field.Name, "[string.Empty]", Environment.NewLine);
          continue;
        }

        objectStringRepresentation.AppendFormat("{0} --> {1}{2}", field.Name, fldValue, Environment.NewLine);
      }

      return objectStringRepresentation.ToString();
    }
  }
}
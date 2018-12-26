namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using BaseMappingModel;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   Represents a base mapping for arrays.
  /// <para>Can be enumerated as <see cref="ClrObject"/>, or <see cref="IClrObjMappingModel"/>.</para>
  /// <para>Can be converted to array of <see cref="IClrObjMappingModel"/> objects.</para>
  /// </summary>
  [NotThreadSafe]
  [DebuggerDisplay("{Elements.Count} [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  public class ArrayMappingModel : ClrObjectMappingModel, IEnumerable<ClrObject>, IEnumerable<IClrObjMappingModel>,
    IEquatable<ArrayMappingModel>
  {
    #region Static fields

    [NotNull]
    public static string EmptyModelText => EmptyClrObjectModel.EmptyModelText;

    #endregion

    #region Instance Fields
    /// <summary>
    ///   Read array content
    /// </summary>
    public readonly List<IClrObjMappingModel> Elements = new List<IClrObjMappingModel>();

    #endregion

    #region Properties

    public override string Caption => base.Caption + this.Elements.Count + " elements";

    /// <summary>
    /// Gets the count of elements stored inside this array mapping.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    public int Count => this.Elements?.Count ?? 0;

    /// <summary>
    ///   Gets a value indicating whether this instance does not have any elements.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty => this.Count == 0;

    #endregion

    #region Indexer

    /// <summary>
    ///   Gets the <see cref="IClrObjMappingModel" /> at the specified index.
    /// </summary>
    /// <value>
    ///   The <see cref="IClrObjMappingModel" />.
    /// </value>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    public IClrObjMappingModel this[int index] => this.Elements[index];
    #endregion

    #region Public API
    /// <summary>
    ///   Adds the element into current <see cref="ArrayMappingModel" />
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [NotNull]
    public ArrayMappingModel AddElement([NotNull] IClrObjMappingModel model)
    {
      Assert.ArgumentNotNull(model, "model");

      this.Elements.Add(model);

      return this;
    }

    public bool Equals(ArrayMappingModel other)
    {
      if (other == null)
      {
        return false;
      }

      return this.Address.Equals(other.Address);
    }

    #region Enumeration

    public IEnumerator GetEnumerator()
    {
      return this.Elements.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection as <see cref="ClrObject"/>.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    IEnumerator<ClrObject> IEnumerable<ClrObject>.GetEnumerator()
    {
      return this.Elements.Select(model => model.Obj).GetEnumerator();
    }

    IEnumerator<IClrObjMappingModel> IEnumerable<IClrObjMappingModel>.GetEnumerator()
    {
      return this.Elements.GetEnumerator();
    }

    #endregion

    #endregion

    #region Static methods

    /// <summary>
    ///   Arrays the is null or empty.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns><c>true</c> if array model is <c>null</c>, or <see cref="IsEmpty"/>.</returns>
    public static bool ArrayIsNullOrEmpty([CanBeNull] IClrObjMappingModel obj)
    {
      var tmp = obj as ArrayMappingModel;
      return (tmp == null) || tmp.IsEmpty;
    }

    /// <summary>
    /// Tranforms inner elements to the array.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [NotNull]
    public static IClrObjMappingModel[] ToArray([CanBeNull] ArrayMappingModel model)
    {
      return ArrayIsNullOrEmpty(model) ? new IClrObjMappingModel[0] : model.Elements.ToArray();
    }

    #endregion

    #region Cast operators
    [NotNull]
    public static explicit operator IClrObjMappingModel[] (ArrayMappingModel model)
    {
      return ToArray(model);
    }

    public static explicit operator List<IClrObjMappingModel>(ArrayMappingModel model)
    {
      if ((model == null) || model.IsEmpty)
      {
        return new List<IClrObjMappingModel>(0);
      }

      return model.Elements;
    }

    #endregion
  }
}
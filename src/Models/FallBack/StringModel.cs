using IEnumerable = System.Collections.IEnumerable;
using IEnumerator = System.Collections.IEnumerator;
using StringComparison = System.StringComparison;

namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  using System;
  using System.Collections.Generic;
  using global::System.Diagnostics;

  using Sitecore;

  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
  using Sitecore.MemoryDiagnostics.ModelMetadataInterfaces;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  /// <summary>
  ///   <see cref="string" /> model with pointer.
  ///   <para>Used during reading <see cref="string" /> via <see cref="BaseModelMapperFactory.BuildModel" /> API.</para>
  ///   <remarks>Models are equal, when underlaying data is same.</remarks>
  ///   <para>For reference equality, please use <see cref="ulong" /> overload.</para>
  /// </summary>
  [DebuggerDisplay("{Value} [Model] {GetType().Name}")]
  [ModelMapping(typeof(string))]
  public class StringModel : ClrObjectMappingModel, IEquatable<string>, IEquatable<ulong>, IEquatable<StringModel>, IEnumerable<char>,
    IComparable<string>,
    ICaptionHolder
  {
    #region Fields
    [NotNull]
    protected string value;
    #endregion
    #region Properties
    public override string Caption => MemoryDiagnostics.StringUtil.TrimTo("[String] " + this.Value);
    public string Value => this.IsNullOrEmpty ? "[emptyOrNull]" : this.value;

    public bool IsNullOrEmpty => string.IsNullOrEmpty(this.value);
    #endregion

    #region Compute calls
    public override void Compute()
    {
      Sitecore.Diagnostics.Assert.IsNotNull(this.Obj.Type, "No type");

      var boxedVal = this.Obj.GetStringSafeFromSelf();

      // TODO: Do we need interning here ?
      this.value = string.Intern(!string.IsNullOrEmpty(boxedVal) ? boxedVal : "[null string]");
    }
    #endregion

    public override string ToString()
    {
      return "String Model: " + Value;
    }

    public override int GetHashCode()
    {
      return value?.GetHashCode() ?? (int)Address;
    }

    public int CompareTo(string other)
    {
      return this.value.CompareTo(other);
    }

    #region Equals 

    public override bool Equals(object obj)
    {
      if (obj is string)
      {
        return this.Equals(obj as string);
      }

      if (obj is IClrObjMappingModel)
      {
        if (obj is StringModel)
        {
          return this.Equals(obj as StringModel);
        }
        return this.Equals((obj as IClrObjMappingModel)?.Obj.Address ?? 0);
      }

      return object.ReferenceEquals(this, obj);
    }

    public bool Equals(string other)
    {
      if (string.IsNullOrEmpty(other) && string.IsNullOrEmpty(this.value))
      {
        return true;
      }

      if (string.IsNullOrEmpty(other) || string.IsNullOrEmpty(this.value))
      {
        return false;
      }

      return string.Equals(other, this.value, StringComparison.Ordinal);
    }

    public bool Equals(StringModel other)
    {
      return (other != null) && this.Equals(other.value);
    }

    public bool Equals(ulong other)
    {
      return Obj.Address.Equals(other);
    }

    #endregion


    #region Enumeration
    public IEnumerator<char> GetEnumerator()
    {
      return this.value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion

    #region Casts

    [NotNull]
    public static explicit operator string(StringModel model)
    {
      return model?.value ?? "[null string]";
    }

    #endregion 
  }
}
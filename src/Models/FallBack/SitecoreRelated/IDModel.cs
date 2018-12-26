namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System;
  using System.Diagnostics;

  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Helpers;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Data;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   Mapping For Sitecore <see cref="Sitecore.Data.ID" /> class.
  ///   <para>Stores <see cref="ID" /> with <see cref="ClrObject" /> inside.</para>
  ///   <para>Implements <see cref="System.IEquatable{T}" /> and <see cref="System.IComparable" /></para>
  /// </summary>
  [DebuggerDisplay("{Id} [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping(typeof(ID))]
  public class IDModel : ClrObjectMappingModel, IEquatable<IDModel>, IEquatable<Guid>, IEquatable<ID>, IComparable<Guid>,
    IComparable<ID>
  {
    [InjectFieldValue]
    public Guid _guid;

    public override string Caption => base.Caption + IDsNameHelper.ToString(this._guid);

    public ID Id => new ID(this._guid);

    public static explicit operator Guid(IDModel model)
    {
      return model?._guid ?? Guid.Empty;
    }

    public static implicit operator ID(IDModel model)
    {
      return model?.Id;
    }

    public int CompareTo(Guid other)
    {
      return this._guid.CompareTo(other);
    }

    public int CompareTo(ID other)
    {
      if (ID.IsNullOrEmpty(other))
      {
        if (Guid.Empty == this._guid)
        {
          return 0;
        }

        return 1;
      }

      return this.CompareTo(other.Guid);
    }

    public bool Equals(IDModel other)
    {
      return (other != null) && Equals(other._guid);
    }

    public bool Equals(Guid other)
    {
      return other.Equals(_guid);
    }

    public bool Equals(ID other)
    {
      if (ID.IsNullOrEmpty(other))
      {
        return Guid.Empty == _guid;
      }

      return Equals(other.Guid);
    }

    public override string ToString()
    {
      return _guid.ToString("B");
    }
  }
}
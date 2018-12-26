namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Utils;

  [ModelMapping(typeof(Sitecore.Data.Version))]
  public class VersionModel : ClrObjectMappingModel, IEquatable<VersionModel>, IEquatable<Data.Version>, IComparable<VersionModel>, IComparable<Data.Version>
  {
    [InjectFieldValue]
    public int _number;

    public override string Caption => $"{base.Caption} {this._number}";


    public int ShiftedHashCode => VersionUtils.ShiftHashCode(this._number);

    public static bool operator ==(Data.Version uno, VersionModel duo)
    {
      if ((uno == null) && (duo == null))
      {
        return true;
      }

      if ((uno == null) || (duo == null))
      {
        return false;
      }

      return uno.Number == duo._number;
    }

    public static bool operator !=(Data.Version uno, VersionModel duo)
    {
      return !(uno == duo);
    }

    public int CompareTo(VersionModel other)
    {
      return this.Equals(other) ? 0 : this._number.CompareTo(other._number);
    }

    public int CompareTo(Data.Version other)
    {
      return this.Equals(other) ? 0 : this._number.CompareTo(other.Number);
    }

    public bool Equals(VersionModel other)
    {
      return (other != null) && (other._number == _number);
    }

    public bool Equals(Data.Version other)
    {
      return (other != null) && (other.Number == _number);
    }

    public override string ToString()
    {
      return this._number.ToString();
    }
  }
}
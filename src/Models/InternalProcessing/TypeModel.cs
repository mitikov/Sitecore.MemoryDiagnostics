namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using Microsoft.Diagnostics.Runtime;
  using Sitecore.Diagnostics;
  

  /// <summary>
  /// Represents <see cref="System.Type"/> model.
  /// </summary>
  /// <seealso cref="TypeModel" />
  public class TypeModel : global::System.IEquatable<TypeModel>
  {
    public readonly int hash;

    /// <summary>
    /// The type Meta Table.
    /// </summary>
    public readonly ulong MT;

    /// <summary>
    /// The type name.
    /// </summary>
    public readonly string TypeName;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeModel"/> class.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="mt">The mt.</param>
    public TypeModel([NotNull] string typeName, ulong mt)
    {
      Assert.ArgumentNotNullOrEmpty(typeName, "typeName");
      this.TypeName = typeName;
      this.MT = mt;
      this.hash = mt.GetHashCode();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeModel"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public TypeModel([NotNull] ClrType type) : this(type.Name, type.MetadataToken)
    {
      Assert.ArgumentNotNull(type, "type");
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    public bool Equals([CanBeNull] TypeModel other)
    {
      return other?.hash == this.hash;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode() => this.hash;

    public override string ToString()
    {
      var mt = this.MT.ToString("x8");
      return string.Concat(mt, " ", this.TypeName);
    }
  }
}

namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  using System;
  using System.Diagnostics;
  using System.Text.RegularExpressions;

  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelMetadataInterfaces;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  /// <summary>
  ///   <see cref="System.Text.RegularExpressions.Regex" /> mapping with regex pattern and <see cref="System.Text.RegularExpressions.RegexOptions" />
  /// </summary>
  [DebuggerDisplay("{pattern} {roptions.ToString()} {HexAddress}")]
  [ModelMapping(typeof(Regex))]
  public class RegexMappingModel : ClrObjectMappingModel, IEquatable<RegexMappingModel>, ICaptionHolder
  {
    [InjectFieldValue]
    public string pattern;

    [InjectFieldValue]
    public RegexOptions roptions;

    private int _hashcode;

    public string Caption => this.Pattern;

    public string Pattern => string.IsNullOrEmpty(this.pattern) ? "[NoPattern]" : this.pattern;

    /// <summary>
    ///   Computes this instance. Invoked after all fields were injected.
    /// </summary>
    public override void Compute()
    {
      if (string.IsNullOrEmpty(this.pattern))
      {
        return;
      }

      this._hashcode = this.pattern.GetHashCode();
    }

    public bool Equals(RegexMappingModel other)
    {
      return (other != null) && other._hashcode.Equals(this._hashcode);
    }

    public override int GetHashCode()
    {
      return _hashcode;
    }

    public override string ToString()
    {
      return string.Concat(
        this.Pattern,
        " ",
        this.roptions.ToString("G"),
        " ",
        this.HexAddress);
    }
  }
}
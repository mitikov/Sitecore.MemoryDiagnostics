namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System;
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Utils;
  using Sitecore;
  using Sitecore.Globalization;
  using StringComparison = System.StringComparison;  

  [DebuggerDisplay("{_name} [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping(typeof(Language))]
  public class LanguageModel : ClrObjectMappingModel, IEquatable<LanguageModel>, IEquatable<Language>
  {
    public static readonly string NoLanguageText = "[No langauge data]";

    [InjectFieldValue]
    public string _name;

    public int? cultureHashCode;

    [InjectFieldValue]
    public string languageName;

    public override string Caption => base.Caption + (this._name ?? this.languageName);

    public int CultureHashCode
    {
      get
      {
        if (this.cultureHashCode.HasValue)
        {
          return this.cultureHashCode.Value;
        }

        this.cultureHashCode = this.HasName ? CultureInfoUtils.FastHash(this._name) : CultureInfoUtils.FastHash(this.ToString());
        return this.cultureHashCode.GetValueOrDefault();
      }
    }

    public bool HasName => !string.IsNullOrEmpty(this._name);

    [NotNull]
    public static string GetLanguageName([CanBeNull] LanguageModel model)
    {
      return model == null ? NoLanguageText : model.ToString();
    }

    public static bool operator ==(LanguageModel uno, Language duo)
    {
      if (ReferenceEquals(uno, null) && ReferenceEquals(duo, null))
      {
        return true;
      }

      if (ReferenceEquals(uno, null) || ReferenceEquals(duo, null))
      {
        return false;
      }

      return uno.languageName == duo.Name;
    }

    public static bool operator !=(LanguageModel uno, Language duo)
    {
      return !(uno == duo);
    }

    public bool Equals([CanBeNull] LanguageModel other)
    {
      return (other != null) && string.Equals(other.languageName, this.languageName, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals([CanBeNull] Language other)
    {
      return (other != null) && string.Equals(other.Name, this.languageName, StringComparison.OrdinalIgnoreCase);
    }


    public override string ToString()
    {
      return this._name ?? this.languageName ?? NoLanguageText;
    }
  }
}
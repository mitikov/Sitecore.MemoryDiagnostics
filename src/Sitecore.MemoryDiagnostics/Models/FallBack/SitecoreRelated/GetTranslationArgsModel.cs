using Environment = System.Environment;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore;
  using Sitecore.Pipelines.GetTranslation;

  [ModelMapping(typeof(GetTranslationArgs))]
  public class GetTranslationArgsModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool _suspended;

    [NotNull]
    public string Key;

    [NotNull]
    public string Language;

    [InjectFieldValue]
    protected LanguageModel language;

    public override string Caption
    {
      get
      {
        return base.Caption + Language + " - > " + Key;
      }
    }

    public override void Compute()
    {
      Key = Obj.GetStringFld("<Key>k__BackingField") ?? "[NoKey]";
      Language = LanguageModel.GetLanguageName(language);
    }

    public override string ToString()
    {
      return Caption + Environment.NewLine + " is suspended: " + _suspended;
    }
  }
}
namespace Sitecore.MemoryDiagnostics.CollectionReaders.SitecoreSpecific
{
  using System;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class JobDictionaryReader : SafeDictionaryReader
  {
    public static string JobDictionaryClassName = @"Sitecore.Collections.JobDictionary";

    public override bool SupportTransformation(ClrObject obj)
    {
      return obj.Type.Name.StartsWith(JobDictionaryClassName, StringComparison.OrdinalIgnoreCase);
    }
  }
}
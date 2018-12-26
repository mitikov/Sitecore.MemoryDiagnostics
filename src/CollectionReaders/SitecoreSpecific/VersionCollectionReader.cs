namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class VersionCollectionReader : GenericListReader
  {
    public override bool SupportTransformation(ClrObject obj)
    {
      return (obj.Type != null) && string.Equals(obj.Type.Name, (@"Sitecore.Collections.VersionCollection"), StringComparison.OrdinalIgnoreCase);
    }
  }
}
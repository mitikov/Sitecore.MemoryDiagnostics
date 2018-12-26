namespace Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders
{
  using System;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Reads <see cref="Guid"/> from corresponding <see cref="ClrObject"/>.    
  /// </summary>
  /// <seealso cref="Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders.IPrimitiveEntityReader" />
  public class GuidReader : IPrimitiveEntityReader
  {
    public Type SupportedType => typeof(Guid);

    public object Read(ClrObject obj, string fldName)
    {
      ClrObject guidObject = obj.GetValueFld(fldName, false);

      // TOOD: Ensure this code works.
      return ClrObjectValuesReader.ReadGuidValue(guidObject);
    }
  }
}
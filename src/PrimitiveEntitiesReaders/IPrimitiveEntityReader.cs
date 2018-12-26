namespace Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders
{
  using System;
  using Sitecore;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   Gives a support for injection of known types into model fields.
  /// </summary>
  public interface IPrimitiveEntityReader
  {
    [NotNull]
    Type SupportedType { get; }

    object Read(ClrObject obj, [NotNull] string fldName);
  }
}
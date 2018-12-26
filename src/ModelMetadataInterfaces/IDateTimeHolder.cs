namespace Sitecore.MemoryDiagnostics
{
  using System;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  /// <summary>
  /// Indicates that <see cref="IClrObjMappingModel"/> has <see cref="DateTime"/> metadata.
  /// </summary>
  public interface IDateTimeHolder
  {
    /// <summary>
    /// Gets the datetime.
    /// </summary>
    /// <value>
    /// The datetime.
    /// </value>
    DateTime Datetime { get; }
  }
}
namespace Sitecore.MemoryDiagnostics.Attributes
{
  using System;

  /// <summary>
  /// Indicates assembly has memory-analysis related logic.
  /// </summary>
  /// <seealso cref="System.Attribute" />
  [Serializable]
  [AttributeUsage(AttributeTargets.Assembly)]
  public sealed class CarriesMemoryDumpAnalysisLogic : Attribute
  {
  }
}
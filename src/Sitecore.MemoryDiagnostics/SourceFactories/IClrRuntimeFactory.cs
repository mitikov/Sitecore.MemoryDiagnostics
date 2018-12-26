namespace Sitecore.MemoryDiagnostics.SourceFactories
{
  using ConnectionDetails;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;

  /// <summary>
  /// Builds <see cref="ClrRuntime"/> from provided connection details.
  /// </summary>
  public interface IClrRuntimeFactory
  {
    /// <summary>
    /// Builds the <see cref="ClrRuntime"/> using provided connections.
    /// <para>Must not return empty objects.</para>
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns></returns>
    [NotNull]
    ClrRuntime BuildClrRuntime([NotNull] IMemoryDumpConnectionPath settings);
  }
}
namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators.LiveObjects
{
  using System.Collections.Generic;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;

  /// <summary>
  /// Provides addresses of all alive objects from heap.
  /// <para>Allows to find which objects would survive after the GC.</para>
  /// </summary>
  public interface IGetAliveObjects
  {
    /// <summary>
    /// Gets addresses of all alive objects from heap, thereby allows to find which objects would survive after the GC.
    /// </summary>
    /// <param name="runtime">The runtime.</param>
    /// <returns>Addresses of all rooted objects found in heap.</returns>
    HashSet<ulong> GetAliveObjects([NotNull] ClrRuntime runtime);
  }
}
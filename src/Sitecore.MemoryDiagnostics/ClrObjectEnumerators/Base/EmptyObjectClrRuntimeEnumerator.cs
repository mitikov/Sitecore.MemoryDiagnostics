namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators.Base
{
  using System.Collections.Generic;
  using Microsoft.Diagnostics.Runtime;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Returns an empty enumerator (<see cref="EnumerateObjectsFromSource"/> API).
  /// </summary>
  /// <seealso cref="ClrObjectEnumerators.IEnumerateClrObjectsFromClrRuntime" />
  public sealed class EmptyObjectClrRuntimeEnumerator : IEnumerateClrObjectsFromClrRuntime
  {
    /// <summary>
    /// A Single instance of state-less object.
    /// </summary>
    public static EmptyObjectClrRuntimeEnumerator Instance = new EmptyObjectClrRuntimeEnumerator();

    /// <summary>
    /// Prevents instances of <see cref="EmptyObjectClrRuntimeEnumerator"/> state-less object from being created.
    /// </summary>
    private EmptyObjectClrRuntimeEnumerator()
    {
    }

    /// <summary>
    /// An empty sequence of objects from given runtime.
    /// </summary>
    /// <param name="runtime">The runtime to extract objects from. Implementation ignores the parameter.</param>
    /// <returns>An emtpy sequence.</returns>
    public IEnumerable<ClrObject> EnumerateObjectsFromSource(ClrRuntime runtime)
    {
      yield break;
    }
  }
}
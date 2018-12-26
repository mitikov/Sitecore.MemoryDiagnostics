namespace Sitecore.MemoryDiagnostics.Facades.ObjectEnumeration
{
  using Sitecore.MemoryDiagnostics.ClrObjectEnumerators.Base;
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders;

  /// <summary>
  /// Carries an empty enumerator.
  /// An attempt to call <see cref="DefaultObjectEnumerationFacade.ExtractFromRuntime"/> would result in empty sequence.
  /// </summary>
  /// <seealso cref="Facades.ObjectEnumeration.DefaultObjectEnumerationFacade" />
  public class EmptyEnumeratorFacade : DefaultObjectEnumerationFacade
  {
    /// <summary>
    /// Initializes a new instance of <see cref="EmptyEnumeratorFacade"/>.
    /// <para>An attempt to call <see cref="DefaultObjectEnumerationFacade.ExtractFromRuntime"/> would result in empty sequence.</para>
    /// </summary>
    public EmptyEnumeratorFacade() : base(EmptyObjectClrRuntimeEnumerator.Instance, NullFilterClrObjectProvider.Instance)
    {
    }
  }
}
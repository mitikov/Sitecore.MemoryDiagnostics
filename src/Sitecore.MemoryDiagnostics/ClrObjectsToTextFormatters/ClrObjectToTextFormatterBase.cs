namespace Sitecore.MemoryDiagnostics.ClrObjectsToTextFormatters
{
  using Sitecore;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Transforms <see cref="ClrObject"/> into text view.
  /// <para>Good for primitive types when formatting to <see cref="string"/> is straightforward.</para>
  /// </summary>
  public abstract class ClrObjectToTextFormatterBase
  {
    public virtual string CouldNotConvertText => "[Could not convert]";

    /// <summary>
    /// Tries to convert primitive objects to text representation.
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="text"></param>
    /// <returns></returns>    
    public abstract bool TryToText(ClrObject clrObject, [NotNull] out string text);
  }
}
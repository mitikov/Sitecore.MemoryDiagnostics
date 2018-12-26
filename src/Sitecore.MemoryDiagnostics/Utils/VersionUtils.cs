namespace Sitecore.MemoryDiagnostics.Utils
{
  using System.Runtime;
  using System.Runtime.CompilerServices;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;
  using Sitecore.Data;

  public static class VersionUtils
  {
    /// <summary>
    /// Shifts the hash code for 21 character.
    /// <para>Hashcode for integer is integer itself.</para>
    /// <para><see cref="Version"/> hashcode is hashcode of underlying integer, so version number itself.</para>
    /// </summary>
    /// <param name="number">The number.</param>
    /// <returns>Integer shifted to 21 bit</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static int ShiftHashCode(int number)
    {
      return number << 21;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static int ShiftHashCode(Version version)
    {
      return version == null ? 0 : ShiftHashCode(version.Number);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static int ShiftHashCode(VersionModel version)
    {
      return version == null ? 0 : ShiftHashCode(version._number);
    }
  }
}
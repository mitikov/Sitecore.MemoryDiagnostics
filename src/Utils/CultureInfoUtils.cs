namespace Sitecore.MemoryDiagnostics.Utils
{
  using System.Runtime;
  using System.Runtime.CompilerServices;

  public static class CultureInfoUtils
  {
    private const char Hyphen = '-';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [TargetedPatchingOptOut("Performance critical")]
    public static int FastHash(string name)
    {
      int hash = 0;
      if (string.IsNullOrEmpty(name))
      {
        return hash;
      }

      for (int index = 0; index < name.Length; index++)
      {
        var ch = name[index];

        if (ch == Hyphen)
        {
          continue;
        }

        if (ch > 90) // 65- A, 90 Uppercase Z, higher codes for lower case a-z. 
        {
          ch = (char)(ch - 32);
        }

        hash += ch - 64;

        // 28 characters in total, so round to 32 = 2^5 
        hash = hash << 5;
      }

      return hash;
    }
  }
}
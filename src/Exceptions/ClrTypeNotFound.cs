namespace Sitecore.MemoryDiagnostics.Exceptions
{
  using System;

  [Serializable]
  public class ClrTypeNotFound : Exception
  {
    public readonly string NotFoundTypeName;

    public ClrTypeNotFound(string typeName)
    {
      NotFoundTypeName = typeName;
    }
  }
}
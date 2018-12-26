namespace Sitecore.MemoryDiagnostics.Exceptions
{
  using System;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.StringExtensions;

  [Serializable]
  public class AttributeMissingException : Exception
  {
    public readonly string AttributeTypeName;
    public readonly string TypeNameFullName;

    public AttributeMissingException([NotNull] Type candidate, [NotNull] Type
      attribureType)
    {
      Assert.ArgumentNotNull(candidate, "candidate");
      Assert.ArgumentNotNull(attribureType, "attribute");
      TypeNameFullName = candidate.FullName;
      AttributeTypeName = attribureType.FullName;
    }

    public AttributeMissingException([NotNull] string candidateTypeName, [NotNull] string attributeTypeName)
    {
      Assert.ArgumentNotNull(candidateTypeName, "candidate");
      Assert.ArgumentNotNull(attributeTypeName, "attribute");
      TypeNameFullName = candidateTypeName;
      AttributeTypeName = attributeTypeName;
    }

    public override string Message
    {
      get
      {
        return "{0} type does not have {1} attribute assigned.".FormatWith(TypeNameFullName, AttributeTypeName);
      }
    }
  }
}
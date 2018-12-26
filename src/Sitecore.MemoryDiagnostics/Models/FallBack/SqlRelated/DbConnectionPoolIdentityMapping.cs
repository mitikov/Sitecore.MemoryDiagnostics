namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping("System.Data.ProviderBase.DbConnectionPoolIdentity")]
  public class DbConnectionPoolIdentityMapping : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public readonly int _hashCode;

    [InjectFieldValue]
    public bool _isNetwork;

    public override int GetHashCode()
    {
      return _hashCode;
    }
  }
}
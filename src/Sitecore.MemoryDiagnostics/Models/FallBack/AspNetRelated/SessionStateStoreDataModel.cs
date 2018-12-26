namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  using Attributes;
  using BaseMappingModel;
  using SessionStateStoreData = System.Web.SessionState.SessionStateStoreData;

  /// <summary>
  /// Defines the field to inject for <see cref="SessionStateStoreData"/> class.
  /// <para>Class layout <see cref="!:https://referencesource.microsoft.com/#System.Web/State/ISessionStateStore.cs,551688556bd45e2a"/></para> 
  /// </summary>
  /// <seealso cref="Sitecore.MemoryDiagnostics.Models.BaseMappingModel.ClrObjectMappingModel" />
  [ModelMapping(typeof(SessionStateStoreData))]
  public class SessionStateStoreDataModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public IClrObjMappingModel _sessionItems;

    [InjectFieldValue]
    public int _timeout;
  }
}
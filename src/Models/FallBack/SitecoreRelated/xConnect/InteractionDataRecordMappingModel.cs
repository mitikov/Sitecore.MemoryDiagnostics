using System;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated.xConnect
{
  [ModelMapping("Sitecore.Xdb.Collection.Model.InteractionDataRecord")]
  public class InteractionDataRecordMappingModel: ClrObjectMappingModel
  {
    [InjectFieldValue]
    public DateTime _lastModified;

    [InjectFieldValue]
    public DateTime _created;

    [InjectFieldValue]
    public Guid Id;

    [InjectFieldValue]
    public Guid ChannelId;

    [InjectFieldValue]
    public Guid ContactId;

    [InjectFieldValue]
    protected IClrObjMappingModel _events;

    public ArrayMappingModel Events => _events as ArrayMappingModel;
  }
}

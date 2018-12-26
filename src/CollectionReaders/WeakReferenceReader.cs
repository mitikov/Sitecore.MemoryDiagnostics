namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Microsoft.Diagnostics.Runtime;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;
  using Sitecore.StringExtensions;
  using System.Diagnostics;

  public class WeakReferenceReader : CollectionReaderBase
  {
    public static ClrObject GetTargetObject(ClrObject weakRef)
    {
      ulong mHandleAddr = weakRef.Type.Fields[0].GetAddress(weakRef.Address);

      ClrType intPtrType = weakRef.Type.Fields[0].Type;

      var ptrToObj = (ulong)intPtrType.Fields[0].GetValue(mHandleAddr, true);

      ulong weakRefTarget;

      bool success = weakRef.Type.Heap.ReadPointer(ptrToObj, out weakRefTarget);

      if ((!success) || (weakRefTarget == 0))
        return ClrObject.NullObject;

      var clrObj = new ClrObject(weakRefTarget, weakRef.Type.Heap);
      return clrObj.Type == null ? ClrObject.NullObject : clrObj;
    }

    public override IClrObjMappingModel ReadEntity(ClrObject obj, ModelMapperFactory factory)
    {
      ClrObject target = GetTargetObject(obj);
#if TRACE
      Trace.TraceInformation($"{obj.HexAddress} WeakRef points on {target.HexAddress} with {target.Type?.Name ?? "no"} type");
#endif

      return factory.BuildModel(target);
    }

    public override bool SupportTransformation(ClrObject obj)
    {
      return obj.Type.Name.StartsWith(typeof(WeakReference).FullName, StringComparison.Ordinal);
    }
  }
}
namespace Sitecore.MemoryDiagnostics.CollectionReaders
{
  using System;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;
  using System.Linq;

  public class EventHandlerReader : CollectionReaderBase
  {
    [NotNull]
    public override IClrObjMappingModel ReadEntity(ClrObject obj, [NotNull] ModelMapperFactory factory)
    {
      Assert.ArgumentNotNull(factory, "factory");

      if (obj.IsNullObj)
        return EmptyClrObjectModel.Instance;

      var result = new EventHandlerModel
      {
        Obj = obj,
        target = obj.GetRefFld("_target")
      };

      ClrObject invocationList = obj.GetRefFld(@"_invocationList");

      if (invocationList.IsNullObj)
      {
        // Not multicast delegate
        ClrMethod methodDescriptor;

        if (!TryReadMethodPtr(obj, out methodDescriptor))
        {
          if (!TryResolveMethodFromTarget(obj, out methodDescriptor))
           return result;
        }

        result.Info = methodDescriptor;
        return result;
      }

      // Multicast Delegate
      // TODO: Consider changing ArrayMapping
      result.InvokationList = factory.BuildModel(invocationList) as ArrayMappingModel;

      return result;
    }

    private bool TryResolveMethodFromTarget(ClrObject obj, out ClrMethod methodDescriptor)
    {
      methodDescriptor = null;
      var target = obj.GetRefFld("_target");
      var _methodPtr = (ulong)(long)obj.GetSimpleFld<object>("_methodPtr");
      // TODO: Handle empty types types ?
      if (_methodPtr==0 || target.IsNullObj || target.Type == null)
      {
        return false;
      }
      methodDescriptor = target.Type.Methods.FirstOrDefault(methodInfo => methodInfo.NativeCode == _methodPtr);

      return methodDescriptor != null;

    }

    public override bool SupportTransformation(ClrObject obj)
    {
      return base.SupportTransformation(obj) && obj.Type.Name.Contains(@"System.EventHandler");
    }

    [NotNull]
    private byte[] ReadBytes(ulong methodPtr, [NotNull] ClrRuntime runtime, int countToRead)
    {
      Assert.ArgumentNotNull(runtime, "runtime");

      // TODO: AddOrMerge buffer reuse;
      var bytes = new byte[countToRead];
      int readCount;
      runtime.ReadMemory(methodPtr, bytes, countToRead, out readCount);
      return bytes;
    }

    private bool TryProcessNearJump(ulong methodPtr, [NotNull] ClrRuntime runtime,
      [CanBeNull] out ClrMethod methodDescriptor)
    {
      Assert.ArgumentNotNull(runtime, "runtime");
      try
      {
        byte[] bytes = ReadBytes(methodPtr + 1, runtime, 4);

        uint offset = 0;
        for (int i = bytes.Length; i > 0; i--)
        {
          offset = offset * 256;
          offset += bytes[i - 1];
        }

        ulong res = methodPtr + 5 + offset;
        methodDescriptor = runtime.GetMethodByAddress(res);
        return true;
      }
      catch (Exception)
      {
        methodDescriptor = null;
        return false;
      }
    }

    private bool TryReadMethodPtr(ClrObject obj, [CanBeNull] out ClrMethod methodDescriptor)
    {
      methodDescriptor = null;
      ClrRuntime rn = obj.Type.Heap.Runtime;
      try
      {
        var _methodPtr = (ulong)(long)obj.GetSimpleFld<object>("_methodPtr");
        if (_methodPtr == 0)
          return false;
        byte[] commandOpCode = ReadBytes(_methodPtr, rn, 1);

        return (commandOpCode[0] == 0xE9) && TryProcessNearJump(_methodPtr, rn, out methodDescriptor) &&
               (methodDescriptor != null);
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
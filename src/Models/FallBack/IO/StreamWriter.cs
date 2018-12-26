using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.IO
{
  [ModelMapping(typeof(System.IO.StreamWriter))]
  public class StreamWriterModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool autoFlush;

    [InjectFieldValue]
    protected ArrayMappingModel byteBuffer;

    public override void Compute()
    {
      base.Compute();

      var byteBuf = Obj.GetRefFld("byteBuffer");
      var length = byteBuf.Type.GetArrayLength(byteBuf.Address);

      var chars = from i in Enumerable.Range(0, length)
                  let byteArrayType = byteBuf.Type
                  let reference = byteArrayType.GetArrayElementAddress(byteBuf.Address, i)
                  where reference != default(ulong)
                  let arrayElemType = byteArrayType.ComponentType
                  let elem = arrayElemType.GetValue(reference)
                  let letter = (byte)elem
                  where letter != 0
                  select letter;


      InMemoryContent = Encoding.UTF8.GetString(chars.ToArray());
    }

    public string InMemoryContent { get; private set; } = string.Empty;
  }
}

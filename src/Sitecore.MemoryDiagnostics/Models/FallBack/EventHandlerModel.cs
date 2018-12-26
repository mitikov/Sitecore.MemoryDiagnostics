
namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Diagnostics;
  using Microsoft.Diagnostics.Runtime;

  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using Sitecore.StringExtensions;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  using ArrayList = System.Collections.ArrayList;
  using StringBuilder = System.Text.StringBuilder;

  [DebuggerDisplay("{GetType().Name} {MethodSignature} [ObjAdr] {Obj.Address}")]
  public class EventHandlerModel : ClrObjectMappingModel, IEnumerable<IClrObjMappingModel>
  {
    public ClrMethod Info;

    public ArrayMappingModel InvokationList;

    public ClrObject target;

    private ArrayList _recursion;

    public bool IsEmpty => (this.Info == null) && (!this.IsMulticast);

    public bool IsMulticast => !ArrayMappingModel.ArrayIsNullOrEmpty(this.InvokationList);

    public string MethodSignature
    {
      get
      {
        try
        {
          if (this.recursion.Contains(this))
          {
            return RecursionDetectedModel.RecursionText;
          }

          this.recursion.Add(this);

          return this.GetMethodSignature();
        }
        finally
        {
          this.recursion.Remove(this);
        }
      }
    }

    private string EmptyDelegateText
    {
      get
      {
        return Obj.IsNullObj ? EmptyClrObjectModel.EmptyModelText : $"{HexAddress} delegate is empty";
      }
    }

    private ArrayList recursion
    {
      get
      {
        return _recursion ?? (_recursion = new ArrayList());
      }
    }

    public IEnumerator<IClrObjMappingModel> GetEnumerator()
    {
      return (InvokationList as IEnumerable<IClrObjMappingModel>).GetEnumerator();
    }

    public override string ToString() => this.MethodSignature;

    [NotNull]
    protected virtual string GetMethodSignature()
    {
      if (this.Info != null)
      {
        // Plain delegate
        return this.Info.GetFullSignature();
      }

      // Empty delegate
      if (this.IsEmpty)
      {
        return this.EmptyDelegateText;
      }

      if (this.InvokationList == null)
      {
        return "[Improve EventHandler reading]";
      }

      // Multicast delegate
      // 50 - avg length of method descriptor.
      var sb = new StringBuilder(this.InvokationList.Count * 50);

      foreach (IClrObjMappingModel elem in InvokationList.Elements)
      {
        sb.AppendLine(elem.ToString());
      }

      return sb.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (this as IEnumerable<IClrObjMappingModel>).GetEnumerator();
    }
  }
}
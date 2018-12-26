namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  using System;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelMetadataInterfaces;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;

  [ModelMapping(typeof(Thread))]
  public class ThreadMappingModel : ClrObjectMappingModel, IEquatable<ThreadMappingModel>, ICaptionHolder
  {
    [InjectFieldValue]
    public int m_ManagedThreadId = -1;

    [InjectFieldValue]
    public string m_Name;

    [InjectFieldValue]
    public int m_Priority;

    public ClrThread Thread;

    public string Caption => $"[ThreadMapping] Managed #{this.m_ManagedThreadId} {this.m_Name}";

    [CanBeNull]
    public ClrException Exception => this.Thread?.CurrentException;

    public string StackTrace
    {
      get
      {
        if (this.Thread?.StackTrace == null)
        {
          return "[No Stack Trace]";
        }

        var sb = new StringBuilder();

        ulong? stackPointer = null;

        foreach (ClrStackFrame clrStackFrame in this.Thread.StackTrace)
        {
          if (!stackPointer.HasValue)
          {
            stackPointer = clrStackFrame.StackPointer;
          }
          else
          {
            if (stackPointer == clrStackFrame.StackPointer)
            {
              // Infinite loop.
              break;
            }
          }

          stackPointer = clrStackFrame.StackPointer;

          sb.AppendLine(clrStackFrame.StackPointer.ToString("x8") + '\t' + clrStackFrame.InstructionPointer.ToString("x8") + '\t' + clrStackFrame);
        }

        return sb.ToString();
      }
    }

    public override void Compute()
    {
      if (this.m_ManagedThreadId == -1)
      {
        return;
      }

      this.Thread = this.Obj.Type.Heap.Runtime.Threads.FirstOrDefault(thread => thread.ManagedThreadId == this.m_ManagedThreadId);
    }

    public bool Equals([CanBeNull] ThreadMappingModel other)
    {
      if (other == null)
      {
        return false;
      }

      if ((other.Thread == null) && (this.Thread == null))
      {
        return true;
      }

      if ((other.Thread == null) || (this.Thread == null))
      {
        return false;
      }

      return other.Thread.Address.Equals(this.Thread.Address);
    }

    public override string ToString() => $"Managed Thread #{this.m_ManagedThreadId} priority {this.m_Priority}{Environment.NewLine}{this.StackTrace}";
  }
}
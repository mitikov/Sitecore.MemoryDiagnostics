using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.Diagnostics.Runtime;
using Sitecore.MemoryDiagnostics.Interfaces;

namespace Sitecore.MemoryDiagnostics.Helpers
{
  /// <summary>
  /// Provides only alive user threads from <see cref="ClrRuntime"/>.
  /// <para>Filters out system threads (like <see cref="GC"/>, finalizer, debugger helper and others).</para>
  /// </summary>
  public class UserThreadsFilter:IFilter<ClrThread>
  {
    /// <summary>
    /// A single instance of state-less object.
    /// </summary>
    public static readonly UserThreadsFilter Instance = new UserThreadsFilter();

    #region Properties 
    /// <summary>
    /// Checks if thread is system: either <see cref="ThreadBase.IsGC"/>, Finalizer, DebuggerHelper, or <see cref="ThreadBase.IsShutdownHelper"/>.
    /// </summary>
    /// <param name="thread"></param>
    /// <returns></returns>
    public virtual bool IsSystemThread(ClrThread thread) => thread.IsGC || thread.IsFinalizer || thread.IsDebuggerHelper || thread.IsShutdownHelper;

    #endregion

    /// <summary>
    /// Extracts alive user threads from runtime.
    /// </summary>
    /// <param name="runtime"></param>
    /// <returns></returns>
    public virtual IEnumerable<ClrThread> ExtactAliveUserThreads([NotNull]ClrRuntime runtime)
    {      
      Contract.Requires(runtime != null, "runtime");
      return runtime.Threads.Where(ShouldProcess);
    }

    /// <summary>
    /// Checks if <see cref="ClrThread"/> should not be skipped.
    /// </summary>
    /// <param name="thread"></param>
    /// <returns></returns>
    public bool ShouldProcess([CanBeNull] ClrThread thread)
    {
      return !ShouldSkip(thread);
    }

    /// <summary>
    /// Skips system, and non-alive user threads.
    /// </summary>
    /// <param name="thread"></param>
    /// <returns></returns>
    public virtual bool ShouldSkip(ClrThread thread)
    {
      if ((thread == null) || this.IsSystemThread(thread))
      {
        return true;
      }

      return !thread.IsAlive;
    }

    bool IFilter<ClrThread>.Matches(ClrThread tested)
    {
      return this.ShouldProcess(tested);
    }
  }
}

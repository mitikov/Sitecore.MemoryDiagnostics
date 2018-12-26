namespace Sitecore.MemoryDiagnostics.Disablers
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using Sitecore.Diagnostics;

  public abstract class ThreadStaticDisablerBase<T> : IDisposable
  {
    [ThreadStatic]
    private static Stack<bool> _state;

    private int _disposed;

    protected ThreadStaticDisablerBase()
    {
      if (_state == null)
        _state = new Stack<bool>();

      _state.Push(true);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
    /// </value>
    public static bool IsActive
    {
      get
      {
        return (_state != null) && (_state.Count > 0) && _state.Peek();
      }
    }

    public void Dispose()
    {
      bool notYetDisposed = Interlocked.CompareExchange(ref _disposed, 1, 0) == 0;
      if (notYetDisposed)
        Exit();
    }

    private void Exit()
    {
      Assert.IsTrue((_state != null) && (_state.Count > 0), "Stack is null or empty.");
      _state.Pop();
    }
  }
}
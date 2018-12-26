namespace Sitecore.MemoryDiagnostics.Helpers
{
  using System;
  using System.Collections;
  using Exceptions;

  /// <summary>
  /// Prevents recursive operations by specifying unique <see cref="toTrack"/> number.
  /// <para>once operation is over, <see cref="toTrack"/> number is taken from processing list <see cref="beingTracked"/>.</para>
  /// </summary>
  /// <seealso cref="System.IDisposable" />
  public class RecursionHelper : IDisposable
  {
    private readonly ulong toTrack;

    private ArrayList beingTracked;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecursionHelper"/> class.
    /// </summary>
    /// <param name="toTrack">To track.</param>
    /// <param name="beingTracked">The being processed.</param>
    /// <exception cref="RecursionException"> In case same <paramref name="toTrack"/> is already in <see cref="beingTracked"/> collection.</exception>
    public RecursionHelper(ulong toTrack, [NotNull] ArrayList beingTracked)
    {
      this.toTrack = toTrack;
      this.beingTracked = beingTracked;

      /* TODO: Do we want to switch to HashSet : 
       * http://stackoverflow.com/questions/150750/hashset-vs-list-performance
       */

      lock (beingTracked.SyncRoot)
      {
        if (beingTracked.Contains(toTrack))
        {
          throw new RecursionException();
        }

        beingTracked.Add(this.toTrack);
      }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (this.beingTracked == null)
      {
        // already disposed.
        return;        
      }

      lock (this.beingTracked.SyncRoot)
      {
        this.beingTracked.Remove(this.toTrack);
      }

      this.beingTracked = null;
    }
  }
}
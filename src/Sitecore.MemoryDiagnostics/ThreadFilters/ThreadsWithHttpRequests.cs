using System.Linq;
using Microsoft.Diagnostics.Runtime;
using Sitecore.MemoryDiagnostics.Helpers;

namespace Sitecore.MemoryDiagnostics.ThreadFilters
{
  public class ThreadsWithHttpRequests: UserThreadsFilter
  {
    protected virtual string ProcessRequestNotificationMethodName => "System.Web.HttpRuntime.ProcessRequestNotificationPrivate";

    public override bool ShouldSkip(ClrThread thread)
    {
      if (base.ShouldSkip(thread))
      {
        return true;
      }

      var hasRequestNotification = thread.StackTrace.Any(frame => frame?.DisplayString?.Contains(this.ProcessRequestNotificationMethodName) == true);

      return !hasRequestNotification;
    }
  }
}

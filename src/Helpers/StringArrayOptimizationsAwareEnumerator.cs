using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SitecoreMemoryInspectionKit.Core.ClrHelpers;

namespace Sitecore.MemoryDiagnostics.Helpers
{
  public class StringArrayOptimizationsAwareEnumerator : EnumerationHelper
  {

    protected override ulong CalculateArrayStart(ClrObject clrObject, int pointerSize)
    {
      if (clrObject.Type.ComponentType.ElementType == Microsoft.Diagnostics.Runtime.ClrElementType.String)
      {
        return (ulong)pointerSize + clrObject.Address; 
      }
      else
      {
        return base.CalculateArrayStart(clrObject, pointerSize);
      }
      
    }
  }
}

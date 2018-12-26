using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.MemoryDiagnostics.Interfaces
{
  /// <summary>
  /// Allows to test objects if they match criteria or not.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IFilter<T>
  {
    bool Matches(T tested);
  }
}

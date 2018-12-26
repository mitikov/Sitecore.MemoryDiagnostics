using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD.July.Models.FallBack.SqlRelated
{
  using MD.July.Attributes;
  using MD.July.Models.BaseMappingModel;

  [ModelMapping(@"System.Data.SqlClient.TdsParserStateObject")]
  public class TdsParserStateObjectModelMapping:ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool _reading;

    [InjectFieldValue]
    public int _inBytesRead;

    [InjectFieldValue]
    public long _timeoutTime;

    [InjectFieldValue]
    public bool _hasErrorOrWarning;

  }
}

using ParameterDirection = System.Data.ParameterDirection;
using SqlParameter = System.Data.SqlClient.SqlParameter;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated
{
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ModelFactory;
  using Sitecore.MemoryDiagnostics.ModelMetadataInterfaces;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  [ModelMapping(typeof(SqlParameter))]
  public class SqlParameterMappingModel : ClrObjectMappingModel, ICaptionHolder
  {
    [InjectFieldValue]
    public ParameterDirection _direction;

    [InjectFieldValue]
    public string _parameterName;

    [InjectFieldValue]
    public int _size;

    [InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> _value;

    public string Caption
    {
      get
      {
        return $"{ParameterName}#{Value}";
      }
    }


    public string ParameterName
    {
      get
      {
        return _parameterName ?? "[NoParameterName]";
      }
    }

    /// <summary>
    /// Provides the text representation of the <see cref="_value"/>.
    /// </summary>
    public string Value
    {
      get
      {
        var innerValue = _value?.Value;
        if (innerValue == null)
        {
          // TODO: Consider change some day.
          return TextConstants.NullReferenceString;
        }
        else if (ClrObjectsToTextFormatters.ClrObjectToTextFormatter.Instance.TryToText(innerValue.Obj, out string
        text))
        {
          return text;
        }
        else
        {
          return ClrObjectsToTextFormatters.ClrObjectToTextFormatter.Instance.CouldNotConvertText;
        }
      }
    }

    public override string ToString() => $"{ParameterName}#{Value} ({HexAddress}) is {_direction} with {_size} size.";
  }
}
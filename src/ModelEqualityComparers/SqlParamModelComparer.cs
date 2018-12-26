namespace Sitecore.MemoryDiagnostics.ModelEqualityComparers
{
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SqlRelated;

  public class SqlParamModelComparer : IEqualityComparer<SqlParameterMappingModel>, IEqualityComparer<IClrObjMappingModel>
  {
    public bool Equals(SqlParameterMappingModel left, SqlParameterMappingModel right)
    {
      if (ReferenceEquals(left, right))
      {
        return true;
      }

      if (((left == null) || (right == null))
          || !string.Equals(left._parameterName, right._parameterName))
      {
        return false;
      }

      var leftValue = left.Value;

      var rightValue = right.Value;

      return string.Equals(leftValue, rightValue);
    }

    public int GetHashCode(SqlParameterMappingModel obj)
    {
      if (obj == null)
      {
        return 0;
      }

      return obj.ParameterName.GetHashCode();
    }

    bool IEqualityComparer<IClrObjMappingModel>.Equals(IClrObjMappingModel x, IClrObjMappingModel y)
    {
      return Equals(x as SqlParameterMappingModel, y as SqlParameterMappingModel);
    }

    int IEqualityComparer<IClrObjMappingModel>.GetHashCode(IClrObjMappingModel obj)
    {
      return GetHashCode(obj as SqlParameterMappingModel);
    }
  }
}
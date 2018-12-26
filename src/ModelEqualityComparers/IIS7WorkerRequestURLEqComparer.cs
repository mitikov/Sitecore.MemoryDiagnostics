namespace Sitecore.MemoryDiagnostics.ModelEqualityComparers
{
  using System;
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated;

  public class IIS7WorkerRequestURLEqComparer : IEqualityComparer<IIS7WorkerRequestModel>, IEqualityComparer<IClrObjMappingModel>
  {
    public bool Equals(IIS7WorkerRequestModel x, IIS7WorkerRequestModel y)
    {
      if (ReferenceEquals(x, y))
        return true;
      if ((x == null) || (y == null))
        return false;
      return string.Equals(x._cacheUrl, y._cacheUrl, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(IClrObjMappingModel x, IClrObjMappingModel y)
    {
      return Equals(x as IIS7WorkerRequestModel, y as IIS7WorkerRequestModel);
    }

    public int GetHashCode(IIS7WorkerRequestModel obj)
    {
      return obj == null ? 0 : obj._cacheUrl.FallbackTo("[NoUrl]").GetHashCode();
    }

    public int GetHashCode(IClrObjMappingModel obj)
    {
      return this.GetHashCode(obj as IIS7WorkerRequestModel);
    }
  }
}
namespace Sitecore.MemoryDiagnostics.ModelFactory
{
  using System;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.Diagnostics;
  
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class LazyLoad< T> where T : IClrObjMappingModel
  {
    private readonly Func<ClrObject, T> _calcFunc;

    private readonly ClrObject _obj;
    private bool _calced = false;
    private T _value;

    public LazyLoad([NotNull] Func<ClrObject, T> calcFunc, ClrObject obj)
    {
      Assert.ArgumentNotNull(calcFunc, "Invalid function passed for lazy load.");

      _calcFunc = calcFunc;
      _obj = obj;
    }

    public T Value
    {
      get
      {
        if (!_calced)
        {
          try
          {
            _value = _calcFunc(_obj);
          }
          catch (Exception)
          {
            // TODO: think about correct scenario;
            throw;
          }
          finally
          {
            _calced = true;
          }
        }

        return _value;
      }
    }

    public static explicit operator T([CanBeNull] LazyLoad<T> lazy)
    {
      return lazy == null ? default(T) : lazy.Value;
    }
  }
}
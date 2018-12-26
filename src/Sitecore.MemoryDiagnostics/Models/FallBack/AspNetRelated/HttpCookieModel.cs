namespace Sitecore.MemoryDiagnostics.Models.FallBack.AspNetRelated
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Web;

  using Attributes;
  using BaseMappingModel;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  [DebuggerDisplay("{Name}:{Value}, IsSecure {_secure}. {GetType().Name} {Obj.Address}")]
  [ModelMapping(typeof(HttpCookie))]
  public class HttpCookieModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public bool _added;

    [InjectFieldValue]
    public bool _changed;

    [InjectFieldValue]
    public string _name;

    [InjectFieldValue]
    public string _path;

    [InjectFieldValue]
    public bool _secure;

    public string Value;

    public override string Caption => $"{this.Name} : {this.Value}";

    public string Name => this._name;

    public override void Compute()
    {
      ClrAssert.ObjectNotNullTypeNotEmpty(this.Obj);

      this.Value = Obj.GetStringFld(fieldName: "_stringValue") ?? string.Empty;

      ClrObject multi = Obj.GetRefFld("_multiValue");
      if (multi.IsNullObj)
      {
        return;
      }

      ClrObject arrayList = multi.GetRefFld("_entriesArray");
      if (arrayList.IsNullObj)
      {
        return;
      }

      List<ClrObject> buckets = ClrCollectionHelper.EnumerateArrayList(arrayList);
      foreach (ClrObject bucket in buckets)
      {
        bucket.ReEvaluateType();
        ClrObject val = bucket.GetRefFld("Value");
        val.ReEvaluateType();
        if (val.IsNullObj)
        {
          continue;
        }

        List<ClrObject> nestedValues = ClrCollectionHelper.EnumerateArrayList(val);
        foreach (ClrObject nestedValue in nestedValues)
        {
          nestedValue.ReEvaluateType();
          if (nestedValue.IsNullObj || (nestedValue.Type == null))
          {
            continue;
          }

          object tmpvalue = nestedValue.Type.GetValue(nestedValue.Address);
          if (tmpvalue is string)
          {
            this.Value += tmpvalue;
          }
        }
      }

      // var buckets = ClrCollectionHelper.EnumerateArrayOfRefTypes(arrayList);
      // buckets.
    }

    public override string ToString()
    {
      return Value + " " + base.ToString();
    }
  }
}
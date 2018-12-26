using Environment = System.Environment;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated
{
  using System.Diagnostics;
  using Sitecore.Caching.Generics;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

  /// <summary>
  /// Mapping for <see cref="Cache{T}.CacheEntry"/> class.
  /// </summary>
  [DebuggerDisplay("[Pointer] {Obj.Address} [Model] {GetType().Name}")]
  [ModelMapping("Sitecore.Caching.Generics.Cache+CacheEntry<System.String>")]
  public class CacheEntryModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public object data;

    [InjectFieldValue]
    public object key;

    public override string Caption
    {
      get
      {
        if (key is StringModel)
        {
          return ((StringModel)key).Value;
        }

        return Key;
      }
    }

    public string Key => this.key?.ToString() ?? "[NoCacheEntyKey]";

    public bool KeyIsNull => this.key == null;

    public string ValueString => this.data?.ToString() ?? "[NoData]";

    public override string ToString()
    {
      return this.KeyIsNull ? this.Key : string.Concat("Key -> ", Key, Environment.NewLine, "Value ->", ValueString);
    }
  }    
}
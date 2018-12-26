namespace Sitecore.MemoryDiagnostics.Extensions
{
  using System;
  using System.Linq;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.MemoryDiagnostics.Exceptions;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   <see cref="ClrRuntime" /> extension methods to read static field values
  /// </summary>
  public static class RuntimeExtensions
  {
    public static ClrObject GetStaticFldValue(this ClrRuntime runtime, [NotNull] string typeName,
      [NotNull] params string[] fieldNamesChain)
    {
      Assert.ArgumentNotNull(fieldNamesChain, "no field names are specified");
      Assert.IsTrue(fieldNamesChain.Length > 0, "En empty array is passed");
      var startingPoint = GetStaticFldValue(runtime, typeName, fieldNamesChain[0]);
      if (startingPoint.IsNullObj || (fieldNamesChain.Length == 1))
      {
        return startingPoint;
      }

      var leftFieldNamesToFollow = new string[fieldNamesChain.Length - 1];

      Array.Copy(fieldNamesChain, 1, leftFieldNamesToFollow, 0, fieldNamesChain.Length - 2);
      var val = startingPoint.GetRefFldChained(leftFieldNamesToFollow);

      return val.HasValue ? val.Value : ClrObject.NullObject;
    }

    /// <summary>
    /// Gets the static field value.
    /// <para> WARN - resolves value for single domain only.</para>
    /// </summary>
    /// <param name="runtime">The runtime.</param>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns></returns>
    /// <exception cref="ClrTypeNotFound"></exception>
    public static ClrObject GetStaticFldValue(this ClrRuntime runtime, [NotNull] string typeName,
      [NotNull] string fieldName)
    {
      Assert.ArgumentNotNullOrEmpty(typeName, "typeName");
      Assert.ArgumentNotNullOrEmpty(fieldName, "fieldName");
      ClrHeap heap = runtime.GetHeap();

      Assert.ResultNotNull(heap, "Could not fetch heap from runtime");

      ClrType type = heap.GetTypeByName(typeName);
      if (type == null)
      {
        throw new ClrTypeNotFound(typeName);
      }

      return runtime.GetStaticFldValue(type, fieldName);
    }

    [NotNull]
    public static ClrObject GetStaticFldValue(this ClrRuntime runtime, [NotNull] ClrType clrType,
      [NotNull] string fieldName)
    {
      Assert.ArgumentNotNull(clrType, "typeName");
      Assert.ArgumentNotNullOrEmpty(fieldName, "fieldName");

      ClrStaticField fld = clrType.GetStaticFieldByName(fieldName);
      if (fld == null)
      {
        throw new MissingFieldException(clrType.Name, fieldName);
      }

      var domains = runtime.AppDomains.ToArray();
      if ((domains == null) || (domains.Length == 0))
      {
        throw new ArgumentException("No domains are found inside runtime!");
      }

      ulong? resultAddress = (from domain in domains
        where !string.IsNullOrWhiteSpace(domain.Name)
        where !domain.Name.Contains("Default") && fld.IsInitialized(domain)
        select (ulong?)fld.GetValue(domain)).FirstOrDefault();
      return resultAddress.HasValue ? (new ClrObject(resultAddress.Value, clrType.Heap))
        : ClrObject.NullObject;
    }
  }
}
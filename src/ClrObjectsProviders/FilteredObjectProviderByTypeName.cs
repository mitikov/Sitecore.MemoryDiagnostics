namespace Sitecore.MemoryDiagnostics.ClrObjectsProviders
{
  using System;
  using System.Collections.Generic;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.ClrObjectEnumerators;
  using Sitecore.MemoryDiagnostics.ClrObjectsProviders.Base;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Diagnostics;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  ///   Extracts <see cref="ClrObject" /> that DO HAVE type from <see cref="IEnumerateClrObjectsFromClrRuntime" />.
  ///   <remarks>Uses <see cref="HeapBasedClrObjectEnumerator" /> if no other was specified.</remarks>
  /// </summary>
  public class FilteredObjectProviderByTypeName : FilteredObjectsProviderBase
  {
    private ClrType _searchedType;
    private ulong _typeIndex;

    private bool readingInProgress;
    private string searchedTypeName;

    private bool searchedTypeSet;
    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredObjectProviderByTypeName"/> class.
    /// </summary>
    /// <param name="type">Type of the color.</param>
    public FilteredObjectProviderByTypeName([NotNull] Type type)
      : this(type.FullName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredObjectProviderByTypeName"/> class.
    /// </summary>
    /// <param name="clrType">Type of the color.</param>
    public FilteredObjectProviderByTypeName([NotNull] ClrType clrType)
      : this(clrType.Name)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="FilteredObjectProviderByTypeName" /> class.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    public FilteredObjectProviderByTypeName([NotNull] string typeName)
    {
      this.SetSearchedType(typeName);
    }

    /// <summary>
    ///   Gets the type that an object must be based on to match the criteria.
    /// </summary>
    /// <value>
    ///   The type of the checked.
    /// </value>
    public ClrType CheckedType
    {
      get
      {
        return this._searchedType;
      }

      private set
      {
        Assert.ArgumentNotNull(value, "ClrType");
        this._searchedType = value;
        this._typeIndex = value.MetadataToken;
      }
    }

    /// <summary>
    /// Sets the type that objects must be based on to be filtered.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <exception cref="System.MethodAccessException">Extraction of objects from runtime is in progress.</exception>
    public void SetSearchedType([NotNull] string typeName)
    {
      Assert.ArgumentNotNullOrEmpty(typeName, "typeName");
      if (this.readingInProgress)
      {
        throw new MethodAccessException("Extraction of objects from runtime is in progress.");
      }

      this.searchedTypeName = typeName;
      this._searchedType = null;
      searchedTypeSet = false;
    }

    /// <summary>
    /// Applies <see cref="MatchesExtractCriteria(Microsoft.Diagnostics.Runtime.ClrType)"/> API on every object yielded by <see cref="IEnumerateClrObjectsFromClrRuntime" />
    /// </summary>
    /// <param name="ds">The runtime used to provide objects.</param>
    /// <param name="clrClrObjectEnumerator">The ClrObject enumerator.</param>
    /// <returns>A flow of objects that are based on the searched type.</returns>
    [NotNull]
    protected override sealed IEnumerable<ClrObject> ExtractObjectsFromRuntime([CanBeNull] ClrRuntime ds, [NotNull] IEnumerateClrObjectsFromClrRuntime clrClrObjectEnumerator)
    {
      try
      {
        this.readingInProgress = true;
        return this.ExtractObjectsFromRuntimeWrapped(ds, clrClrObjectEnumerator);
      }
      finally
      {
        this.readingInProgress = false;
      }
    }

    [NotNull]
    protected virtual IEnumerable<ClrObject> ExtractObjectsFromRuntimeWrapped([CanBeNull] ClrRuntime runtime, [NotNull] IEnumerateClrObjectsFromClrRuntime clrClrObjectEnumerator)
    {
      if (runtime == null)
      {
        yield break;
      }

      this.InitSearchedType(runtime.GetHeap());

      foreach (ClrObject c in base.ExtractObjectsFromRuntime(runtime, clrClrObjectEnumerator))
      {
        yield return c;
      }
    }

    private void InitSearchedType(ClrHeap heap)
    {
      this._searchedType = heap.GetTypeByName(this.searchedTypeName);

      Assert.Required(_searchedType, $"{searchedTypeName} was not found by name in the heap");

      this._typeIndex = this._searchedType.MetadataToken;

      searchedTypeSet = true;
    }

    /// <summary>
    /// Checks if an type is same as specified in filter.
    /// </summary>
    /// <param name="clrType">The type object must be based on to pass filtering.</param>
    /// <returns><c>true</c> if object has same type as searched;<c>false</c> otherwise.</returns>
    protected virtual bool MatchesExtractCriteria([CanBeNull] ClrType clrType)
    {
      if (clrType == null)
      {
        return false;
      }

      if (!searchedTypeSet)
      {
        this.InitSearchedType(clrType.Heap);
      }

      // -1 is for desktop Array types. . .
      return /*this._typeIndex != -1*/
             // TODO: NMI check.
        clrType.MethodTable.Equals(this._searchedType.MethodTable);

      // : clrType.Name.Equals(this._searchedType.Name, StringComparison.Ordinal);
    }

    /// <summary>
    ///   Checks if an object matches criteria specified by the instance.
    /// </summary>
    /// <param name="clrObj">The color object.</param>
    /// <returns><c>true</c> if object has same type as searched;<c>false</c> otherwise.</returns>
    public override bool MatchesExtractCriteria([CanBeNullObject] ClrObject clrObj)
    {

      return !clrObj.IsNullObj && this.MatchesExtractCriteria(clrObj.Type);
    }
  }
}
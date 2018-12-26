namespace Sitecore.MemoryDiagnostics.ClrObjectEnumerators
{
  using System;
  using System.Collections.Generic;

  using ClrObjectsProviders.Base;
  using Diagnostics;
  using Extensions;
  using Microsoft.Diagnostics.Runtime;
  using ModelFactory.Abstracts;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Resolves static field of given class value by name, and further chain (if necessary).
  /// <para>Passes resulting object to <see cref="IModelMapperFactory"/> and treats output as <see cref="IEnumerable{T}"/> of <see cref="ClrObject"/>.</para>
  ///<see cref="IFilteredObjectsProvider"/> must perform data filtering.
  /// </summary>
  public class StaticFieldValuesEnumerator : IEnumerateClrObjectsFromClrRuntime
  {
    protected readonly string ClassName;

    protected readonly string[] FieldNamesChain;
    /*    
    public StaticFieldValuesEnumerator(IModelMapperFactory modelMapperFactory, [NotNull] string className, [NotNull] string fieldName):this(modelMapperFactory, className, new string[] { fieldName }) 
    {

    }  */
     
    /// <summary>
    /// Initializes a new instance of the <see cref="StaticFieldValuesEnumerator"/> class.
    /// </summary>    
    /// <param name="className">Name of the class.</param>
    /// <param name="fieldNamesChain">The field names.</param>
    public StaticFieldValuesEnumerator( [NotNull] string className, [NotNull] params string[] fieldNamesChain)
    {
      Assert.ArgumentNotNull(className, "className");
      Assert.ArgumentNotNull(fieldNamesChain, "no field names are provided");
      Assert.IsTrue(fieldNamesChain.Length > 0, "An empty array is passed");
      
      this.ClassName = className;
      this.FieldNamesChain = fieldNamesChain;
    }

    /// <summary>
    /// Enumerates objects from source using custom defined logic.
    /// </summary>
    /// <param name="runtime">The data source.</param>
    /// <returns>
    /// NonEmpty ClrObjects.
    /// </returns>
    /// <exception cref="ArgumentException"> in case <paramref name="runtime"/> is null</exception>
    [NotNull]
    public virtual IEnumerable<ClrObject> EnumerateObjectsFromSource([NotNull] ClrRuntime runtime)
    {
      Assert.ArgumentNotNull(runtime, "runtime");
      var clrObject = this.GetObjectToProcessByFactory(runtime);
      if (clrObject.IsNullObj)
      {
        // Context.Message("Null ClrObject is passed for translation");
        yield break;
      }

      yield return clrObject;
      
    }


    /// <summary>
    /// Gets the object to process by <see cref="IModelMapperFactory"/> and result to be treated as <see cref="IEnumerable{T}"/> of <see cref="ClrObject"/>s.
    /// </summary>
    /// <param name="runtime">The ds.</param>
    /// <returns></returns>
    protected virtual ClrObject GetObjectToProcessByFactory([NotNull] ClrRuntime runtime)
    {
      return this.GetObjectToProcessByFactory(runtime, this.ClassName, this.FieldNamesChain);
    }

    /// <summary>
    /// Gets the object to process by <see cref="IModelMapperFactory"/> and result to be treated as <see cref="IEnumerable{T}"/> of <see cref="ClrObject"/>s.
    /// </summary>
    /// <param name="runtime">The ds.</param>
    /// <returns></returns>
    protected virtual ClrObject GetObjectToProcessByFactory([NotNull] ClrRuntime runtime, string className, string[] fieldNameChain)
    {
      var fld = runtime.GetStaticFldValue(className, fieldNameChain);
      if (fld.IsNullObj)
      {
        // Context.Message("Null object is read by following field chain from {0} class", this.ClassName);
      }

      return fld;
    }
  }
}
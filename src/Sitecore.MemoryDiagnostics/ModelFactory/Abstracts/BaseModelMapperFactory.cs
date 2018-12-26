namespace Sitecore.MemoryDiagnostics.ModelFactory.Abstracts
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Diagnostics;

  using Attributes;
  using Disablers;
  using Models.BaseMappingModel;
  using Models.InternalProcessing;
  using Sitecore;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;
  using StringExtensions;

  /// <summary>
  ///   Filters <see cref="ClrObject" /> with no address, or no type.
  ///   <para>Resolves recursive calls. Once recursion is detected, <see cref="RecursionDetectedModel"/> is produced.</para>
  /// </summary>
  public abstract class BaseModelMapperFactory : IModelMapperFactory
  {
    /// <summary>
    ///   Keep track of already processed <see cref="ClrObject" /> to avoid recursive calls.
    /// </summary>
    private readonly ArrayList currentlyConvertedClrObjectAddresses = new ArrayList(50);

    private readonly Dictionary<ulong, Action<object>> recursionCallBacks = new Dictionary<ulong, Action<object>>();

    /// <summary>
    ///   Transforms <paramref name="obj" /> into corresponding <see cref="IClrObjMappingModel" /> and fills field values.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public virtual IClrObjMappingModel BuildModel([CanBeNullObject] ClrObject obj)
    {
      if (obj.IsNullObj)
      {
        return EmptyClrObjectModel.Instance;
      }

      if (obj.Type == null)
      {
        return new ClrObjNoType
        {
          Obj = obj
        };
      }

      if (!RecursionCheckerDisabler.IsActive && this.currentlyConvertedClrObjectAddresses.Contains(obj.Address))
      {
        return new RecursionDetectedModel
        {
          Obj = obj
        };
      }

      try
      {
        if (!RecursionCheckerDisabler.IsActive)
        {
          this.currentlyConvertedClrObjectAddresses.Add(obj.Address);
        }

        IClrObjMappingModel value = this.DoBuildModelFreeOfRecursion(obj);

        // Would not be called in case an exception occurs during filling model
        this.InvokeSubscribedRecursions(value, obj);

        return value;
      }
      finally
      {
        if (!RecursionCheckerDisabler.IsActive)
        {
          this.recursionCallBacks.Remove(obj.Address);
          this.currentlyConvertedClrObjectAddresses.Remove(obj.Address);
        }
      }
    }

    // TODO: introduce method someday for convinience
    public virtual void InvokeOnLeavingRecursion(ulong address, [NotNull] Action<object> fieldValueSetter)
    {
      Action<object> callback;

      if (!this.recursionCallBacks.ContainsKey(address))
      {
        callback = fieldValueSetter;
        this.recursionCallBacks.Add(address, callback);
      }
      else
      {
        callback = this.recursionCallBacks[address];
        callback += fieldValueSetter;
        this.recursionCallBacks[address] = callback;
      }
    }

    public virtual ArrayMappingModel ReadArray([CanBeNullObject] ClrObject obj)
    {
      if (obj.IsNullObj)
      {
        return new ArrayMappingModel();
      }

      ClrAssert.ObjectNotNullTypeNotEmpty(obj);

      if (!obj.Type.IsArray)
      {
        throw new ArrayTypeMismatchException("Type {0} not of array type".FormatWith(obj.Type.Name));
      }

      return this.BuildModel(obj) as ArrayMappingModel;
    }

    /// <summary>
    ///   <see cref="BuildModel" /> API ensures that method would not go into recursion during processing circular references.
    /// </summary>
    /// <param name="obj">The object ensured to have type and not empty pointer.</param>
    /// <returns>
    ///   The <see cref="IClrObjMappingModel" />.
    /// </returns>
    protected abstract IClrObjMappingModel DoBuildModelFreeOfRecursion([ClrObjAndTypeNotEmpty] ClrObject obj);

    /// <summary>
    /// Invokes the subscribed recursions.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="clrObject">The color object.</param>
    protected virtual void InvokeSubscribedRecursions(IClrObjMappingModel value, ClrObject clrObject)
    {
      if (RecursionCheckerDisabler.IsActive || !this.recursionCallBacks.ContainsKey(clrObject.Address))
      {
        return;
      }

      if (value != null)
      {
        Delegate[] invocationList = this.recursionCallBacks[clrObject.Address].GetInvocationList();
        foreach (Delegate @delegate in invocationList)
        {
          try
          {
            @delegate.DynamicInvoke(value);
          }
          catch (Exception ex)
          {
            Trace.TraceError("{0} error during {1} setting for {2} object", ex.ToString(), value.GetType().Name,
              clrObject.Address.ToString("x8"));
          }
        }
      }

      this.recursionCallBacks.Remove(clrObject.Address);
    }
  }
}
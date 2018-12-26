namespace Sitecore.MemoryDiagnostics.ModelFactory
{
  using System;
  using System.Collections;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.CompilerServices;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.CollectionReaders;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.ModelFactory.Abstracts;
  using Sitecore.MemoryDiagnostics.ModelFactory.Assisting;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.FallBack.SitecoreRelated;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Data;
  using Sitecore.Diagnostics;
  using Sitecore.MemoryDiagnostics;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;
  using Sitecore.Services;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;
  using System.Diagnostics;
  using Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders;

  /// <summary>
  ///   Converts <see cref="ClrObject" /> into <see cref="IClrObjMappingModel" />.
  ///   <para>Class mapping is provided by <see cref="ModelByTypeNameProvider" />.</para>
  ///   <para>
  ///     <example> <see cref="AlarmClock" /> would give <see cref="AlarmClockModel" /></example>
  ///   </para>
  ///   <para>
  ///     Model can have simple fields like ID, Guid, DateTime thanks to <see cref="PrimitiveKnownEntitiesReader" />
  ///   </para>
  ///   <para>Collections are split (read) by <see cref="CollectionEnumeratorProvider" /></para>
  /// </summary>
  public class ModelMapperFactory : CacheBasedModelMapperFactory
  {
    #region Fields
    /// <summary>
    ///   Reads complex objects like  <see cref="Hashtable" />, <see cref="Array" />, generic Lists,
    ///   <see cref="WeakReference" />, <see cref="EventHandler"/>.
    ///   <para>
    ///     Has a set of <see cref="CollectionReaderBase" /> objects that are supposed to determine if they can read an
    ///     object or not.
    ///   </para>
    ///   <para> Should transform <see cref="ClrObject" /> to <see cref="IClrObjMappingModel" /></para>
    /// </summary>
    public readonly CollectionEnumeratorProvider CollectionEnumerator;

    /// <summary>
    ///   <para>Provides class mapping between <see cref="ClrObject.Type" /> and <see cref="IClrObjMappingModel" />.</para>
    ///   <para>
    ///     <example> <see cref="AlarmClock" /> would give <see cref="AlarmClockModel" /> </example>
    ///   </para>
    ///   <para> Key - source (MD) class name, Value - matching model.</para>
    ///   <para><see cref="PriorityFallbackMatchTypeProvider.AddOrUpdate(System.Type)" /> method to add a model binding</para>
    /// </summary>
    public readonly PriorityFallbackMatchTypeProvider ModelByTypeNameProvider;

    /// <summary>
    ///   Reads known types. F.e. <see cref="DateTime" />, <see cref="Guid" />, <see cref="ID" />, <see cref="TimeSpan" />
    /// </summary>
    public readonly PrimitiveKnownEntitiesReader PrimitiveKnownTypesFieldRdr = new PrimitiveKnownEntitiesReader();    

    /// <summary>
    ///   The injectable fields cache. Key is type name, where value is array of fields with
    ///   <see cref="InjectFieldValueAttribute" />
    /// </summary>
    protected readonly InjectableFieldsCache InjectableFldsCache = new InjectableFieldsCache();

    #endregion
    #region Constructors
    /// <summary>
    ///   Initializes a new instance of the <see cref="ModelMapperFactory" /> class.
    /// </summary>
    /// <param name="formatters">The formatters.</param>
    /// <param name="collectionStrippersTypes">The collection strippers types.</param>
    public ModelMapperFactory([CanBeNull] Type[] formatters = null, [CanBeNull] Type[] collectionStrippersTypes = null)
    {
      this.ModelByTypeNameProvider = new PriorityFallbackMatchTypeProvider(formatters, TextConstants.FallbackModelsNamespace);
      this.CollectionEnumerator = new CollectionEnumeratorProvider(collectionStrippersTypes, this);
    }

    #endregion

    #region Public API

    public void AddOrUpdate(IPrimitiveEntityReader reader)
    {
      this.PrimitiveKnownTypesFieldRdr.AddOrUpdateReader(reader);
    }

    public void AddOrUpdate(Type modelType)
    {
      this.ModelByTypeNameProvider.AddOrUpdate(modelType);
    }

    public void AddOrUpdate(CollectionReaderBase collectionReader)
    {
      this.CollectionEnumerator.AddOrUpdate(collectionReader);
    }
    #endregion 

    #region Protected methods
    protected virtual bool CanInjectField([NotNull] FieldInfo modelField, ClrObject obj, out string violation)
    {
      violation = string.Empty;

      if (obj.Type.IsString)
      {
        return true;
      }

      if (!obj.HasSameNameField(modelField))
      {
        violation = ViolationFormatter.NoFieldInSource(obj, modelField);
        return false;
      }

      ClrInstanceField fld = obj.Type.GetFieldByName(modelField.Name);
      if ((fld == null) || (fld.Type == null))
      {
        violation = ViolationFormatter.NoFieldInSource(obj, modelField);
      }

      return fld?.Type != null;
    }

    /// <summary>
    ///   Builds the model. Free of recursion. Caching layer is provided by base class as well as high-level exception handling
    /// </summary>
    /// <param name="obj">Obj is guaranteed to have type and have non-null pointer.</param>
    /// <returns>
    ///   The <see cref="IClrObjMappingModel" />.
    /// </returns>
    protected override IClrObjMappingModel DoBuildModelWrapped([NotNull] ClrObject obj)
    {
      ClrAssert.ObjectNotNullTypeNotEmpty(obj);

      IClrObjMappingModel model = this.GetModelByObjectType(obj);

      if (model is NoConverterForType)
      {
        IClrObjMappingModel specialCollection;

        // Try to read special collections like hashtables, arrays, generic lists, weak references,
        return this.CollectionEnumerator.TryProcessSpecialCollection(obj, out specialCollection) ? specialCollection : model;
      }

#if TRACE
      Trace.TraceInformation("{0} model was picked for {1} obj {2}", model.GetType().Name, obj.Type.Name, obj.Address.ToString("x8"));
#endif

      this.MapModelFields(ref model, obj);
      model.Compute();
      return model;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool FieldWithSameNameExists(ClrObject clrObject, [NotNull] FieldInfo fieldInfo)
    {
      ClrAssert.TypeNotEmpty(clrObject);
      return clrObject.Type.Fields.Any(t => t.Name.Equals(fieldInfo.Name, StringComparison.Ordinal));
    }

    [NotNull]
    protected virtual IClrObjMappingModel GetModelByObjectType([CanBeNull] ClrObject clrObject)
    {
      ClrType tp = clrObject.Type;
      if (tp == null)
      {
        return new ClrObjNoType
        {
          Obj = clrObject
        };
      }

      Type modelType = this.ModelByTypeNameProvider[tp.Name];
      if (modelType == null)
      {
        return new NoConverterForType
        {
          Obj = clrObject
        };
      }

      var result = Activator.CreateInstance(modelType) as IClrObjMappingModel;

#if TRACE
      Tracer.Debug($"{modelType.Name} was picked for {clrObject.HexAddress} of {clrObject.Type.Name} type");
#endif

      return Assert.ResultNotNull(
        result,
        $"Could not cast {modelType.FullName} type to {typeof(IClrObjMappingModel).FullName}");
    }

    protected virtual bool MapFieldValueToModel(FieldInfo fieldInfo, ClrObject clrObject, IClrObjMappingModel model)
    {
      object val;

      bool read = this.ReadFieldValue(fieldInfo, clrObject, out val);

      if (read && (val != null))
      {
        if (val is RecursionDetectedModel)
        {
          ClrObject value = clrObject.GetRefFld(fieldInfo.Name);

          this.InvokeOnLeavingRecursion(value.Address, o => fieldInfo.SetValue(model, o));
        }
        else
        {
          if (val is EmptyClrObjectModel)
          {
#if TRACE
            if (val.GetType().IsInstanceOfType(typeof(NoConverterForType)))
            {
              Trace.TraceInformation("Converter for {0} field of {1} object was not found", fieldInfo.Name, clrObject.Address.ToString("x8"));
              if (Debugger.IsAttached)
                Debugger.Break();
            }
#endif
            if (fieldInfo.FieldType.IsInstanceOfType(val))
            {
              fieldInfo.SetValue(model, val);
            }

            return true;
          }

          fieldInfo.SetValue(model, val);
        }
      }

      return read;
    }

    /// <summary>
    ///   <paramref name="sourceObj" /> is expected to be a plain object - not collection.
    /// </summary>
    /// <param name="targetSetFldInfo">The field information.</param>
    /// <param name="sourceObj">The color object.</param>
    /// <param name="val">The value.</param>
    /// <returns><c>true</c> if field read correctly;<c>false</c> otherwise.</returns>
    protected virtual bool ReadFieldValue(
      [NotNull] FieldInfo targetSetFldInfo,
      [CanBeNull] ClrObject sourceObj,
      [CanBeNull] out object val)
    {
      Assert.ArgumentNotNull(targetSetFldInfo, "targetSetFldInfo");

      val = null;

      string fldName = targetSetFldInfo.Name;
      try
      {
        if (targetSetFldInfo.FieldType == typeof(string))
        {
          val = sourceObj.GetStringSafe(fldName);
          return true;
        }

        // F.e. Int, float ...
        if (targetSetFldInfo.FieldType.IsPrimitive)
        {
          // TODO: NMI test. Migration to newer CLR
          var fld = sourceObj.Type.GetFieldByName(fldName);
          val = fld.GetValue(sourceObj.Address);

          // val = sourceObj.Type.GetFieldValue(sourceObj.Address, new[] { fldName });
          // = sourceObj.Type.GetTypeToMapOn(sourceObj.GetValueFld(fldName).Address);
          return true;
        }

        if (targetSetFldInfo.FieldType.IsEnum)
        {
          // TODO: NMI test. Migration to newer CLR
          var fld = sourceObj.Type.GetFieldByName(fldName);
          val = fld.GetValue(sourceObj.Address);
          return true;

          // return sourceObj.Type.TryGetFieldValue(sourceObj.Address, new[] { fldName }, out val);
        }

        // Try read primitive types like Guid, ID, DateTime .
        // And return read value as an instance of primitive type.
        if (this.PrimitiveKnownTypesFieldRdr.TryCalcFieldValue(targetSetFldInfo, sourceObj, out val))
        {
          return true;
        }

        // Read pointer for the object reference
        ClrObject value = sourceObj.GetRefFld(fldName);

        // Read value through recursion.
        val = this.BuildModel(value);

        return true;
      }
      catch (Exception ex)
      {
        // TODO: Consider returning true and an exception.
        return false;
      }
    }

    #endregion
    #region Private methods
    /// <summary>
    ///   Maps the model fields.
    ///   <para>Reads all interface fields and tries to get fields with same names in <paramref name="clrObject"/>.   
    ///   </para>
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="clrObject">The CLR object.</param>
    private void MapModelFields(ref IClrObjMappingModel model, ClrObject clrObject)
    {
      // For safety.
      clrObject.ReEvaluateType();

      model.Obj = clrObject;
      if (model.Obj.Type == null)
      {
        model = new ClrObjNoType
        {
          Obj = clrObject
        };
        return;
      }

      foreach (FieldInfo fieldInfo in this.InjectableFldsCache[model])
      {
        string violation;
        if (this.CanInjectField(fieldInfo, clrObject, out violation))
        {
          this.MapFieldValueToModel(fieldInfo, clrObject, model);
        }
        else
        {
          model.BindingLog.AppendLine(violation);
        }
      }
    }
    #endregion
  }
}
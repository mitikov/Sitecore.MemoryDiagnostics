namespace Sitecore.MemoryDiagnostics.ModelFactory.Assisting
{
  using System;
  using System.Collections;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.MemoryDiagnostics.CollectionReaders;
  using Sitecore.MemoryDiagnostics.Helpers;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore;
  using Sitecore.Diagnostics;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;
  using System.Diagnostics;

  /// <summary>
  ///   Should return <see cref="IClrObjMappingModel" /> in case can read object.
  ///   <para>
  ///     Based on a collection of <see cref="CollectionReaderBase" />. Each object is passed through a set of collection
  ///     readers by calling <see cref="CollectionReaderBase.SupportTransformation" />.
  ///   </para>
  ///   <para>
  ///     Reads complicated collection objects like <see cref="Hashtable" />, Generic lists, and etc. where match by type
  ///     name would not work.
  ///   </para>
  /// </summary>
  public class CollectionEnumeratorProvider
  {
    protected List<CollectionReaderBase> CollectionEnumerators;
    protected ModelMapperFactory Factory;

    public CollectionEnumeratorProvider([CanBeNull] Type[] collectionEnumerators, [NotNull] ModelMapperFactory factory)
    {
      Assert.ArgumentNotNull(factory, " no factory provided.");
      if (collectionEnumerators == null)
        collectionEnumerators =
          ReflectionHelper.GetNotAbstractClassTypesFromNamespace(TextConstants.CollectionEnumerationNamespace);
      CollectionReaderBase[] readers = BuildEntityReaders(collectionEnumerators);
      CollectionEnumerators = new List<CollectionReaderBase>(readers);

      Factory = factory;
    }

    /*      Builder Pattern. Should Allow easy API     */
    [NotNull]
    public CollectionEnumeratorProvider Add([CanBeNull] CollectionReaderBase instance)
    {
      if (instance != null)
        CollectionEnumerators.Add(instance);

      return this;
    }

    public CollectionEnumeratorProvider AddOrUpdate([CanBeNull] CollectionReaderBase instance)
    {

      // TODO: Rework API.
      if (instance != null)
      {
        if (CollectionEnumerators.Contains(instance))
        {
          CollectionEnumerators.Remove(instance);
        }

        CollectionEnumerators.Add(instance);
      }


      return this;
    }

    /// <summary>
    ///   Tries to process special collections like <see cref="WeakReference" />, <see cref="Array" />,
    ///   <see cref="Hashtable" />, <see cref="ConcurrentDictionary{TKey,TValue}" />
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public bool TryProcessSpecialCollection([NotNull] ClrObject obj, [CanBeNull] out IClrObjMappingModel result)
    {
      result = null;
      CollectionReaderBase reader;

      if (!IsSpecial(obj, out reader))
      {
#if TRACE
        Trace.TraceWarning("No Reader for {0} type {1}", obj.Type.Name, obj.Address.ToString("x8"));
#endif
        return false;
      }

#if TRACE
      Trace.TraceInformation($"{reader.GetType().Name} reader was picked for {obj.HexAddress} object with {obj.Type.Name} type");

#endif

      result = reader.ReadEntity(obj, Factory);

      return true;
    }

    protected Dictionary<Func<ClrObject, bool>, Func<ClrObject, ModelMapperFactory, IClrObjMappingModel>> BuildEntityReaderDictionary([CanBeNull] Type[] candidates)
    {
      CollectionReaderBase[] readers = BuildEntityReaders(candidates);
      var res = new Dictionary<Func<ClrObject, bool>, Func<ClrObject, ModelMapperFactory, IClrObjMappingModel>>();
      if (readers == null)
        return res;

      foreach (CollectionReaderBase reader in readers)
      {
        res.Add(reader.SupportTransformation, reader.ReadEntity);
      }

      return res;
    }

    protected CollectionReaderBase[] BuildEntityReaders([CanBeNull] Type[] candidates)
    {
      return candidates == null
        ? null
        : candidates.Select(Activator.CreateInstance).OfType<CollectionReaderBase>().ToArray();
    }

    protected bool IsSpecial(ClrObject obj, out CollectionReaderBase res)
    {
      // obj.ReEvaluateType();
      res = null;

      if (CollectionEnumerators == null)
        return false;

      res = CollectionEnumerators.FirstOrDefault(t =>
      {
        try
        {
          return t.SupportTransformation(obj);
        }
        catch
        {
          return false;
        }
      }
     );

      return res != null;
    }
  }
}
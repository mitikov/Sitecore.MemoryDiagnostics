namespace Sitecore.MemoryDiagnostics.StatisticsCollector.HeapByType
{
  using System;
  using System.Collections.Generic;
  using System.Runtime.CompilerServices;
  using System.Threading;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  public class HeapTypeStatisticsEntry : IEquatable<HeapTypeStatisticsEntry>, ICloneable
  {
    [NotNull]
    public readonly TypeModel _typeModel;

    public long ElementsCount;

    public long TotalSizeOccupied;

    public HeapTypeStatisticsEntry(long totalSizeOccupied, [NotNull] ClrType type, long elementsCount = 1)
      : this(totalSizeOccupied, new TypeModel(type), elementsCount)
    {
    }

    public HeapTypeStatisticsEntry(long totalSizeOccupied, [NotNull] string typeName, ulong mt, long elementsCount = 1)
      : this(totalSizeOccupied, new TypeModel(typeName, mt), elementsCount)
    {
    }

    public HeapTypeStatisticsEntry(long totalSizeOccupied, [NotNull] TypeModel model, long elementsCount = 1)
    {
      Assert.ArgumentNotNull(model, "typeModel");

      // Assert.ArgumentCondition(elementsCount > 0, "elementsCount", "Count must be bigger than zero.");
      _typeModel = model;
      TotalSizeOccupied = totalSizeOccupied;
      ElementsCount = elementsCount;
    }

    public ulong MT => this._typeModel.MT;

    public string TypeName => this._typeModel.TypeName;

    [NotNull]
    public static HeapTypeStatisticsCollection BuildContainerWithDefaultComparer()
    {
      return new HeapTypeStatisticsCollection();
    }

    public static HeapTypeStatisticsEntry operator +(HeapTypeStatisticsEntry f, long usedByObj)
    {
      if (f == null)
      {
        return null;
      }

      f.AddTypeUsage(usedByObj);
      return f;
    }

    public static HeapTypeStatisticsEntry operator +(HeapTypeStatisticsEntry f, HeapTypeStatisticsEntry sec)
    {
      if ((f == null) && (sec == null))
      {
        return null;
      }

      if ((f == null) || (sec == null))
      {
        return f ?? sec;
      }

      return f.Add(sec);
    }

    public static HeapTypeStatisticsEntry operator -(HeapTypeStatisticsEntry f, HeapTypeStatisticsEntry sec)
    {
      if ((f == null) && (sec == null))
      {
        return null;
      }

      if ((f != null) && (sec == null))
      {
        return f;
      }

      if ((f == null) && (sec != null))
      {
        return new HeapTypeStatisticsEntry(-sec.TotalSizeOccupied, sec._typeModel, -sec.ElementsCount);
      }

      Assert.IsTrue(f._typeModel.Equals(sec._typeModel), "Cannot process models with different types!");

      return new HeapTypeStatisticsEntry(
       totalSizeOccupied: f.TotalSizeOccupied - sec.TotalSizeOccupied,
       model: f._typeModel,
       elementsCount: f.ElementsCount - sec.ElementsCount);
    }

    [NotNull]
    public HeapTypeStatisticsEntry Add([CanBeNull] HeapTypeStatisticsEntry f)
    {
      if (f == null)
      {
        return this;
      }

      Assert.IsTrue(f._typeModel.Equals(_typeModel), "Cannot add different types");

      Interlocked.Add(ref TotalSizeOccupied, f.TotalSizeOccupied);

      Interlocked.Add(ref ElementsCount, f.ElementsCount);

      return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddTypeUsage(long size, long count = 1)
    {
      Assert.ArgumentCondition(size > 0, "size", "Size must be bigger than zero.");
      Assert.ArgumentCondition(count > 0, "count", "Count must be bigger than zero.");

      Interlocked.Add(ref TotalSizeOccupied, size);
      Interlocked.Add(ref ElementsCount, count);
    }

    public void ChangeTotalSizeSign()
    {
      long tmp = Interlocked.Read(ref TotalSizeOccupied);

      // TODO: Change in future.
      Interlocked.Exchange(ref TotalSizeOccupied, -tmp);
    }

    public object Clone()
    {
      return new HeapTypeStatisticsEntry(TotalSizeOccupied, _typeModel, ElementsCount);
    }

    public bool Equals([CanBeNull] HeapTypeStatisticsEntry other)
    {
      return (other != null) && _typeModel.Equals(other._typeModel);
    }

    public override int GetHashCode()
    {
      return this._typeModel.hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool OfSameType([CanBeNull] HeapTypeStatisticsEntry entry)
    {
      return (entry != null) && entry._typeModel.Equals(_typeModel);
    }

    public override string ToString()
    {
      return string.Concat
        (this._typeModel, 
        " ",
        "Total Size: ",
        StringUtil.GetSizeString(this.TotalSizeOccupied),
        " Total objects: ",
        this.ElementsCount);
    }

    public class HeapTypeStatisticsEntryComparer : IEqualityComparer<HeapTypeStatisticsEntry>
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Equals(HeapTypeStatisticsEntry x, HeapTypeStatisticsEntry y)
      {
        if ((x == null) && (y == null))
        {
          return true;
        }

        if ((x == null) || (y == null))
        {
          return false;
        }

        return x._typeModel.MT.Equals(y._typeModel.MT);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int GetHashCode(HeapTypeStatisticsEntry obj)
      {
        return obj._typeModel.hash;
      }
    }
  }
}
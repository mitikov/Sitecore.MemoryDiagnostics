namespace Sitecore.MemoryDiagnostics.StatisticsCollector.HeapByType
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using System.Text;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Microsoft.Diagnostics.Runtime;
  using Sitecore.Diagnostics;

  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  [NotThreadSafe]
  public class HeapTypeStatisticsCollection : ICloneable, IEnumerable<HeapTypeStatisticsEntry>
  {
    private readonly Dictionary<ulong, HeapTypeStatisticsEntry> typeStats =
      new Dictionary<ulong, HeapTypeStatisticsEntry>(10000);

    public long DifferentTypesFound
    {
      get
      {
        return typeStats.Count;
      }
    }

    public long TotalNumberOfObjects
    {
      get
      {
        return this.Sum(t => t.ElementsCount);
      }
    }

    public long TotalSizeInBytes
    {
      get
      {
        return this.Sum(t => t.TotalSizeOccupied);
      }
    }

    public HeapTypeStatisticsEntry this[ulong mt] => this.Get(mt);

    public HeapTypeStatisticsEntry this[string typeName] => this.Get(typeName);

    public static HeapTypeStatisticsCollection operator -(
      HeapTypeStatisticsCollection first, HeapTypeStatisticsCollection second)
    {
      HeapTypeStatisticsCollection result;
      if ((first == null) && (second == null))
      {
        return null;
      }

      if ((first == null) || (second == null))
      {
        if (first == null)
        {
          result = second.Clone() as HeapTypeStatisticsCollection;
          if (result == null)
          {
            return null; // never happens
          }

          result.ChangeSizeSign();
          return result;
        }

        return first;
      }

      result = new HeapTypeStatisticsCollection();

      foreach (HeapTypeStatisticsEntry typeStatInFirst in first)
      {
        HeapTypeStatisticsEntry sameTypeInSecond = second[typeStatInFirst.MT];
        result.AddOrMerge(typeStatInFirst - sameTypeInSecond);
      }

      // Add non existing in first as negative to result
      foreach (HeapTypeStatisticsEntry tpSecondStat in second)
      {
        HeapTypeStatisticsEntry val = result[tpSecondStat.MT];
        if (val != null) // value already exists. 
        {
          continue;
        }

        val = tpSecondStat.Clone() as HeapTypeStatisticsEntry;
        if (val == null)
        {
          continue;
        }

        val.ChangeTotalSizeSign();
        result.AddOrMerge(val);
      }

      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add([NotNull] ClrObject obj)
    {
      Assert.ArgumentCondition(obj.Type != null, "obj", "Object with empty type");

      Add(obj.Address, obj.Type);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(ulong candidateAddress, [NotNull] ClrType type)
    {
      if (candidateAddress == 0)
        return;

      Assert.ArgumentNotNull(type, "type");
      HeapTypeStatisticsEntry existing = Get(type.MetadataToken);

      var size = (long)type.GetSize(candidateAddress);
      if (existing == null)
      {
        var added = new HeapTypeStatisticsEntry(size, type);
        typeStats.Add(type.MetadataToken, added);
        return;
      }

      existing.AddTypeUsage(size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddOrMerge(HeapTypeStatisticsEntry candidate)
    {
      Assert.ArgumentNotNull(candidate, "candidate");

      HeapTypeStatisticsEntry existing = Get(candidate.MT);
      if (existing == null)
      {
        typeStats.Add(candidate.MT, candidate);
        return;
      }

      existing.Add(candidate);
    }


    [NotThreadSafe]
    public void Cleanup()
    {
      typeStats.Clear();
    }

    public object Clone()
    {
      var result = new HeapTypeStatisticsCollection();

      IEnumerable<HeapTypeStatisticsEntry> clonedValues = from keyPair in typeStats
                                                          let sourceTypeStat = keyPair.Value.Clone() as HeapTypeStatisticsEntry
                                                          where sourceTypeStat != null
                                                          select sourceTypeStat;

      foreach (HeapTypeStatisticsEntry heapTypeStatisticsEntry in clonedValues)
      {
        result.typeStats.Add(heapTypeStatisticsEntry.MT, heapTypeStatisticsEntry);
      }

      return result;
    }

    public IEnumerator<HeapTypeStatisticsEntry> GetEnumerator()
    {
      return typeStats.Values.GetEnumerator();
    }

    public override string ToString()
    {
      var sb = new StringBuilder(typeStats.Count * 200);

      foreach (HeapTypeStatisticsEntry entry in this.OrderByDescending(t => t.TotalSizeOccupied))
      {
        sb.AppendLine(entry.ToString());
        sb.AppendLine();
      }

      return sb.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return typeStats.Values.GetEnumerator();
    }

    protected void ChangeSizeSign()
    {
      foreach (var entry in typeStats)
      {
        entry.Value.ChangeTotalSizeSign();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected HeapTypeStatisticsEntry Get(ulong mt)
    {
      return typeStats.ContainsKey(mt) ? typeStats[mt] : null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected HeapTypeStatisticsEntry Get([CanBeNull] string typeName)
    {
      return
        typeStats.Values.FirstOrDefault(
          existingValue => existingValue.TypeName.Equals(typeName, StringComparison.Ordinal));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected HeapTypeStatisticsEntry Get(HeapTypeStatisticsEntry candidate)
    {
      return Get(candidate._typeModel.MT);
    }
  }
}
using Math = System.Math;
using StringBuilder = System.Text.StringBuilder;

namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  using System;
  using System.Text;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  [ModelMapping(typeof(StringBuilder))]
  public class StringBuilderMappingModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public int m_ChunkLength;

    public string TextContent = string.Empty;


    public bool IsEmpty() => string.IsNullOrEmpty(TextContent);
    public override string Caption
    {
      get
      {
        var length = Math.Min(TextContent.Length, 30);

        return base.Caption + (length > 0 ? TextContent.Substring(0, length) : "[no text]");
      }
    }

    public override void Compute()
    {
      var sb = this.ComputeOwnContent();

      InjectFollowedChunkIfAny(sb);

      TextContent = sb.ToString();
    }

    private void InjectFollowedChunkIfAny(StringBuilder sb)
    {
      var previousChunk = Obj.GetRefFld("m_ChunkPrevious");
      if ((previousChunk.IsNullObj) || previousChunk.Address == Obj.Address || previousChunk.Type == null)
      {
        return;
      }

      var parent = new StringBuilderMappingModel() { Obj = previousChunk };
      parent.Compute();

      sb.Insert(0, parent.TextContent);
    }

    private StringBuilder ComputeOwnContent()
    {
      var m_ChunkCharsPointer = Obj.GetRefFld("m_ChunkChars");
      if (m_ChunkCharsPointer.IsNullObj || (m_ChunkCharsPointer.Type == null) || (!m_ChunkCharsPointer.Type.IsArray))
        return new StringBuilder();

      m_ChunkLength = Obj.GetInt32Fld(nameof(m_ChunkLength));

      var ar = ClrCollectionHelper.EnumerateArrayOfSimpleTypes<char>(m_ChunkCharsPointer, m_ChunkLength);

      var sb = new StringBuilder(ar.Count);
      for (var i = 0; (i < m_ChunkLength) && (i < ar.Count); i++)
      {
        var letter = ar[i];
        sb.Append(letter);
      }

      return sb;
    }

    public override string ToString()
    {
      return TextContent;
    }
  }
}
namespace Sitecore.MemoryDiagnostics.Models.FallBack.Reflection
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  [DebuggerDisplay("{m_strAssemblyName} has {m_iPublicComTypeCount} types. Is saved: {m_isSaved}")]
  [ModelMapping("System.Reflection.Emit.AssemblyBuilderData")]
  public class AssemblyBuilderDataModel: ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string m_strAssemblyName;

    [InjectFieldValue]
    public bool m_isSaved;

    [InjectFieldValue]
    public string m_strDir;

    // Expect elements of 'System.Reflection.Emit.TypeBuilder' type.
    [InjectFieldValue]
    public ArrayMappingModel m_publicComTypeList;

    [InjectFieldValue]
    public int m_iPublicComTypeCount;

    public override string Caption => base.Caption + m_strAssemblyName ?? "[No aseembly name]";

    public bool HasTypesDefined => DefinedTypes?.Count > 0;

    public IReadOnlyList<TypeBuilderModel> DefinedTypes { get; set; }

    public override void Compute()
    {
      var definedTypes = new List<TypeBuilderModel>();

      if (m_publicComTypeList != null)
      {
        definedTypes.AddRange(m_publicComTypeList.OfType<TypeBuilderModel>());
      }

      DefinedTypes = definedTypes;

      base.Compute();
    }
    }
  }

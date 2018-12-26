namespace Sitecore.MemoryDiagnostics.Models.FallBack.Reflection
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Reflection;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

  /// <summary>
  /// Carries information about a type being generated on fly.
  /// </summary>
  [DebuggerDisplay("{m_strFullQualName} has {m_constructorCount} constructors")]
  [ModelMapping("System.Reflection.Emit.TypeBuilder")]
  public class TypeBuilderModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string m_strName;

    [InjectFieldValue]
    public string m_strNameSpace;

    [InjectFieldValue]
    public string m_strFullQualName;

    [InjectFieldValue]
    public TypeAttributes m_iAttr;

    [InjectFieldValue]
    public int m_constructorCount;

    [InjectFieldValue]
    public ArrayMappingModel m_listMethods;

    public bool HasMethodsDefined => Methods?.Count > 0;

    public IReadOnlyList<MethodBuilderModel> Methods { get; set; }

    public override string Caption => base.Caption + m_strFullQualName;

    public override void Compute()
    {
      var methods = new List<MethodBuilderModel>();

      if (m_listMethods != null)
      {                                                                                                                       
        methods.AddRange(m_listMethods.OfType<MethodBuilderModel>());    
      }

      this.Methods = methods;

      base.Compute();
    }


  }
}

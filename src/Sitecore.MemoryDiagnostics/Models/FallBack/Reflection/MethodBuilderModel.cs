using System.Diagnostics;
using System.Reflection;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.ModelFactory;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.Models.InternalProcessing;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.Reflection
{
  /// <summary>
  /// Represents a method generated dynamically.
  /// </summary>
  [DebuggerDisplay("{m_strName} is baked {m_bIsBaked}")]
  [ModelMapping("System.Reflection.Emit.MethodBuilder")]
  public class MethodBuilderModel : ClrObjectMappingModel
  {
    /*  Class definition
     *  http://referencesource.microsoft.com/#mscorlib/system/reflection/emit/methodbuilder.cs,ea0410df0289a8e0
     */

    [InjectFieldValue]
    public string m_strName;

    [InjectFieldValue]
    public MethodAttributes m_iAttributes;

    [InjectFieldValue]
    public CallingConventions m_callingConvention;

    [InjectFieldValue]
    public bool m_bIsBaked;

    /// <summary>
    /// The IL code for the method.
    /// <para>Expected type to be <see cref="ArrayMappingModel"/>.</para>
    /// </summary>
    [InjectFieldValue]
    public LazyLoad<IClrObjMappingModel> m_ubBody;

    /// <summary>
    /// Defines the method signature that shall be used by the method.
    /// </summary>
    [InjectFieldValue]
    public SignatureHelperModel m_signature;

     /// <summary>
     /// <c>true</c> if method represents contructor.
     /// </summary>
    public bool IsConstructor => string.Equals(m_strName, ".ctor");

    public bool IsPublic => m_iAttributes.HasFlag(MethodAttributes.Public);
    public bool IsStatic => m_iAttributes.HasFlag(MethodAttributes.Static);

    public string MethodName => m_strName ?? "[No Method name yet]";

    public override string Caption => $"{base.Caption} {MethodName} public- {IsPublic}";
  }
}

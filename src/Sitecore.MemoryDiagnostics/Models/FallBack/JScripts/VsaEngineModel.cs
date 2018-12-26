using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.MemoryDiagnostics.Attributes;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;

namespace Sitecore.MemoryDiagnostics.Models.FallBack.JScripts
{
  [ModelMapping("Microsoft.JScript.Vsa.VsaEngine")]
  public class VsaEngineModel : ClrObjectMappingModel
  {
    [InjectFieldValue]
    public string compiledRootNamespace;

    [InjectFieldValue]
    public string scriptLanguage;

    [InjectFieldValue]
    public bool haveCompiledState;

    [InjectFieldValue]
    public bool isEngineInitialized;

    [InjectFieldValue]
    public string engineMoniker;

    [InjectFieldValue]
    public string rootNamespace;

    [InjectFieldValue]
    public string assemblyVersion;

    [InjectFieldValue]
    public bool doSaveAfterCompile;

  }
}

namespace Sitecore.MemoryDiagnostics.Attributes
{
  using System;
  using Microsoft.Diagnostics.Runtime;

  /// <summary>
  ///   Indicates that parameter is expected to have not empty <see cref="ClrObject2.Address" /> and
  ///   <see cref="ClrObject2.Type" />.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = false)]
  public sealed class ClrObjAndTypeNotEmpty : Attribute
  {
  }
}
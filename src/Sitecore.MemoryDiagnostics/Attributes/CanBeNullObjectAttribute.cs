namespace Sitecore.MemoryDiagnostics.Attributes
{
  using System;
  using Microsoft.Diagnostics.Runtime;


  /// <summary>
  ///   <see cref="ClrObject2" /> can be null Object ( no address, no type )
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
  public sealed class CanBeNullObjectAttribute : Attribute
  {
  }
}
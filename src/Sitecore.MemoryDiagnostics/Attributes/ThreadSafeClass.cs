namespace Sitecore.MemoryDiagnostics.Attributes
{
  using System;

  /// <summary>
  ///   Indicates that class is thread safe.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class ThreadSafeClass : Attribute
  {
  }
}
namespace Sitecore.MemoryDiagnostics.Attributes
{
  using System;
  using Sitecore.MemoryDiagnostics.ModelFactory;

  /// <summary>
  ///   <see cref="ModelMapperFactory" /> would inject value into field that has this attribute.
  ///   <para>Model must have field with same as object in memory dump.</para>
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public class InjectFieldValueAttribute : Attribute
  {
  }
}
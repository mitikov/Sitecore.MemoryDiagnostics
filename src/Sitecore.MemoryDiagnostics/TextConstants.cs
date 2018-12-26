using Sitecore.MemoryDiagnostics.CollectionReaders;
using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
using Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders;

namespace Sitecore.MemoryDiagnostics
{
  /// <summary>
  /// Text constants like namespaces names, default string representations.
  /// </summary>
  public static class TextConstants
  {
    /// <summary>
    /// Default namespace to load <see cref="CollectionReaderBase"/> from.
    /// </summary>
    public const string CollectionEnumerationNamespace = @"Sitecore.MemoryDiagnostics.CollectionReaders";

    /// <summary>
    /// Default namespace to load <see cref="IClrObjMappingModel"/> models from.
    /// </summary>
    public const string FallbackModelsNamespace = @"Sitecore.MemoryDiagnostics.Models.FallBack";

    /// <summary>
    ///  Default namespace to load <see cref="IPrimitiveEntityReader"/> from.
    /// </summary>
    public const string PrimitiveEntitiesNamespace = @"Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders";

    /// <summary>
    /// The 'null reference' text representation.
    /// </summary>
    public const string NullReferenceString = "[NullReference]";

   
    /// <summary>
    /// The 'NoType' message.
    /// </summary>
    public const string NoType = "[NoType]";

    public const string CouldNotReadPointer = "[CouldNotReadPointer]";

    public const string NoUrlText = "[NoUrl]";

    public static class CookieNames

    {
      public const string SitecoreAnalyticsGlobal = "SC_ANALYTICS_GLOBAL_COOKIE";

      public const string AspNetSession = "ASP.NET_SessionId";
    }

  }
}
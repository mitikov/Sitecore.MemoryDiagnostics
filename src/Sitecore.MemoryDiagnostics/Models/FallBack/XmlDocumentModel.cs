namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  using System.Diagnostics;
  using System.IO;
  using System.Text;
  using System.Xml;
  using Sitecore.MemoryDiagnostics.Attributes;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  [DebuggerDisplay("{TextDocument}")]
  [ModelMapping(typeof(XmlDocument))]
  public class XmlDocumentModel : ClrObjectMappingModel
  {
    public XmlDocument Document;

    public string TextDocument;
    public override void Compute()
    {
      base.Compute();

      Document = ClrXmlHelper.ReconstructFromXmlDocument(this.Obj);

      var xmlDocumentString = new StringBuilder();
      Document.Save(new StringWriter(xmlDocumentString));
      TextDocument = xmlDocumentString.ToString();
    }

    public override string ToString()
    {
      return TextDocument;
    }
  }
}

namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using System.Collections;
  using System.Diagnostics;
  using System.Text;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.MemoryDiagnostics.ModelMetadataInterfaces;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.StringExtensions;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Mapping for majority of key-value collections. 
  /// </summary>
  [DebuggerDisplay("{Count} [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  public class HashtableMappingModel : ClrObjectMappingModel, IEnumerable, ICaptionHolder
  {
    public static readonly int LIMIT = 40;

    /// <summary>
    ///   The content of the <see cref="HashtableMappingModel" />.
    /// </summary>
    [NotNull]
    public readonly Hashtable Elements;

    /// <summary>
    ///   Initializes a new instance of the <see cref="HashtableMappingModel" /> class.
    /// </summary>
    public HashtableMappingModel()
      : this(new Hashtable())
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="HashtableMappingModel" /> class.
    /// </summary>
    /// <param name="initializedHashtable">The initialized hashtable.</param>
    public HashtableMappingModel([NotNull] Hashtable initializedHashtable)
      : this(initializedHashtable, new ClrObject())
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="HashtableMappingModel" /> class.
    /// </summary>
    /// <param name="expectedElementsCount">The expected count of <see cref="Elements" />.</param>
    public HashtableMappingModel(int expectedElementsCount)
      : this(new Hashtable(expectedElementsCount))
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="HashtableMappingModel" /> class.
    /// </summary>
    /// <param name="obj">The object.</param>
    public HashtableMappingModel(ClrObject obj)
      : this(new Hashtable(), obj)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="HashtableMappingModel" /> class.
    /// </summary>
    /// <param name="expectedElementsCount">The expected elements count.</param>
    /// <param name="obj">The object.</param>
    public HashtableMappingModel(int expectedElementsCount, ClrObject obj)
      : this(new Hashtable(expectedElementsCount), obj)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="HashtableMappingModel" /> class.
    /// </summary>
    /// <param name="initializedHashtable">The initialized hashtable.</param>
    /// <param name="clrObj">The color object.</param>
    public HashtableMappingModel([NotNull] Hashtable initializedHashtable, ClrObject clrObj)
    {
      Assert.ArgumentNotNull(initializedHashtable, "Hashtable");
      this.Elements = initializedHashtable;
      this.Obj = clrObj;
    }

    public string Caption => $"{base.Caption} {this.Count} elements in Hashtable";

    [NotNull]
    public int Count => this.Elements.Count;

    public object this[object key] => this.Elements.ContainsKey(key) ? this.Elements[key] : null;

    public static explicit operator Hashtable(HashtableMappingModel model)
    {
      return model?.Elements;
    }

    public IEnumerator GetEnumerator()
    {
      return this.Elements.GetEnumerator();
    }

    public override string ToString()
    {
      int real = (this.Count > LIMIT) ? LIMIT : this.Count;

      var sb = new StringBuilder(real * 100);
      sb.AppendLine(this.Caption);
      int i = 0;
      foreach (DictionaryEntry keyValue in this.Elements)
      {
        sb.AppendLine("{0} -> {1}".FormatWith(keyValue.Key, keyValue.Value));
        i++;
        if (i > LIMIT)
        {
          return sb.ToString();
        }
      }

      return sb.ToString();
    }
  }
}
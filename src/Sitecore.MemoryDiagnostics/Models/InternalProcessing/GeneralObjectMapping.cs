namespace Sitecore.MemoryDiagnostics.Models.InternalProcessing
{
  using System;
  using System.Diagnostics;
  using System.Text;
  using Microsoft.Diagnostics.Runtime;

  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.MemoryDiagnostics.Extensions;
  using Sitecore.MemoryDiagnostics.MetadataProviders;
  using Sitecore.MemoryDiagnostics.Models.BaseMappingModel;
  using Sitecore.MemoryDiagnostics.PrimitiveEntitiesReaders;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  /// <summary>
  /// Fallback for unknown models.
  /// </summary>
  [DebuggerDisplay(
     "{caption} [Pointer] {Obj.Address} [Model] {GetType().Name}")]
  public class GeneralMapping : ClrObjectMappingModel
  {
    private static string fieldPrintFormat = "{0}: {1}{2}";
    protected string caption;
    protected string content;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralMapping"/> class.
    /// </summary>
    /// <param name="obj">The object.</param>
    public GeneralMapping(ClrObject obj)
    {
      this.Obj = obj;


      this.caption = "[General][" + this.Obj.HexAddress + "] " + this.ObjTypeFullName;
    }

    [NotNull]
    public override string Caption
    {
      get
      {
        Assert.IsNotNullOrEmpty(this.caption, "No caption provided");
        return this.caption;
      }
    }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(this.content))
      {
        return this.content;
      }

      try
      {
        this.content = this.BuildModelFields().ToString();
      }
      catch (Exception ex)
      {
        this.content = "Could not build fields." + Environment.NewLine + ex;
      }

      return this.content;
    }


    protected virtual void AppendFieldToStringBuilder(StringBuilder sb, ClrField field)
    {
      Assert.ArgumentNotNull(sb, "stringbuilder");
      Assert.ArgumentNotNull(field, "field");
      switch (field.ElementType)
      {
        case ClrElementType.String:
        {
          sb.AppendFormat(fieldPrintFormat, field.Name, this.Obj.GetStringSafe(field.Name), Environment.NewLine);
          break;
        }

        case ClrElementType.Object:
        {
          sb.AppendFormat("{0}: {1}: {2}{3}", field.Name, field.Type.Name, this.Obj.GetRefFld(field.Name).HexAddress, Environment.NewLine);
          break;
        }

        case ClrElementType.Int32:
        {
          sb.AppendFormat(fieldPrintFormat, field.Name, this.Obj.GetSimpleFld<int>(field.Name), Environment.NewLine);
          break;
        }

        case ClrElementType.Int64:
        {
          sb.AppendFormat(fieldPrintFormat, field.Name, this.Obj.GetSimpleFld<long>(field.Name), Environment.NewLine);
          break;
        }

        case ClrElementType.Boolean:
        {
          sb.AppendFormat(fieldPrintFormat, field.Name, this.Obj.GetBoolFld(field.Name), Environment.NewLine);
          break;
        }

        case ClrElementType.Double:
        {
          sb.AppendFormat(fieldPrintFormat, field.Name, this.Obj.GetDoubleFld(field.Name), Environment.NewLine);
          break;
        }

        case ClrElementType.UInt64:
        {
          sb.AppendFormat(fieldPrintFormat, field.Name, this.Obj.GetUInt64Fld(field.Name), Environment.NewLine);
          break;
        }

        case ClrElementType.Struct:
        {
          var t = field.Type;
          if (t == null)
            break;

          // check date
          string stringres = string.Empty;

          var fld = field.Type.GetFieldByName("dateData");
          if (fld != null)
          {
            stringres = this.DateToString(field);
          }
          else if (TimeSpanReader.CanRead(this.Obj, field.Name))
          {
            stringres = TimeSpanReader.Read(this.Obj, field.Name).ToString("g");
          }
          else
            {
              break; // Cannot Read anyway =\
            }

          sb.AppendFormat(fieldPrintFormat, field.Name, stringres, Environment.NewLine);
          break;
        }
      }

      sb.AppendLine(string.Empty); // delimitor to simplify reading for human eye
    }

    protected virtual StringBuilder BuildModelFields()
    {
      var obType = this.Obj.Type;
      var allFields = obType.Fields;

      var sb = new StringBuilder(allFields.Count * 200);
      if (allFields.Count > 0)
      {
        sb.AppendLine("Instance fields");
      }

      foreach (var field in allFields)
      {
        this.AppendFieldToStringBuilder(sb, field);
      }

      var threadStaticFlds = obType.ThreadStaticFields;

      if (threadStaticFlds.Count > 0)
      {
        sb.AppendLine("Thread static fields");
      }

      foreach (var field in threadStaticFlds)
      {
        this.AppendFieldToStringBuilder(sb, field);
      }

      return sb;
    }

    private string DateToString(ClrField field)
    {
      string stringres = string.Empty;
      var dt = DateTimeReader.FromObjToDate(this.Obj, field);

      if (dt == DateTime.MinValue)
      {
        return string.Format(fieldPrintFormat, field.Name, "[EmptyDate]", Environment.NewLine);
      }

      var timeSince = MetadataManager.GetDumpTime(this.Obj) - dt;
      return dt.ToString("s") + " ( " + timeSince.ToString("g") + " ago)";
    }
  }
}
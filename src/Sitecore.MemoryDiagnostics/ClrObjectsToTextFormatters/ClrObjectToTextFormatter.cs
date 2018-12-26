namespace Sitecore.MemoryDiagnostics.ClrObjectsToTextFormatters
{
  using System;

  using Attributes;
  using Microsoft.Diagnostics.Runtime;
  using PrimitiveEntitiesReaders;
  using SitecoreMemoryInspectionKit.Core.ClrHelpers;

  public class ClrObjectToTextFormatter : ClrObjectToTextFormatterBase
  {
    /// <summary>
    /// Single instance of state-less object.
    /// </summary>
    public static readonly ClrObjectToTextFormatter Instance = new ClrObjectToTextFormatter();

    public override bool TryToText([CanBeNullObject]ClrObject clrObject, out string text)
    {
      try
      {
        return this.Parse(clrObject, out text);
      }
      catch
      {
        text = string.Empty;
        return false;
      }
    }

    protected virtual bool Parse([CanBeNullObject] ClrObject clrObject, out string text)
    {
      text = string.Empty;
      if (clrObject.IsNullObj || (clrObject.Type == null))
      {
        return false;
      }

      var elementType = clrObject.Type.ElementType;
      var type = clrObject.Type;
      switch (elementType)
      {
        case ClrElementType.String:
        {
          text = type.GetValue(clrObject.Address) as string;
          break;
        }

        case ClrElementType.Int32:
        {
          text = ((int)type.GetValue(clrObject.Address)).ToString();

          break;
        }

        case ClrElementType.Int64:
        {
          text = ((long)type.GetValue(clrObject.Address)).ToString();
          break;
        }

        case ClrElementType.Boolean:
        {
          text = ((bool)type.GetValue(clrObject.Address)).ToString();
          break;
        }

        case ClrElementType.Double:
        {
          text = ((double)type.GetValue(clrObject.Address)).ToString();
          break;
        }

        case ClrElementType.UInt64:
        {
          text = ((ulong)type.GetValue(clrObject.Address)).ToString();
          break;
        }

        case ClrElementType.Struct:
        {
          if (string.Equals(type.Name, typeof(Guid).FullName))
          {
            text = ClrObjectValuesReader.ReadGuidValue(clrObject).ToString();
            break;
          }

          if (string.Equals(type.Name, typeof(DateTime).FullName))
          {
            text = DateTimeReader.FromObjToDate(clrObject).ToString();
            break;
          }

          if (string.Equals(type.Name, typeof(TimeSpan).FullName))
          {
            text = TimeSpanReader.Read(clrObject).ToString();
            break;
          }

          break;
        }
      }

      return string.IsNullOrEmpty(text);
    }
  }
}
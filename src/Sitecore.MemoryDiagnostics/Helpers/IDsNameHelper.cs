namespace Sitecore.MemoryDiagnostics.Helpers
{
  using System;
  using System.Collections;
  using System.Linq;
  using System.Reflection;
  using JetBrains.Annotations;
  using Sitecore.Data;

  public class IDsNameHelper
  {
    private static readonly Hashtable FieldIdName = new Hashtable();

    private static readonly string[] fieldIDsClassName = new[]
    {
      @"Sitecore.FieldIDs", @"Sitecore.TemplateFieldIDs"
    };

    static IDsNameHelper()
    { try
      {
        LoadMappings();
      }
      catch
      {

      }
      
    }

    private static void LoadMappings()
    {
      var sitecoreKernel = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(t => t.FullName.StartsWith("Sitecore.Kernel"));
      if (sitecoreKernel == null)
      {
        return;
      }

      foreach (var s in fieldIDsClassName)
      {
        var tp = sitecoreKernel.GetType(s);
        if (tp == null)
        {
          continue;
        }

        var fields = tp.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (FieldInfo fieldInfo in fields)
        {
          var val = fieldInfo.GetValue(null) as ID;
          var name = fieldInfo.Name;
          if (!ReferenceEquals(val, null) && (!string.IsNullOrEmpty(name)))
          {
            FieldIdName[val] = name;
          }
        }
      }

      foreach (var definedType in sitecoreKernel.ExportedTypes)
      {
        if (definedType.Name.IndexOf("ItemIDs", StringComparison.OrdinalIgnoreCase) < 0)
        {
          continue;
        }

        var fields = definedType.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var fieldInfo in fields)
        {
          if (fieldInfo.FieldType == typeof(ID))
          {
            var val = fieldInfo.GetValue(null) as ID;
            var name = fieldInfo.Name;
            if (!ReferenceEquals(val, null) && (!string.IsNullOrEmpty(name)))
            {
              FieldIdName[val] = definedType.Name + "." + name;
            }
          }
        }
      }
    }

    public static void AddMapping([CanBeNull] string name, [NotNull] ID id)
    {
      /*
       * TODO: think about reload
       */
      if (!string.IsNullOrEmpty(name))
      {
        FieldIdName[id] = name;
      }
    }

    public static string ToString(ID id)
    {
      if (!FieldIdName.ContainsKey(id))
      {
        return id.ToString();
      }

      return (string)FieldIdName[id];
    }

    public static string ToString(Guid guid)
    {
      var id = new ID(guid);
      return ToString(id);
    }
  }
}
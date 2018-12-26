﻿namespace Sitecore.MemoryDiagnostics.Helpers
{
  using System.Linq;
  using System.Text;
  using Sitecore.MemoryDiagnostics.Extensions;

  /// <summary>
  ///   Credit to  http://stackoverflow.com/questions/4580397/json-formatter-in-c
  /// <para>Allows to format JSON.</para>
  /// </summary>
  public class JsonHelper
  {
    private const string INDENT_STRING = "      ";

    public static string FormatJson(string str)
    {
      int indent = 0;
      bool quoted = false;
      var sb = new StringBuilder();
      for (int i = 0; i < str.Length; i++)
      {
        char ch = str[i];
        switch (ch)
        {
          case '{':
          case '[':
            sb.Append(ch);
            if (!quoted)
            {
              sb.AppendLine();
              Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
            }

            break;
          case '}':
          case ']':
            if (!quoted)
            {
              sb.AppendLine();
              Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
            }

            sb.Append(ch);
            break;
          case '"':
            sb.Append(ch);
            bool escaped = false;
            int index = i;
            while ((index > 0) && (str[--index] == '\\'))
              escaped = !escaped;
            if (!escaped)
              quoted = !quoted;
            break;
          case ',':
            sb.Append(ch);
            if (!quoted)
            {
              sb.AppendLine();
              Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
            }

            break;
          case ':':
            sb.Append(ch);
            if (!quoted)
              sb.Append(" ");
            break;
          default:
            sb.Append(ch);
            break;
        }
      }

      return sb.ToString();
    }
  }
}
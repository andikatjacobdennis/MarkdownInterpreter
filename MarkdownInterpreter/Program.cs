using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace MarkdownInterpreter
{
    namespace MarkdownToXml
    {
        class Program
        {
            static void Main(string[] args)
            {
                string markdown = "# Heading \n\nThis is some **bold** text and some _italic_ text.\n\n> This is a blockquote.\n\n```csharp\nConsole.WriteLine(\"Hello, world!\");\n```\n\n---\n\nThis is a [link](https://example.com).";

                MarkdownInterpreter interpreter = new MarkdownInterpreter();
                string xml = interpreter.Interpret(markdown);

                Console.WriteLine(xml);
                Console.ReadLine();
            }
        }

        // Interpreter Interface
        public interface IInterpreter
        {
            string Interpret(string input);
        }

        // Concrete Interpreter
        public class MarkdownInterpreter : IInterpreter
        {
            private Dictionary<string, string> regexDictionary;

            public MarkdownInterpreter()
            {
                regexDictionary = new Dictionary<string, string>
                {
                    {"^(#{1,6})\\s+(.+)$", "<h1>$2</h1>"},
                    {"(\\*\\*)(.*?)(\\*\\*)", "<bold>$2</bold>"},
                    {"(_)(.*?)(_)", "<italic>$2</italic>"},
                    {"(>)(.*)", "<blockquote>$2</blockquote>"},
                    {"(```csharp)(.*?)(```)", "<codeblock>$2</codeblock>"},
                    {"(---)", "<hr />"},
                    {"\\[(.*?)\\]\\((.*?)\\)", "<link href=\"$2\">$1</link>"}
                };
            }

            public string Interpret(string input)
            {
                XmlDocument xmlDoc = new XmlDocument();
                using (XmlWriter writer = xmlDoc.CreateNavigator().AppendChild())
                {
                    writer.WriteStartElement("root");
                    foreach (KeyValuePair<string, string> entry in regexDictionary)
                    {
                        string pattern = entry.Key;
                        string replacement = entry.Value;

                        Regex regex = new Regex(pattern);
                        input = regex.Replace(input, replacement);
                    }
                    writer.WriteString(input);
                    writer.WriteEndElement();
                }
                return xmlDoc.InnerXml;
            }
        }
    }
}

using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace COSMOS.Core.Config
{
    public class XmlConfigReader : IConfigReader, IConfigReaderErrorInfo
    {
        public bool HasChildren
        {
            get
            {
                return children.Count > 0;
            }
        }

        public int ChildCount
        {
            get
            {
                return children.Count;
            }
        }

        public string Type { get; }
        public string Name { get; }
        public string Value { get; }

        private readonly List<XmlElement> children = new List<XmlElement>();

        private readonly XmlElement root;

        public XmlConfigReader(XmlElement xml)
        {
            root = xml;
            Name = xml.Name;

            if (xml.HasAttribute("Type"))
            {
                Type = xml.GetAttribute("Type");
            }

            foreach (XmlNode item in xml.ChildNodes)
            {
                if (item is XmlElement)
                {
                    children.Add(item as XmlElement);
                }
            }

            if (children.Count == 0)
            {
                Value = xml.InnerText;
            }
        }
        public static XmlConfigReader CreateReader(string xml)
        {
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(xml);

                var xroot = xdoc.DocumentElement;

                return new XmlConfigReader(xroot);
            }
            catch (System.Exception ex)
            {
                Log.Error("Create XmlConfigReader failed.\n" + ex.ToString(), "Config",
                    "Reader", "Xml");

                return null;
            }
        }

        public IConfigReader GetChild(int i)
        {
            if (i >= children.Count)
            {
                return null;
            }
            return new XmlConfigReader(children[i]);
        }

        public string GetInfoForError()
        {
            string info = "Element: \"" + root.Name;

            if (root is IXmlLineInfo)
            {
                var lineInfo = root as IXmlLineInfo;
                if (lineInfo.HasLineInfo())
                {
                    info += "\" line: " + lineInfo.LineNumber;
                }
            }

            return info;
        }

        public IEnumerable<KeyValuePair<string, string>> GetArgs()
        {
            return Enumerable.Select(root.Attributes.Cast<XmlAttribute>(), a => new KeyValuePair<string, string>(a.Name, a.Value));
        }
    }
}

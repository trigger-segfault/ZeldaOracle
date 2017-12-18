using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConscriptDesigner.Content {
	public class ContentXmlInfo {

		public const string XmlNamespace = @"http://schemas.microsoft.com/developer/msbuild/2003";

		private static readonly string[] ValidElementNames = new string[] {
			"None",
			"Compile",
			"Content",
			"Folder"
		};

		public static bool IsContentElement(XmlElement element) {
			if (element.HasAttribute("Include")) {
				foreach (string name in ValidElementNames) {
					if (element.Name == name)
						return true;
				}
			}
			return false;
		}


		public string ElementName { get; set; }

		public Dictionary<string, string> Attributes { get; set; }
		public Dictionary<string, string> Elements { get; set; }

		public string Include {
			get {
				string value;
				Attributes.TryGetValue("Include", out value);
				return value ?? "";
			}
			set { Attributes["Include"] = value; }
		}
		public string CopyToOutputDirectory {
			get {
				string value;
				Elements.TryGetValue("CopyToOutputDirectory", out value);
				return value ?? "";
			}
			set { Elements["CopyToOutputDirectory"] = value; }
		}
		public string Name {
			get {
				string value;
				Elements.TryGetValue("Name", out value);
				return value ?? "";
			}
			set { Elements["Name"] = value; }
		}

		public string Importer {
			get {
				string value;
				Elements.TryGetValue("Importer", out value);
				return value ?? "";
			}
			set { Elements["Importer"] = value; }
		}
		public string Processor {
			get {
				string value;
				Elements.TryGetValue("Processor", out value);
				return value ?? "";
			}
			set { Elements["Processor"] = value; }
		}

		public ContentXmlInfo() {
			ElementName = "";
			Attributes = new Dictionary<string, string>();
			Elements = new Dictionary<string, string>();
			Attributes.Add("Include", "");
		}

		public void Read(XmlElement element) {
			ElementName = element.Name;
			foreach (XmlAttribute attr in element.Attributes) {
				Attributes[attr.Name] = attr.InnerText;
			}
			foreach (XmlNode node in element.ChildNodes) {
				XmlElement elem = node as XmlElement;
				if (elem == null) continue;
				Elements[elem.Name] = elem.InnerText;
			}
		}
		public XmlElement Write(XmlDocument doc) {
			XmlElement element = doc.CreateElement("", ElementName, XmlNamespace);
			foreach (var pair in Attributes) {
				element.SetAttribute(pair.Key, pair.Value);
			}
			foreach (var pair in Elements) {
				XmlElement child = doc.CreateElement("", pair.Key, XmlNamespace);
				child.InnerText = pair.Value;
				element.AppendChild(child);
			}
			return element;
		}
	}
}

using System.Collections.Generic;
using System.Xml;

namespace ConscriptDesigner.Content {
	/// <summary>XML info used by the .contentproj file.</summary>
	public class ContentXmlInfo {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The XML namespace used for the project file.</summary>
		public const string XmlNamespace = @"http://schemas.microsoft.com/developer/msbuild/2003";
		
		/// <summary>The valid names for content file elements.</summary>
		private static readonly string[] ValidElementNames = new string[] {
			"None",
			"Compile",
			"Content",
			"Folder"
		};


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The element name of the content file.</summary>
		public string ElementName { get; set; }

		/// <summary>The element attributes of the content file.</summary>
		public Dictionary<string, string> Attributes { get; set; }
		/// <summary>The contained elements of the content file.</summary>
		public Dictionary<string, string> Elements { get; set; }

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the base XML info.</summary>
		public ContentXmlInfo() {
			ElementName = "";
			Attributes = new Dictionary<string, string>();
			Elements = new Dictionary<string, string>();
			Attributes.Add("Include", "");
		}
		

		//-----------------------------------------------------------------------------
		// Reading
		//-----------------------------------------------------------------------------

		/// <summary>Reads the XML info from the element.</summary>
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

		/// <summary>Writes the XML info to an element and returns it.</summary>
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


		//-----------------------------------------------------------------------------
		// Static
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the specified element is identified as a content file.</summary>
		public static bool IsContentElement(XmlElement element) {
			if (element.HasAttribute("Include")) {
				foreach (string name in ValidElementNames) {
					if (element.Name == name)
						return true;
				}
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets or sets the include path of the element.</summary>
		public string Include {
			get {
				string value;
				Attributes.TryGetValue("Include", out value);
				return value ?? "";
			}
			set { Attributes["Include"] = value; }
		}

		/// <summary>Gets or sets the name for the content asset.</summary>
		public string Name {
			get {
				string value;
				Elements.TryGetValue("Name", out value);
				return value ?? "";
			}
			set { Elements["Name"] = value; }
		}

		/// <summary>Gets or sets the importer for the content asset.</summary>
		public string Importer {
			get {
				string value;
				Elements.TryGetValue("Importer", out value);
				return value ?? "";
			}
			set { Elements["Importer"] = value; }
		}

		/// <summary>Gets or sets the processor for the content asset.</summary>
		public string Processor {
			get {
				string value;
				Elements.TryGetValue("Processor", out value);
				return value ?? "";
			}
			set { Elements["Processor"] = value; }
		}

		/// <summary>Gets or sets how copying to the output directory is handled.</summary>
		public string CopyToOutputDirectory {
			get {
				string value;
				Elements.TryGetValue("CopyToOutputDirectory", out value);
				return value ?? "";
			}
			set { Elements["CopyToOutputDirectory"] = value; }
		}


	}
}

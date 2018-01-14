using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;
using ConscriptDesigner.Windows;
using ZeldaOracle.Common.Geometry;

namespace ConscriptDesigner {
	public static class ProjectUserSettings {

		public const int Version = 1;


		// Playback
		public static class Playback {
			public static double Volume { get; set; }
			public static bool Looping { get; set; }
		}

		// Find and Replace
		public static class FindAndReplace {
			public static bool MatchCase { get; set; }
			public static bool MatchWord { get; set; }
			public static bool RegularExpressions { get; set; }
			public static bool SearchUp { get; set; }
			public static bool LiveSearch { get; set; }
			public static FindScopes Scope { get; set; }
		}

		// Unimplemented
		public static class TextEditing {
			public static bool WordWrap { get; set; }
			public static bool SmartIndentation { get; set; }
			public static bool DarkMode { get; set; }
		}

		public static class Window {
			public static bool Maximized { get; set; }
			public const double DefaultWidth = 1030;
			public const double DefaultHeight = 650;
			public static double Width { get; set; }
			public static double Height { get; set; }
		}

		static ProjectUserSettings() {
			LoadDefaults();
		}

		public static void LoadDefaults() {

			Playback.Volume = 1.0;
			Playback.Looping = false;

			FindAndReplace.MatchCase = true;
			FindAndReplace.MatchWord = false;
			FindAndReplace.RegularExpressions = false;
			FindAndReplace.SearchUp = false;
			FindAndReplace.LiveSearch = true;
			FindAndReplace.Scope = FindScopes.CurrentDocument;

			TextEditing.WordWrap = false;
			TextEditing.SmartIndentation = true;
			TextEditing.DarkMode = false;

			Window.Maximized = false;
			Window.Width = Window.DefaultWidth;
			Window.Height = Window.DefaultHeight;
		}

		public static bool Load() {
			string path = DesignerControl.ProjectSettingsFile;
			if (File.Exists(path)) {
				LoadDefaults();
				try {
					XmlDocument doc = new XmlDocument();

					XmlNode node;
					XmlAttribute attr;
					doc.Load(path);

					XmlNode root = doc.SelectSingleNode("/ConscriptDesigner");
					attr = root.Attributes["Version"];
					if (attr == null) return false;
					

					node = root.SelectSingleNode("Playback/Volume");
					if (node != null)
						Playback.Volume = GMath.Clamp(double.Parse(node.InnerText), 0, 1);
					node = root.SelectSingleNode("Playback/Looping");
					if (node != null)
						Playback.Looping = bool.Parse(node.InnerText);


					node = root.SelectSingleNode("FindAndReplace/MatchCase");
					if (node != null)
						FindAndReplace.MatchCase = bool.Parse(node.InnerText);
					node = root.SelectSingleNode("FindAndReplace/MatchWord");
					if (node != null)
						FindAndReplace.MatchWord = bool.Parse(node.InnerText);
					node = root.SelectSingleNode("FindAndReplace/RegularExpressions");
					if (node != null)
						FindAndReplace.RegularExpressions = bool.Parse(node.InnerText);
					node = root.SelectSingleNode("FindAndReplace/SearchUp");
					if (node != null)
						FindAndReplace.SearchUp = bool.Parse(node.InnerText);
					node = root.SelectSingleNode("FindAndReplace/LiveSearch");
					if (node != null)
						FindAndReplace.LiveSearch = bool.Parse(node.InnerText);
					node = root.SelectSingleNode("FindAndReplace/Scope");
					if (node != null)
						FindAndReplace.Scope = (FindScopes)Enum.Parse(typeof(FindScopes), node.InnerText);

					node = root.SelectSingleNode("Window/Maximized");
					if (node != null)
						Window.Maximized = bool.Parse(node.InnerText);
					node = root.SelectSingleNode("Window/Width");
					if (node != null)
						Window.Width = Math.Max(1, double.Parse(node.InnerText));
					node = root.SelectSingleNode("Window/Height");
					if (node != null)
						Window.Height = Math.Max(1, double.Parse(node.InnerText));

					XmlNode anchorables = root.SelectSingleNode("Anchorables");
					if (anchorables != null) {
						if (anchorables.SelectSingleNode("OutputConsole") != null)
							DesignerControl.MainWindow.OpenOutputConsole();
						if (anchorables.SelectSingleNode("ProjectExplorer") != null)
							DesignerControl.MainWindow.OpenProjectExplorer();
						if (anchorables.SelectSingleNode("SpriteBrowser") != null)
							DesignerControl.MainWindow.OpenSpriteBrowser();
						if (anchorables.SelectSingleNode("SpriteSourceBrowser") != null)
							DesignerControl.MainWindow.OpenSpriteSourceBrowser();
						if (anchorables.SelectSingleNode("StyleBrowser") != null)
							DesignerControl.MainWindow.OpenStyleBrowser();
						if (anchorables.SelectSingleNode("TileDataBrowser") != null)
							DesignerControl.MainWindow.OpenTileDataBrowser();
						if (anchorables.SelectSingleNode("TilesetBrowser") != null)
							DesignerControl.MainWindow.OpenTilesetBrowser();
					}

					XmlNode documents = root.SelectSingleNode("Documents");
					if (documents != null) {
						string activePath = null;
						attr = documents.Attributes["ActivePath"];
						if (attr != null)
							activePath = attr.InnerText;
						foreach (XmlNode docNode in documents.SelectNodes("Content")) {
							string contentPath = docNode.InnerText;
							ContentFile file = DesignerControl.Project.Get(contentPath);
							if (file != null) {
								file.Open(true, false);
							}
						}
						if (activePath != null) {
							ContentFile activeFile = DesignerControl.Project.Get(activePath);
							if (activeFile != null && activeFile.IsOpen) {
								activeFile.Document.IsActive = true;
							}
						}
					}

					XmlNode expanded = root.SelectSingleNode("ExpandedFolders");
					if (expanded != null) {
						foreach (XmlNode expandNode in expanded.SelectNodes("Folder")) {
							string contentPath = expandNode.InnerText;
							ContentFile file = DesignerControl.Project.Get(contentPath);
							if (file != null && file.IsFolder) {
								file.TreeViewItem.IsExpanded = true;
							}
						}
					}

					return true;
				}
				catch (Exception) { }
			}
			LoadDefaults();
			return false;
		}

		public static bool Save() {
			string path = DesignerControl.ProjectSettingsFile;
			try {
				XmlDocument doc = new XmlDocument();
				XmlElement element;
				XmlElement root = doc.CreateElement("ConscriptDesigner");
				root.SetAttribute("Version", Version.ToString());
				doc.AppendChild(root);

				XmlElement playback = doc.CreateElement("Playback");
				root.AppendChild(playback);

				element = doc.CreateElement("Volume");
				element.AppendChild(doc.CreateTextNode(Playback.Volume.ToString()));
				playback.AppendChild(element);
				element = doc.CreateElement("Looping");
				element.AppendChild(doc.CreateTextNode(Playback.Looping.ToString()));
				playback.AppendChild(element);

				XmlElement findReplace = doc.CreateElement("FindAndReplace");
				root.AppendChild(findReplace);

				element = doc.CreateElement("MatchCase");
				element.AppendChild(doc.CreateTextNode(FindAndReplace.MatchCase.ToString()));
				findReplace.AppendChild(element);
				element = doc.CreateElement("MatchWord");
				element.AppendChild(doc.CreateTextNode(FindAndReplace.MatchWord.ToString()));
				findReplace.AppendChild(element);
				element = doc.CreateElement("RegularExpressions");
				element.AppendChild(doc.CreateTextNode(FindAndReplace.RegularExpressions.ToString()));
				findReplace.AppendChild(element);
				element = doc.CreateElement("SearchUp");
				element.AppendChild(doc.CreateTextNode(FindAndReplace.SearchUp.ToString()));
				findReplace.AppendChild(element);
				element = doc.CreateElement("LiveSearch");
				element.AppendChild(doc.CreateTextNode(FindAndReplace.LiveSearch.ToString()));
				findReplace.AppendChild(element);
				element = doc.CreateElement("Scope");
				element.AppendChild(doc.CreateTextNode(FindAndReplace.Scope.ToString()));
				findReplace.AppendChild(element);

				XmlElement window = doc.CreateElement("Window");
				root.AppendChild(window);

				Window.Maximized = DesignerControl.MainWindow.WindowState == WindowState.Maximized;
				if (Window.Maximized) {
					Window.Width = Window.DefaultWidth;
					Window.Height = Window.DefaultHeight;
				}
				else {
					Window.Width = DesignerControl.MainWindow.Width;
					Window.Height = DesignerControl.MainWindow.Height;
				}

				element = doc.CreateElement("Maximized");
				element.AppendChild(doc.CreateTextNode(Window.Maximized.ToString()));
				window.AppendChild(element);
				element = doc.CreateElement("Width");
				element.AppendChild(doc.CreateTextNode(Window.Width.ToString()));
				window.AppendChild(element);
				element = doc.CreateElement("Height");
				element.AppendChild(doc.CreateTextNode(Window.Height.ToString()));
				window.AppendChild(element);

				XmlElement anchorables = doc.CreateElement("Anchorables");
				root.AppendChild(anchorables);

				if (DesignerControl.MainWindow.OutputConsole != null)
					anchorables.AppendChild(doc.CreateElement("OutputConsole"));
				if (DesignerControl.MainWindow.ProjectExplorer != null)
					anchorables.AppendChild(doc.CreateElement("ProjectExplorer"));
				if (DesignerControl.MainWindow.SpriteBrowser != null)
					anchorables.AppendChild(doc.CreateElement("SpriteBrowser"));
				if (DesignerControl.MainWindow.SpriteSourceBrowser != null)
					anchorables.AppendChild(doc.CreateElement("SpriteSourceBrowser"));
				if (DesignerControl.MainWindow.StyleBrowser != null)
					anchorables.AppendChild(doc.CreateElement("StyleBrowser"));
				if (DesignerControl.MainWindow.TileDataBrowser != null)
					anchorables.AppendChild(doc.CreateElement("TileDataBrowser"));
				if (DesignerControl.MainWindow.TilesetBrowser != null)
					anchorables.AppendChild(doc.CreateElement("TilesetBrowser"));

				XmlElement documents = doc.CreateElement("Documents");
				root.AppendChild(documents);
				ContentFile activeFile = DesignerControl.GetActiveContentFile();
				if (activeFile != null)
					documents.SetAttribute("ActivePath", activeFile.Path);

				foreach (ContentFile file in DesignerControl.MainWindow.GetOrderedOpenContentFiles()) {
					element = doc.CreateElement("Content");
					element.AppendChild(doc.CreateTextNode(file.Path));
					documents.AppendChild(element);
				}

				XmlElement expanded = doc.CreateElement("ExpandedFolders");
				root.AppendChild(expanded);

				foreach (ContentFile file in DesignerControl.Project.GetAllFiles()) {
					if (file.TreeViewItem.IsExpanded && file.IsFolder) {
						element = doc.CreateElement("Folder");
						element.AppendChild(doc.CreateTextNode(file.Path));
						expanded.AppendChild(element);
					}
				}

				doc.Save(path);

				return true;
			}
			catch (Exception) {
				return false;
			}
		}
	}
}

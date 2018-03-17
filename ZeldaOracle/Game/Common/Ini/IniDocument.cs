using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Ini {
	public class IniDocument : IEnumerable<IniSection> {

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		// Save Settings
		/// <summary>The assignment style used for property values.</summary>
		private IniAssignmentStyle assignmentStyle;
		/// <summary>The comment style used for documentation.</summary>
		private IniCommentStyle commentStyle;
		/// <summary>The maximum line length before the line is continued.</summary>
		private int maxLineLength;
		/// <summary>The line spacing between sections.</summary>
		private int sectionSpacing;

		// Load Settings
		/// <summary>True if comments are retained upon loading.</summary>
		private bool keepComments;
		/// <summary>True if duplicate sections are allowed.</summary>
		//private bool allowDuplicates;

		// Load and Save Settings
		/// <summary>Requires that all formatted lines use escape characters even when
		/// unnecessary.</summary>
		private bool strictFormatting;
		/// <summary>True if characters are escaped.</summary>
		private bool escapeEnabled;

		// Ini Properties
		/// <summary>The list of sections containing properties.</summary>
		private Dictionary<string, IniSection> sections;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the ini document.</summary>
		public IniDocument() {
			this.assignmentStyle    = IniAssignmentStyle.EqualsSpaced;
			this.commentStyle       = IniCommentStyle.SemicolonSpaced;
			this.maxLineLength      = int.MaxValue;
			this.sectionSpacing     = 1;

			this.keepComments       = true;

			this.strictFormatting   = false;
			this.escapeEnabled      = false;

			this.sections           = new Dictionary<string, IniSection>();

			// Add the global section
			Add("");
		}


		//-----------------------------------------------------------------------------
		// IEnumerable
		//-----------------------------------------------------------------------------

		/// <summary>Gets the enumerator for the sections in the document.</summary>
		public IEnumerator<IniSection> GetEnumerator() {
			return sections.Values.GetEnumerator();
		}

		/// <summary>Gets the enumerator for the sections in the document.</summary>
		IEnumerator IEnumerable.GetEnumerator() {
			return sections.Values.GetEnumerator();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the section in the document with the specified name.</summary>
		public IniSection Get(string sectionName, bool returnDefault = true) {
			IniSection section;
			sections.TryGetValue(sectionName, out section);
			if (section == null && returnDefault)
				return new IniSection(sectionName);
			return section;
		}

		/// <summary>Returns true if this document contains the specified section.</summary>
		public bool Contains(IniSection section) {
			return sections.ContainsKey(section.Name);
		}

		/// <summary>Returns true if this document contains a section with the
		/// specified name.</summary>
		public bool Contains(string sectionName) {
			return sections.ContainsKey(sectionName);
		}

		/// <summary>Returns true if the document has any non-global sections.</summary>
		public bool Any() {
			return sections.Count > 1;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds the section to the document.</summary>
		public IniSection Add(IniSection section) {
			sections[section.Name] = section;
			return section;
		}

		/// <summary>Adds a new section to the document.</summary>
		public IniSection Add(string sectionName, string comments = "") {
			return Add(new IniSection(sectionName, comments));
		}

		/// <summary>Removes the section from the document.</summary>
		public void Remove(IniSection section) {
			if (section.Name == "")
				throw new ArgumentException("Cannot remove the global section from " +
					"an ini document!");
			sections.Remove(section.Name);
		}

		/// <summary>Removes the section with the specified name from the document.</summary>
		public void Remove(string sectionName) {
			if (sectionName == "")
				throw new ArgumentException("Cannot remove the global section from " +
					"an ini document!");
			sections.Remove(sectionName);
		}

		/// <summary>Removes all of the sections from the document.</summary>
		public void Clear() {
			sections.Clear();
		}


		//-----------------------------------------------------------------------------
		// Input/Output
		//-----------------------------------------------------------------------------

		/// <summary>Loads the document from the specified tex.</summary>
		public void LoadFromText(string text) {
			ReadDocument(text);
		}

		/// <summary>Loads the document from the specified file.</summary>
		public void LoadFromFile(string path) {
			LoadFromText(File.ReadAllText(path, Encoding.UTF8));
		}

		/// <summary>Saves the document to the specified file.</summary>
		public void SaveToFile(string path) {
			File.WriteAllText(path, WriteDocument(), Encoding.UTF8);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		// Parsing --------------------------------------------------------------------

		/// <summary>Parses the text to create the list of properties.</summary>
		private void ReadDocument(string text) {
			// The lines of the text
			List<string> lines = new List<string>();
			// The current section being edited, (Set to global section by default)
			IniSection section = new IniSection("");
			// The current comments fo the next section or property
			string comments = "";

			sections.Clear();
			Add(section);

			// Formating Order:
			// Replace '\r\n' with '\n'
			// Replace '\r' with ''
			// Remove all empty lines
			// Remove '\n' if line ends with '\'
			// Split up lines
			// Remove lines beginning with ';', '#', or '!'
			// Split lines at "=", " = ", ":", or " : " and not "\\=" or "\\:"
			// Remove quotes around values
			// Implement all escape characters:
			// '\\', '\0', '\a', '\b', '\t', '\r', '\n', '\;',
			// '\#', '\!', '\=', '\:', '\"', '\ ', '\x####'

			// Replace all carriage returns with new lines
			text = text.Replace("\r\n", "\n");

			if (!keepComments) {
				// Remove all empty lines
				text = Regex.Replace(text, @"\n+", "\n");

				// Remove all commented lines
				text = Regex.Replace(text, @"\n[\;|\#|\!].*", "");
				text = Regex.Replace(text, @"^[\;|\#|\!].*\n", "");

				text = text.Replace("\\\n", "");

				// Split up the lines
				lines.AddRange(text.Split('\n'));
			}
			else {
				// Split up the lines
				lines.AddRange(text.Split('\n'));
				// Remove new line if line ends with '\'
				for (int i = 0; i < lines.Count; i++) {
					string line = lines[i];
					if (line.Any()) {
						Match match = Regex.Match(lines[i], @"^[^\;\#\!].*\\$");
						if (match.Success && match.Index == 0) {
							line = line.Substring(0, line.Length - 1);
							if (i + 1 < lines.Count) {
								lines[i] += lines[i + 1];
								lines.RemoveAt(i + 1);
							}
						}
					}
				}
			}

			// Iterate through all the lines
			foreach (string line in lines) {
				if (!line.Any()) { // Empty line comment
					// If the comment is a blank line
					comments += "\n";
				}
				else if (RegexMatchesAtZero(line, @"^[\;\#\!].*")) { // Comment
					// If the comment is spaced or not
					if (RegexMatchesAtZero(line, @"^[\;\#\!] .*"))
						comments += line.Substring(2);
					else
						comments += line.Substring(1);
				}
				else if (RegexMatchesAtZero(line, @"^\[.*\]$")) { // Section
					section = Add(ParseSection(line), comments);
					comments = "";
				}
				else { // Property
					string[] parts = ParseProperty(line).Split('\n');
					if (parts.Length == 2) {
						bool useQuotes;
						string name     = ParsePropertyName(parts[0]);
						string value    = ParsePropertyValue(parts[1], out useQuotes);
						section.Add(name, value, comments, useQuotes);
						comments = "";
					}
					else {
						// Not a valid line
					}
				}
			}
		}

		/// <summary>Returns true if the first regex pattern match starts at zero.</summary>
		private bool RegexMatchesAtZero(string text, string pattern) {
			Match match = Regex.Match(text, pattern);
			return match.Success && match.Index == 0;
		}

		/// <summary>Parses and returns the text with escape characters.</summary>
		private string ParseEscape(string text) {
			text = text.Replace("\\\\", "\\");
			text = text.Replace("\\0", "\0");
			text = text.Replace("\\a", "\a");
			text = text.Replace("\\b", "\b");
			text = text.Replace("\\t", "\t");
			text = text.Replace("\\r", "\r");
			text = text.Replace("\\n", "\n");
			text = text.Replace("\\\"", "\"");
			text = text.Replace("\\;", ";");
			text = text.Replace("\\#", "#");
			text = text.Replace("\\!", "!");
			text = text.Replace("\\=", "=");
			text = text.Replace("\\:", ":");
			text = text.Replace("\\ ", " ");
			return text;
		}

		/// <summary>Parses and returns the section.</summary>
		private string ParseSection(string section) {
			section = section.Substring(1, section.Length - 2);
			if (escapeEnabled)
				section = ParseEscape(section);
			return section;
		}

		/// <summary>Parses and returns the property.</summary>
		private string ParseProperty(string property) {
			// Separate the key and value by splitting it with '\n'
			// Replace ' ', '=', and ':' with non-matchable characters
			property = property.Replace("\\ ", "\\s");
			property = property.Replace("\\=", "\\e");
			property = property.Replace("\\:", "\\c");

			int assignIndex = -1;
			int equalsIndex = property.IndexOf("=");
			int colonIndex = property.IndexOf(":");
			if (equalsIndex != -1 &&
				(equalsIndex < colonIndex || colonIndex == -1))
				assignIndex = equalsIndex;
			else if (colonIndex != -1 &&
				(colonIndex < equalsIndex || equalsIndex == -1))
				assignIndex = colonIndex;
			if (assignIndex != -1)
				property = property.Remove(assignIndex, 1).Insert(assignIndex, "\n");

			property = property.Replace("\\s", "\\ ");
			property = property.Replace("\\e", "\\=");
			property = property.Replace("\\c", "\\:");
			return property;
		}

		/// <summary>Parses and returns the property name.</summary>
		private string ParsePropertyName(string name) {
			if (escapeEnabled)
				name = ParseEscape(name);
			return name.Trim();
		}

		/// <summary>Parses and returns the property name.</summary>
		private string ParsePropertyValue(string value, out bool useQuotes) {
			// Remove quotes if the value was quoted
			useQuotes = RegexMatchesAtZero(value, @"^\"".*\""$");
			if (useQuotes) {
				value = value.Substring(1, value.Length - 2);
			}
			if (escapeEnabled)
				value = ParseEscape(value);
			return value.Trim();
		}

		// Formatting -----------------------------------------------------------------

		/// <summary>Writes the document to a string.</summary>
		private string WriteDocument() {
			string text = "";
			int index = 0;

			// Write each of the sections
			foreach (IniSection section in sections.Values) {
				text += WriteSection(section);
				if (text.Any() && index + 1 < sections.Count && sectionSpacing > 0)
					text += new string('\n', sectionSpacing);
			}

			// Final Formatting
			text = text.Replace("\n", Environment.NewLine);

			return text;
		}

		/// <summary>Writes the section to a string.</summary>
		private string WriteSection(IniSection section) {
			string text = "";
			if (section.Comments.Any())
				text += FormatComments(section.Comments);
			if (section.Name.Any())
				text += FormatSection(section.Name) + "\n";

			foreach (IniProperty property in section) {
				text += WriteProperty(property);
			}
			return text;
		}

		/// <summary>Writes the property to a string.</summary>
		private string WriteProperty(IniProperty property) {
			string text = "";
			if (property.Comments.Any())
				text += FormatComments(property.Comments);
			text += FormatProperty(property.Name, property.Value, property.UseQuotes) + "\n";
			return text;
		}

		/// <summary>Format and returns the text with escape characters.</summary>
		private string FormatEscape(string text, bool escape, bool special,
			bool quotes, bool spaces)
		{
			text = text.Replace("\\", "\\\\");
			if (escape) {
				text = text.Replace("\0", "\\0");
				text = text.Replace("\a", "\\a");
				text = text.Replace("\b", "\\b");
				text = text.Replace("\t", "\\t");
				text = text.Replace("\r", "\\r");
				text = text.Replace("\n", "\\n");
				text = text.Replace("\0", "\\");
				text = text.Replace("\0", "\\");
			}
			if (quotes) {
				text = text.Replace("\"", "\\\"");
			}
			if (special) {
				text = text.Replace(";", "\\;");
				text = text.Replace("#", "\\#");
				text = text.Replace("!", "\\!");
				text = text.Replace("=", "\\=");
				text = text.Replace(":", "\\:");
			}
			if (spaces) {
				text = text.Replace(" ", "\\ ");
			}
			return text;
		}

		/// <summary>Formats and returns the section header.</summary>
		private string FormatSection(string name) {
			if (!name.Any())
				return "";
			string text = name;
			if (escapeEnabled)
				text = FormatEscape(text, true, strictFormatting, strictFormatting,
					strictFormatting);
			text = "[" + text + "]";
			return ContinueLine(text);
		}

		/// <summary>Formats and returns the property.</summary>
		private string FormatProperty(string name, string value, bool useQuotes) {
			string text = "";
			text += FormatPropertyName(name);
			text += assignmentStyle.GetString();
			text += FormatPropertyValue(value, useQuotes);
			return ContinueLine(text);
		}

		/// <summary>Formats and returns the property name.</summary>
		private string FormatPropertyName(string name) {
			if (escapeEnabled)
				return FormatEscape(name, escapeEnabled, true, strictFormatting, true);
			return name;
		}

		/// <summary>Formats and returns the property value.</summary>
		private string FormatPropertyValue(string value, bool useQuotes) {
			if (escapeEnabled)
				value = FormatEscape(value, true, strictFormatting, true,
					strictFormatting);
			if (useQuotes)
				value = "\"" + value + "\"";
			return value;
		}

		/// <summary>Formats and returns the comments with comment indicators.</summary>
		private string FormatComments(string comments) {
			if (comments == "")
				return "";
			string[] lines = comments.Split('\n');
			for (int i = 0; i < lines.Length; i++) {
				// Don't comment empty lines
				if (lines[i].Any())
					lines[i] = commentStyle.GetString() + lines[i];
			}
			return string.Join("\n", lines) + "\n";
		}

		/// <summary>Continues the line if it reaches the max length.</summary>
		private string ContinueLine(string text) {
			List<string> lines = new List<string>();
			while (text.Length > maxLineLength) {
				lines.Add(text.Substring(0, maxLineLength) + "/");
				text = text.Substring(maxLineLength, text.Length - maxLineLength);
			}
			if (lines.Any())
				return string.Join("/n", lines) + text;
			return text;
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the number of sections in the document.</summary>
		public int Count {
			get { return sections.Count; }
		}

		/// <summary>Gets the global ini section.</summary>
		public IniSection GlobalSection {
			get { return Get(""); }
		}

		// Load Settings --------------------------------------------------------------

		/// <summary>Gets or sets the style used for ini property value assignments.</summary>
		public IniAssignmentStyle AssignmentStyle {
			get { return assignmentStyle; }
			set { assignmentStyle = value; }
		}

		/// <summary>Gets or sets the style used for ini line comments.</summary>
		public IniCommentStyle CommentStyle {
			get { return commentStyle; }
			set { commentStyle = value; }
		}

		/// <summary>Gets or sets the maximum line length before the line is continued
		/// when saving.</summary>
		public int MaxLineLength {
			get { return maxLineLength; }
			set { maxLineLength = value; }
		}

		/// <summary>Gets or sets the line spacing between sections.</summary>
		public int SectionSpacing {
			get { return sectionSpacing; }
			set { sectionSpacing = value; }
		}


		// Save Settings --------------------------------------------------------------

		/// <summary>Gets or sets if comments are retained upon loading.</summary>
		public bool KeepComments {
			get { return keepComments; }
			set { keepComments = value; }
		}

		/// <summary>Gets or sets if duplicate sections are allowed.</summary>
		/*public bool AllowDuplicates {
			get { return allowDuplicates; }
			set { allowDuplicates = value; }
		}*/

		// Save & Save Settings -------------------------------------------------------

		/// <summary>Gets or sets if formatted lines use escape characters even when
		/// unnecessary.</summary>
		public bool StrictFormatting {
			get { return strictFormatting; }
			set { strictFormatting = value; }
		}

		/// <summary>Gets or sets if characters are escaped.</summary>
		public bool EscapeEnabled {
			get { return escapeEnabled; }
			set { escapeEnabled = value; }
		}
	}
}

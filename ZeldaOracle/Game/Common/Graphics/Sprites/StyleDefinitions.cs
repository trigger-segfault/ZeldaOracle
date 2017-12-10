using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A definition of a style group and the style in the group to use.</summary>
	public struct StyleDefinition {
		/// <summary>The style group for this definition.</summary>
		public string Group { get; set; }
		/// <summary>The style in the group to be used.</summary>
		public string Style { get; set; }

		/// <summary>Constructs the style definition.</summary>
		public StyleDefinition(string group, string style) {
			this.Group	= group;
			this.Style	= style;
		}
	}
	/// <summary>A collection of style definitions for what styles to use.</summary>
	public class StyleDefinitions {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>An empty collection of style definitions.</summary>
		public static readonly StyleDefinitions Empty = new StyleDefinitions();

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The collection of style definitions.</summary>
		private Dictionary<string, string> styles;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a new collection of style definitions.</summary>
		public StyleDefinitions() {
			this.styles     = new Dictionary<string, string>();
		}

		/// <summary>Constructs a copy of the collection of style definitions.</summary>
		public StyleDefinitions(StyleDefinitions copy) {
			this.styles     = new Dictionary<string, string>(copy.styles);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the collection of style definitions.</summary>
		public IEnumerable<StyleDefinition> GetStyles() {
			foreach (var pair in styles) {
				yield return new StyleDefinition(pair.Key, pair.Value);
			}
		}

		/// <summary>Gets the style definition for the specified group.
		/// <para>Returns null if that style group is not defined.</para></summary>
		public string GetStyle(string group) {
			if (group == null)
				throw new ArgumentNullException("Style group cannot be null!");
			string style;
			styles.TryGetValue(group, out style);
			return style;
		}

		/// <summary>Returns true if a style for the specified group is defined.</para></summary>
		public bool ContainsStyle(string group) {
			if (group == null)
				throw new ArgumentNullException("Style group cannot be null!");
			return styles.ContainsKey(group);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		/// <summary>Sets the style definition for the specified group.</summary>
		public void SetStyle(string group, string style) {
			if (group == null)
				throw new ArgumentNullException("Style group cannot be null!");
			if (style == null)
				throw new ArgumentNullException("Style cannot be null!");
			styles[group] = style;
		}

		/// <summary>Removes the style definition for the specified group.</summary>
		public void RemoveStyle(string group) {
			if (group == null)
				throw new ArgumentNullException("Style group cannot be null!");
			styles.Remove(group);
		}
	}
}

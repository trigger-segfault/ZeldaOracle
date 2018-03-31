using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Content {
	/// <summary>A class for storing information about a style group.</summary>
	public class StyleGroupCollection {
		/// <summary>The style group for the contained styles.</summary>
		public string Group { get; }
		/// <summary>The collection of styles for the style group.</summary>
		public HashSet<string> Styles { get; }
		/// <summary>The default style for the styleGroup.</summary>
		public string DefaultStyle { get; set; }
		/// <summary>The preview for the style group.</summary>
		public ISprite Preview { get; set; }

		/// <summary>Gets if the style group has a preview.</summary>
		public bool HasPreview {
			get { return Preview != null; }
		}
		/// <summary>Gets if the style group has any styles.</summary>
		public bool HasStyles {
			get { return Styles.Any(); }
		}

		/// <summary>Constructs the style group collection.</summary>
		public StyleGroupCollection(string group, StyleSprite preview) {
			Group           = group;
			Styles          = new HashSet<string>();
			DefaultStyle    = "";
			Preview         = preview;
		}
	}
}

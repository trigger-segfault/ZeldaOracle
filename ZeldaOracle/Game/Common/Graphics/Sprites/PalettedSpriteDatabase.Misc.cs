using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {

	/// <summary>A pre-looked up mapped color for a color group's subtype.</summary>
	public struct ColorGroupSubtypePair {
		/// <summary>The color group for the mapping.</summary>
		public string ColorGroup { get; set; }
		/// <summary>The color group's subtype for the mapping.</summary>
		public LookupSubtypes Subtype { get; set; }
		/// <summary>The output mapped color.</summary>
		public Color MappedColor { get; set; }

		/// <summary>Constructs the color group subtype pair.</summary>
		public ColorGroupSubtypePair(string colorGroup, LookupSubtypes subtype, PaletteDictionary dictionary) {
			this.ColorGroup     = colorGroup;
			this.Subtype        = subtype;
			// Store the map color during creation.
			this.MappedColor	= dictionary.GetMappedColor(colorGroup, subtype);
		}
	}

	/// <summary>Arguments used for palettizing a portion of an image.</summary>
	public struct SpritePaletteArgs {
		/// <summary>The image to get the sprite from.</summary>
		public Image Image { get; set; }
		/// <summary>The source rect for the sprite in the image.</summary>
		public Rectangle2I SourceRect { get; set; }
		/// <summary>The color group mappings available for
		/// each color.</summary>
		public Dictionary<Color, Dictionary<int, ColorGroupSubtypePair>> ColorMapping { get; set; }
		/// <summary>An array with the index of each color group.</summary>
		public int[] IndexedPossibleColorGroups { get; set; }
		/// <summary>The names of each available color group.</summary>
		public string[] PossibleColorGroups { get; set; }
		/// <summary>Colors to ignore and keep unmapped.</summary>
		public HashSet<Color> IgnoreColors { get; set; }
		/// <summary>The palette dictionary to reference color
		/// groups from.</summary>
		public PaletteDictionary Dictionary { get; set; }
		/// <summary>The size of chunks when palettizing a sprite.
		/// Use zero for full size.</summary>
		public Point2I ChunkSize { get; set; }
		/// <summary>The color mapping the sprite will be forced to
		/// after determining its palette.</summary>
		public Color[] DefaultMapping { get; set; }
	}

	/// <summary>An exception thrown when a color is not recognized
	/// for the palettizer.</summary>
	public class UnspecifiedColorException : Exception {
		public Color Color { get; set; }
		public Point2I Point { get; set; }

		public UnspecifiedColorException(Color color, Point2I point) :
			base("Unspecified color " + color + " at " + point + "!")
		{
			this.Color = color;
			this.Point = point;
		}
	}

	/// <summary>An exception thrown when no color group was listed
	/// that uses the discovered colors.</summary>
	public class NoMatchingColorGroupsException : Exception {
		public HashSet<Color> Colors { get; set; }
		public Point2I Point { get; set; }

		public NoMatchingColorGroupsException(HashSet<Color> colors, Point2I point) :
			base("No matching color group with the following colors: " +
				ListColors(colors) + " at the point " + point + "!")
		{
			this.Colors = colors;
			this.Point = point;
		}

		private static string ListColors(HashSet<Color> colors) {
			string str = "";
			bool first = true;
			foreach (Color color in colors) {
				if (!first)
					str += ", ";
				str += color;
				first = true;
			}
			return str;
		}
	}

	/// <summary>An exception thrown when the preloaded sprite database does not
	/// match the sprites created in the conscripts.</summary>
	public class SpriteDatabaseException : Exception {
		public SpriteDatabaseException(string message) : base(message) { }
	}
}

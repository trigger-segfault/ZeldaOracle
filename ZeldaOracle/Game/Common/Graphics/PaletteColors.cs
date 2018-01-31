using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Graphics {

	/// <summary>A reference to a palette's color.</summary>
	public struct PaletteReference {

		/// <summary>The palette's color group to use.</summary>
		public string ColorGroup { get; set; }
		/// <summary>The palette color group's subtype to use.</summary>
		public LookupSubtypes Subtype { get; set; }
		/// <summary>The palette type to use.</summary>
		public PaletteTypes Type { get; set; }


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a palette reference with the specified
		/// color group, subtype, and palette type.</summary>
		public PaletteReference(string colorGroup, LookupSubtypes subtype, PaletteTypes type = PaletteTypes.Entity) {
			this.ColorGroup     = colorGroup;
			this.Subtype        = subtype;
			this.Type           = type;
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Gets the hash code for the palette reference.</summary>
		public override int GetHashCode() {
			return Type.GetHashCode() ^ ColorGroup.GetHashCode() ^ Subtype.GetHashCode();
		}

		/// <summary>Returns true if the palette reference is equal to the object.</summary>
		public override bool Equals(object obj) {
			if (obj is PaletteReference)
				return this == ((PaletteReference) obj);
			return false;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static bool operator ==(PaletteReference a, PaletteReference b) {
			return a.Type == b.Type &&
				a.ColorGroup == b.ColorGroup &&
				a.Subtype == b.Subtype;
		}

		public static bool operator !=(PaletteReference a, PaletteReference b) {
			return a.Type != b.Type ||
				a.ColorGroup != b.ColorGroup ||
				a.Subtype != b.Subtype;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the final mapped color with the chosen tile and entity palette.</summary>
		public Color GetMappedColor(Palette tilePalette, Palette entityPalette) {
			if (Type == PaletteTypes.Tile)
				return tilePalette.PaletteDictionary.GetMappedColor(ColorGroup, Subtype);
			else if (Type == PaletteTypes.Entity)
				return entityPalette.PaletteDictionary.GetMappedColor(ColorGroup, Subtype);
			return Color.Black;
		}

		/// <summary>Gets the final unmapped color with the chosen tile and entity palette.</summary>
		public Color GetUnmappedColor(Palette tilePalette, Palette entityPalette) {
			if (Type == PaletteTypes.Tile)
				return tilePalette.LookupColor(ColorGroup, Subtype);
			else if (Type == PaletteTypes.Entity)
				return entityPalette.LookupColor(ColorGroup, Subtype);
			return Color.Black;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the final mapped color using the palette shader's current palettes
		/// This should only be used during a draw function.</summary>
		public Color MappedColor {
			get {
				if (Type == PaletteTypes.Entity)
					return GameData.PaletteShader.EntityPalette.PaletteDictionary.GetMappedColor(ColorGroup, Subtype);
				else if (Type == PaletteTypes.Tile)
					return GameData.PaletteShader.TilePalette.PaletteDictionary.GetMappedColor(ColorGroup, Subtype);
				return Color.Black;
			}
		}

		/// <summary>Gets the final unmapped color using the palette shader's current palettes
		/// This should only be used during a draw function.</summary>
		public Color UnmappedColor {
			get {
				if (Type == PaletteTypes.Entity)
					return GameData.PaletteShader.EntityPalette.LookupColor(ColorGroup, Subtype);
				else if (Type == PaletteTypes.Tile)
					return GameData.PaletteShader.TilePalette.LookupColor(ColorGroup, Subtype);
				return Color.Black;
			}
		}
	}

	/// <summary>A structure that contains either a color or palette reference.</summary>
	public struct ColorOrPalette {

		/// <summary>The object storing either a color or palette reference.
		/// This should never be null.</summary>
		private object value;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a color.</summary>
		public ColorOrPalette(Color color) {
			value = color;
		}

		/// <summary>Constructs a palette color.</summary>
		public ColorOrPalette(PaletteReference paletteColor) {
			value = paletteColor;
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Gets the hash code for the color or palette.</summary>
		public override int GetHashCode() {
			return value.GetHashCode();
		}

		/// <summary>Returns true if the object is equal to the color or palette.
		/// Works with Color, PaletteReference, and ColorOrPalette.</summary>
		public override bool Equals(object obj) {
			if (obj is Color)
				return value.Equals(obj);
			else if (obj is PaletteReference)
				return value.Equals(obj);
			else if (obj is ColorOrPalette)
				return this == ((ColorOrPalette) obj);
			return false;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static bool operator ==(ColorOrPalette a, ColorOrPalette b) {
			return a.value.Equals(b.value);
		}

		public static bool operator !=(ColorOrPalette a, ColorOrPalette b) {
			return !a.value.Equals(b.value);
		}


		//-----------------------------------------------------------------------------
		// Implicit Conversion
		//-----------------------------------------------------------------------------

		public static implicit operator ColorOrPalette(Color color) {
			return new ColorOrPalette(color);
		}

		public static implicit operator ColorOrPalette(PaletteReference paletteColor) {
			return new ColorOrPalette(paletteColor);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the final (mapped) color with the chosen tile and entity palette.</summary>
		public Color GetMappedColor(Palette tilePalette, Palette entityPalette) {
			if (value is PaletteReference)
				return ((PaletteReference) value).GetMappedColor(tilePalette, entityPalette);
			else
				return (Color) value;
		}

		/// <summary>Gets the final (unmapped) color with the chosen tile and entity palette.</summary>
		public Color GetUnmappedColor(Palette tilePalette, Palette entityPalette) {
			if (value is PaletteReference)
				return ((PaletteReference) value).GetUnmappedColor(tilePalette, entityPalette);
			else
				return (Color) value;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the final (mapped) color using the palette shader's current palettes
		/// This should only be used during a draw function.</summary>
		public Color MappedColor {
			get {
				if (value is PaletteReference)
					return ((PaletteReference) value).MappedColor;
				else
					return (Color) value;
			}
		}

		/// <summary>Gets the final (unmapped) color using the palette shader's current palettes
		/// This should only be used during a draw function.</summary>
		public Color UnmappedColor {
			get {
				if (value is PaletteReference)
					return ((PaletteReference) value).UnmappedColor;
				else
					return (Color) value;
			}
		}

		/// <summary>Returns true if this is a color.</summary>
		public bool IsColor {
			get { return value is Color; }
		}

		/// <summary>Returns true if this is a palette reference.</summary>
		public bool IsPalette {
			get { return value is PaletteReference; }
		}
	}
}

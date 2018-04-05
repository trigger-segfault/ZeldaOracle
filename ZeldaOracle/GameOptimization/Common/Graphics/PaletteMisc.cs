
namespace ZeldaOracle.Common.Graphics {
	/// <summary>The type of object supported by the palette and palette dictionary.</summary>
	public enum PaletteTypes {
		/// <summary>The palette type is invalid.</summary>
		None,
		/// <summary>The palette type is for tiles.</summary>
		Tile,
		/// <summary>The palette type is for entities.</summary>
		Entity,
	}

	/// <summary>The subtypes to use for looking up colors in color groups.</summary>
	public enum LookupSubtypes {
		/// <summary>Used for tile and entities.</summary>
		Light = 0,
		/// <summary>Used for tiles only.</summary>
		Medium = 1,
		/// <summary>Used for entities only.</summary>
		Transparent = 1,
		/// <summary>Used for tile and entities.</summary>
		Dark = 2,
		/// <summary>Used for tile and entities.</summary>
		Black = 3,
		/// <summary>Used to assign all tile and entity colors in the group.</summary>
		All,
	}

	/// <summary>A nullable palette color.</summary>
	public struct PaletteColor {

		/// <summary>An undefined palette color.</summary>
		public static readonly PaletteColor Undefined = new PaletteColor();

		/// <summary>The internal nullable color.</summary>
		private Color? color;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the palette color.</summary>
		public PaletteColor(Color color) {
			this.color      = color;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the palette color is undefined.</summary>
		public bool IsUndefined {
			get { return !color.HasValue; }
		}

		/// <summary>Gets the palette color.</summary>
		public Color Color {
			get {
				if (color.HasValue)
					return color.Value;
				return Color.Black;
			}
			set { color = value; }
		}
	}

	/// <summary>A nullable lookup color group and subtype.</summary>
	public struct LookupPair {

		/// <summary>An undefined lookup pair.</summary>
		public static readonly LookupPair Undefined = new LookupPair();

		/// <summary>The name of the lookup color group.</summary>
		public string Name { get; set; }
		/// <summary>The subtype of the lookup color group.</summary>
		public LookupSubtypes Subtype { get; set; }


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the lookup pair.</summary>
		public LookupPair(string name, LookupSubtypes subtype) {
			Name	= name;
			Subtype	= subtype;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the lookup pair is undefined.</summary>
		public bool IsUndefined {
			get { return Name == null; }
		}
	}
}

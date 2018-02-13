using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {

	/// <summary>The type of object supported by the palette and palette dictionary.</summary>
	public enum PaletteTypes {
		None,
		Tile,
		Entity
	};

	/// <summary>A dictionary for assigning names to palette indecies. The purpose of
	/// this dictionary is for palette setup at the beginning of the game.</summary>
	public class PaletteDictionary {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The number of colors in a palette color group.</summary>
		public const int ColorGroupSize = 4;

		/// <summary>The alpha value used to identify a mapped color.</summary>
		public const byte AlphaIdentifier = 254;

		/// <summary>The maximum available color groups per palette.</summary>
		public const int MaxColorGroups = 256*256 / ColorGroupSize;

		/// <summary>The dimensions of the palette image.</summary>
		public static readonly Point2I Dimensions = new Point2I(256, 256);

		/// <summary>The blue color identifying the use of the tile palette.</summary>
		public const byte TileBlue = 0;
		/// <summary>The blue color identifying the use of the entity palette.</summary>
		public const byte EntityBlue = 1;

		/// <summary>The default color group for tiles.</summary>
		public static readonly Color[] DefaultTile = new Color[4] {
			Color.ToGBCColor(Color.White),
			Color.ToGBCColor(0.66f, 0.66f, 0.66f),
			Color.ToGBCColor(0.33f, 0.33f, 0.33f),
			Color.Black
		};

		/// <summary>The default color group for enitities.</summary>
		public static readonly Color[] DefaultEntity = new Color[4] {
			Color.Transparent,
			Color.ToGBCColor(0.66f, 0.66f, 0.66f),
			Color.ToGBCColor(0.33f, 0.33f, 0.33f),
			Color.Black
		};


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The parent palette dictionary.</summary>
		private PaletteDictionary parentDictionary;
		/// <summary>Index is a multiple of four.</summary>
		private Dictionary<string, int> indexMap;
		/// <summary>The type of object supported by the palette and palette dictionary.</summary>
		private PaletteTypes type;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an empty palette dictionary.</summary>
		public PaletteDictionary(PaletteTypes type) {
			this.parentDictionary   = null;
			this.indexMap			= new Dictionary<string, int>();
			this.type               = type;

			switch (type) {
			case PaletteTypes.Tile:
				indexMap.Add("default", 0);
				break;
			case PaletteTypes.Entity:
				indexMap.Add("default", 0);
				break;
			default:
				throw new ArgumentException("Invalid PaletteType");
			}
		}

		/// <summary>Constructs a copy of the palette dictionary.</summary>
		public PaletteDictionary(PaletteDictionary copy) {
			this.parentDictionary   = copy.parentDictionary;
			this.indexMap			= new Dictionary<string, int>(copy.indexMap);
			this.type               = copy.type;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the collection of index mappings.</summary>
		public IEnumerable<KeyValuePair<string, int>> GetDictionary() {
			foreach (var pair in indexMap) {
				yield return pair;
			}
			if (parentDictionary != null) {
				foreach (var pair in parentDictionary.indexMap) {
					yield return pair;
				}
			}
		}

		/// <summary>Gets the collection of keys.</summary>
		public IEnumerable<string> GetKeys() {
			foreach (var key in indexMap.Keys) {
				yield return key;
			}
			if (parentDictionary != null) {
				foreach (var key in parentDictionary.indexMap.Keys) {
					yield return key;
				}
			}
		}

		/// <summary>Gets the index mapping with the specified name.</summary>
		public int Get(string name) {
			int index;
			if (indexMap.TryGetValue(name, out index)) {
				return index;
			}
			else if (parentDictionary != null &&
				parentDictionary.indexMap.TryGetValue(name, out index))
			{
				return index;
			}
			return -1;
		}

		/// <summary>Returns true if an index mapping with the specified name exists.</summary>
		public bool Contains(string name) {
			if (indexMap.ContainsKey(name)) {
				return true;
			}
			else if (parentDictionary != null &&
				parentDictionary.indexMap.ContainsKey(name))
			{
				return true;
			}
			return false;
		}

		/// <summary>Gets the color used to map to the specified palette color.</summary>
		public Color GetMappedColor(string name, LookupSubtypes subtype) {
			Color color = new Color();
			// Marks this color as a paletted color
			color.A = AlphaIdentifier;

			// The lookup index of the palette color
			int index = Get(name) + (int)subtype;
			color.R = (byte) (index % Dimensions.X);
			color.G = (byte) (index / Dimensions.Y);

			// Is this a tile or entity palette color
			if (type == PaletteTypes.Tile)
				color.B = TileBlue;
			else if (type == PaletteTypes.Entity)
				color.B = EntityBlue;
			return color;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds a new index mapping for a name that hasn't been defined yet.</summary>
		public void Add(string name, int index) {
			indexMap.Add(name, index);
		}

		/// <summary>Sets the index mapping for the name.</summary>
		public void Set(string name, int index) {
			indexMap[name] = index;
		}

		/// <summary>Removes the index mapping with the specified name.</summary>
		public void Remove(string name) {
			indexMap.Remove(name);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the index mapping with the specified name.</summary>
		public int this[string name] {
			get { return Get(name); }
			set { Set(name, value); }
		}

		/// <summary>Gets or sets the global palette dictionary.</summary>
		public PaletteDictionary ParentDictionary {
			get { return parentDictionary; }
			set { parentDictionary = value; }
		}

		/// <summary>Gets the type of the palette dictionary.</summary>
		public PaletteTypes PaletteType {
			get { return type; }
		}
	}
}

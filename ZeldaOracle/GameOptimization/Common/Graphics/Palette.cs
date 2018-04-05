using System;
using System.Collections.Generic;

using XnaColor = Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A class mapping colors from a palette dictionary to an image for use
	/// with the palette shader.</summary>
	public class Palette {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The number of colors in a palette color group.</summary>
		public const int ColorGroupSize = PaletteDictionary.ColorGroupSize;

		/// <summary>The total number of colors in a palette.</summary>
		public static readonly Point2I Dimensions = new Point2I(256, 256);

		/// <summary>The default color group for tiles.</summary>
		public static readonly Color[] DefaultTile = new Color[ColorGroupSize] {
			Color.ToGBCColor(Color.White),
			Color.ToGBCColor(0.66f, 0.66f, 0.66f),
			Color.ToGBCColor(0.33f, 0.33f, 0.33f),
			Color.Black
		};

		/// <summary>The default color group for enitities.</summary>
		public static readonly Color[] DefaultEntity = new Color[ColorGroupSize] {
			Color.ToGBCColor(Color.White),
			Color.Transparent,
			Color.ToGBCColor(0.5f, 0.5f, 0.5f),
			Color.Black
		};

		/// <summary>Gets the range of available lookup subtypes.</summary>
		public static IEnumerable<LookupSubtypes> LookupSubtypeRange {
			get {
				for (int i = 0; i < ColorGroupSize; i++)
					yield return (LookupSubtypes) i;
			}
		}

		/// <summary>The maximum allowed lookups before reaching a defined color.</summary>
		public const int MaxLookupDepth = 20;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The palette dictionary used for this palette.</summary>
		private PaletteDictionary dictionary;
		/// <summary>The image storing the lookup information for the palette shader.</summary>
		private Image paletteImage;
		/// <summary>The defined colors for color groups.</summary>
		private Dictionary<string, PaletteColor[]> colorGroups;
		/// <summary>The defined colors for constants.</summary>
		private Dictionary<string, PaletteColor[]> constColorGroups;
		/// <summary>The defined lookups for color groups.</summary>
		private Dictionary<string, LookupPair[]> lookupGroups;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Creates a new palette.</summary>
		public Palette(PaletteDictionary dictionary) {
			this.dictionary			= dictionary;

			paletteImage		= new Image(Dimensions);
			colorGroups			= new Dictionary<string, PaletteColor[]>();
			constColorGroups	= new Dictionary<string, PaletteColor[]>();
			lookupGroups		= new Dictionary<string, LookupPair[]>();
			
			if (dictionary.PaletteType == PaletteTypes.Tile)
				colorGroups.Add("default", ColorGroupToPaletteGroup(DefaultTile));
			else if (dictionary.PaletteType == PaletteTypes.Entity)
				colorGroups.Add("default", ColorGroupToPaletteGroup(DefaultEntity));
		}

		/// <summary>Constructs a copy of the palette.</summary>
		public Palette(Palette copy) {
			dictionary			= copy.dictionary;

			paletteImage		= new Image(Dimensions);
			colorGroups			= new Dictionary<string, PaletteColor[]>();
			constColorGroups	= new Dictionary<string, PaletteColor[]>();
			lookupGroups		= new Dictionary<string, LookupPair[]>();

			foreach (var pair in copy.colorGroups)
				colorGroups.Add(pair.Key, (PaletteColor[]) pair.Value.Clone());
			foreach (var pair in copy.constColorGroups)
				constColorGroups.Add(pair.Key, (PaletteColor[]) pair.Value.Clone());
			foreach (var pair in copy.lookupGroups)
				lookupGroups.Add(pair.Key, (LookupPair[]) pair.Value.Clone());
		}


		//-----------------------------------------------------------------------------
		// Disposing
		//-----------------------------------------------------------------------------

		/// <summary>Disposes of the palette's texture.</summary>
		public void Dispose() {
			if (paletteImage != null)
				paletteImage.Dispose();
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		/// <summary>Updates the shader texture with the current palette.</summary>
		public void UpdatePalette() {
			XnaColor[] colorData = paletteImage.CreateData();
			Color[] defaultGroup = PaletteGroupToColorGroup(colorGroups["default"]);
			for (int i = 0; i < PaletteDictionary.MaxColorGroups; i++) {
				int index = i * ColorGroupSize;
				for (int j = 0; j < ColorGroupSize; j++) {
					colorData[index + j] = defaultGroup[j].ToXnaColor();
				}
			}
			foreach (var pair in dictionary.GetDictionary()) {
				int index = pair.Value;
				for (int i = 0; i < ColorGroupSize; i++) {
					colorData[index + i] =
						LookupColor(pair.Key, (LookupSubtypes) i).ToXnaColor();
				}
			}
			paletteImage.SetData(colorData);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Looks up the color with the specified name and subtype.</summary>
		public Color LookupColor(string name, LookupSubtypes subtype, bool safe = false) {
			if (!dictionary.Contains(name) && !constColorGroups.ContainsKey(name)) {
				if (safe)
					return Color.Black;
				throw new ArgumentException("Unknown color group '" + name + "'!");
			}
			while (lookupGroups.ContainsKey(name)) {
				LookupPair lookupPair = lookupGroups[name][(int) subtype];
				if (!lookupPair.IsUndefined) {
					name = lookupPair.Name;
					subtype = lookupPair.Subtype;
				}
				else {
					break;
				}
			}
			PaletteColor[] colorGroup;
			if (constColorGroups.TryGetValue(name, out colorGroup)) {
				PaletteColor color = colorGroup[(int) subtype];
				if (!color.IsUndefined)
					return color.Color;
			}
			else if (colorGroups.TryGetValue(name, out colorGroup)) {
				PaletteColor color = colorGroup[(int) subtype];
				if (!color.IsUndefined)
					return color.Color;
			}
			return colorGroups["default"][(int) subtype].Color;
		}

		/// <summary>Gets the collection of defined constant colors.</summary>
		public IEnumerable<KeyValuePair<string, PaletteColor[]>> GetDefinedConsts() {
			foreach (var pair in constColorGroups) {
				yield return pair;
			}
		}

		/// <summary>Gets the collection of defined colors.</summary>
		public IEnumerable<KeyValuePair<string, PaletteColor[]>> GetDefinedColors() {
			foreach (var pair in colorGroups) {
				yield return pair;
			}
		}

		/// <summary>Gets the collection of defined color lookups.</summary>
		public IEnumerable<KeyValuePair<string, LookupPair[]>> GetDefinedLookups() {
			foreach (var pair in lookupGroups) {
				yield return pair;
			}
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the name and subtype to a lookup.</summary>
		public void SetLookup(string name, LookupSubtypes subtype, string lookupName,
			LookupSubtypes lookupSubtype)
		{
			if (subtype != LookupSubtypes.All && lookupSubtype == LookupSubtypes.All)
				throw new ArgumentException("Mismatch, cannot assign a single " +
					"color to a lookup subtype of all.");
			if (name == lookupName &&
				(subtype == lookupSubtype || subtype == LookupSubtypes.All))
				throw new ArgumentException("Infinite loop detected in color " +
					"lookup!");
			if (!dictionary.Contains(name))
				throw new ArgumentException("Unknown color group '" + name + "'!");
			if (constColorGroups.ContainsKey(name))
				throw new ArgumentException("Cannot set lookup for const color " +
					"group '" + name + "'!");

			// Do a for loop only if lookupSubtype is All
			LookupSubtypes i =
				(lookupSubtype == LookupSubtypes.All ? 0 : lookupSubtype);
			for (; i < LookupSubtypes.All; i++) {
				string nextName = lookupName;
				LookupSubtypes nextSubtype = i;
				int depth = 0;
				while (lookupGroups.ContainsKey(nextName)) {
					LookupPair lookupPair = lookupGroups[nextName][(int) nextSubtype];
					if (!lookupPair.IsUndefined) {
						if (depth == MaxLookupDepth) {
							throw new ArgumentException("Maximum lookup depth of " +
								MaxLookupDepth + " hit!");
						}
						if (lookupPair.Name == name) {
							if (subtype == LookupSubtypes.All) {
								if (lookupSubtype == LookupSubtypes.All)
									throw new ArgumentException("Infinite loop " +
										"detected in color lookup!");
							}
							else if (lookupPair.Subtype == subtype)
								throw new ArgumentException("Infinite loop detected " +
									"in color lookup!");
						}
						nextName = lookupPair.Name;
						nextSubtype = lookupPair.Subtype;
						depth++;
					}
					else {
						break;
					}
				}
				if (lookupSubtype != LookupSubtypes.All)
					break;
			}

			LookupPair[] lookupGroup;
			PaletteColor[] colorGroup;
			colorGroups.TryGetValue(name, out colorGroup);
			if (!lookupGroups.TryGetValue(name, out lookupGroup)) {
				lookupGroup = new LookupPair[ColorGroupSize];
				lookupGroups[name] = lookupGroup;
			}
			if (subtype != LookupSubtypes.All) {
				lookupGroup[(int) subtype] = new LookupPair(lookupName, lookupSubtype);
				// Check if color group is unreferenced and we can remove it
				if (colorGroup != null) {
					colorGroup[(int) subtype] = PaletteColor.Undefined;

					foreach (var paletteColor in colorGroup) {
						if (!paletteColor.IsUndefined) // Nope, end the function here
							return;
					}

					colorGroups.Remove(name);
				}
			}
			else {
				foreach (LookupSubtypes j in LookupSubtypeRange) {
				//for (LookupSubtypes j = 0; j < LookupSubtypes.All; j++) {
					if (lookupSubtype == LookupSubtypes.All)
						lookupGroup[(int) j] = new LookupPair(lookupName, j);
					else
						lookupGroup[(int) j] = new LookupPair(lookupName,
							lookupSubtype);
				}

				// Remove the unreferenced color group
				if (colorGroup != null) {
					colorGroups.Remove(name);
				}
			}

			return;
		}

		/// <summary>Sets the name and subtype to a lookup.</summary>
		public void CopyLookup(string name, LookupSubtypes subtype, string lookupName,
			LookupSubtypes lookupSubtype)
		{
			if (subtype != LookupSubtypes.All && lookupSubtype == LookupSubtypes.All)
				throw new ArgumentException("Mismatch, cannot assign a single color " +
					"to a lookup subtype of all.");
			if (name == lookupName &&
				(subtype == lookupSubtype || subtype == LookupSubtypes.All))
				throw new ArgumentException("Infinite loop detected in color copy!");
			bool isConst = constColorGroups.ContainsKey(name);
			if (!dictionary.Contains(name) && !isConst)
				throw new ArgumentException("Unknown color group '" + name + "'!");

			LookupPair[] lookupGroup;
			PaletteColor[] colorGroup = null;
			if (isConst) {
				colorGroup = constColorGroups[name];
			}
			else if (!colorGroups.TryGetValue(name, out colorGroup)) {
				colorGroup = new PaletteColor[ColorGroupSize];
				colorGroups[name] = colorGroup;
			}
			lookupGroups.TryGetValue(name, out lookupGroup);
			if (subtype != LookupSubtypes.All) {
				colorGroup[(int) subtype].Color =
					LookupColor(lookupName, lookupSubtype);

				// Check if lookup group is unreferenced and we can remove it
				if (lookupGroup != null) {
					lookupGroup[(int) subtype] = LookupPair.Undefined;

					foreach (var lookupPair in lookupGroup) {
						if (!lookupPair.IsUndefined) // Nope, end the function here
							return;
					}

					lookupGroups.Remove(name);
				}
			}
			else {
				foreach (var j in LookupSubtypeRange) {
				//for (LookupSubtypes j = 0; j < LookupSubtypes.All; j++) {
					if (lookupSubtype == LookupSubtypes.All)
						colorGroup[(int) j].Color = LookupColor(lookupName, j);
					else
						colorGroup[(int) j].Color = LookupColor(lookupName,
							lookupSubtype);
				}

				// Remove the unreferenced lookup group
				if (lookupGroup != null) {
					lookupGroups.Remove(name);
				}
			}

			return;
		}

		/// <summary>Sets the name and subtype to a color.</summary>
		public void SetColor(string name, LookupSubtypes subtype, Color color) {
			if (!dictionary.Contains(name) && !constColorGroups.ContainsKey(name))
				throw new ArgumentException("Unknown color group '" + name + "'!");
			PaletteColor[] colorGroup;
			if (!constColorGroups.TryGetValue(name, out colorGroup) &&
				!colorGroups.TryGetValue(name, out colorGroup))
			{
				colorGroup = new PaletteColor[ColorGroupSize];
				colorGroups[name] = colorGroup;
			}
			LookupPair[] lookupGroup;
			lookupGroups.TryGetValue(name, out lookupGroup);

			if (subtype != LookupSubtypes.All) {
				colorGroup[(int) subtype].Color = color;
				
				// Check if lookup group is unreferenced and we can remove it
				if (lookupGroup != null) {
					lookupGroup[(int) subtype] = LookupPair.Undefined;
					
					foreach (var lookupPair in lookupGroup) {
						if (!lookupPair.IsUndefined) // Nope, end the function here
							return;
					}

					lookupGroups.Remove(name);
				}
			}
			else {
				for (int i = 0; i < ColorGroupSize; i++) {
					colorGroup[i].Color = color;
				}
				if (lookupGroup != null)
					lookupGroups.Remove(name);
			}
		}

		/// <summary>Resets and removes the lookup from the color.</summary>
		public void Reset(string name, LookupSubtypes subtype) {
			if (!dictionary.Contains(name))
				throw new ArgumentException("Unknown color group '" + name + "'!");
			if (constColorGroups.ContainsKey(name))
				throw new ArgumentException("Cannot reset const color group '" +
					name + "'!");
			LookupPair[] lookupGroup;
			if (lookupGroups.TryGetValue(name, out lookupGroup)) {
				if (subtype == LookupSubtypes.All) {
					for (int i = 0; i < ColorGroupSize; i++)
						lookupGroup[i] = LookupPair.Undefined;
				}
				else {
					lookupGroup[(int) subtype] = LookupPair.Undefined;
				}
			}
		}

		/// <summary>Adds a custom constant color group.</summary>
		public void AddConst(string name) {
			if (dictionary.Contains(name))
				throw new ArgumentException("Color group '" + name +
					"' already exists!");
			if (!constColorGroups.ContainsKey(name))
				constColorGroups.Add(name, new PaletteColor[ColorGroupSize]);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Converts a color group to a palette color group.</summary>
		private PaletteColor[] ColorGroupToPaletteGroup(Color[] colorGroup) {
			PaletteColor[] paletteGroup = new PaletteColor[ColorGroupSize];
			for (int i = 0; i < ColorGroupSize; i++) {
				paletteGroup[i].Color = colorGroup[i];
			}
			return paletteGroup;
		}

		/// <summary>Converts a palette color group to a color group.</summary>
		private Color[] PaletteGroupToColorGroup(PaletteColor[] paletteGroup) {
			Color[] colorGroup = new Color[ColorGroupSize];
			for (int i = 0; i < ColorGroupSize; i++) {
				colorGroup[i] = paletteGroup[i].Color;
			}
			return colorGroup;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the palette texture used for the shader.</summary>
		public Image PaletteImage {
			get { return paletteImage; }
		}

		/// <summary>Gets the palette's local index mapping dictionary.</summary>
		public PaletteDictionary PaletteDictionary {
			get { return dictionary; }
		}

		/// <summary>Gets the defined type of the palette dictionary.</summary>
		public PaletteTypes PaletteType {
			get { return dictionary.PaletteType; }
		}
	}
}

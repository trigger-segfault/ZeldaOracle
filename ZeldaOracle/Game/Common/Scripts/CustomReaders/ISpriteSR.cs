using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripts.Commands;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripts.CustomReaders {

	public partial class ISpriteSR : ScriptReader {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------

		private struct ColorStyleGroup {
			public string StyleGroup { get; set; }
			public string ColorGroup { get; set; }

			public ColorStyleGroup(string styleGroup, string colorGroup) {
				this.StyleGroup	= styleGroup;
				this.ColorGroup	= colorGroup;
			}
		}

		private enum SourceModes {
			None,
			SpriteSheet,
			SpriteSet
		}

		[Flags]
		private enum Modes {
			Root			= 0,
			SpriteSet		= 1 << 0,
			EmptySprite		= 1 << 1,
			BasicSprite		= 1 << 2,
			OffsetSprite	= 1 << 3,
			StyleSprite		= 1 << 4,
			ColorSprite		= 1 << 5,
			StyleColorSprite= 1 << 6,
			CompositeSprite	= 1 << 7,
			Animation		= 1 << 8,
			MultiStyle		= 1 << 9,
			SpriteMask		= EmptySprite | BasicSprite | OffsetSprite |
							  StyleSprite | ColorSprite | StyleColorSprite |
							  CompositeSprite | Animation | MultiStyle
		}

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------
		
		private string spriteName;
		private AnimationBuilder animationBuilder;
		
		private SpritePaletteArgs paletteArgs;
		
		private SpriteSet editingSpriteSet;
		private Point2I editingSetStart;
		private Point2I editingSetDimensions;

		// ColorStyle
		private string editingStyleGroup;
		private List<string> editingColorGroups;
		private List<ColorStyleGroup> editingColorStyleGroups;
		
		private ISpriteSource source;
		private ISprite sprite;

		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public ISpriteSR() {
			
			this.source         = null;
			this.sprite         = null;
			this.paletteArgs.ColorMapping = new Dictionary<Color, Dictionary<int, ColorGroupSubtypePair>>();
			this.paletteArgs.IndexedPossibleColorGroups = new int[0];
			this.paletteArgs.PossibleColorGroups = new string[0];
			this.paletteArgs.IgnoreColors = new HashSet<Color>();
			this.animationBuilder = new AnimationBuilder();
			this.animationBuilder.PaletteArgs = paletteArgs;
			this.editingColorGroups = new List<string>();
			this.editingColorStyleGroups = new List<ColorStyleGroup>();

			//=====================================================================================
			// Type Definitions
			//=====================================================================================
			AddType("Sprite",
				"string spriteName",
				// Int needs to go before string as int/float defaults to string.
				"Point sourceIndex",
				"(string animationName, int substrip)",
				"(string definitionSpriteName, string definition)",
				"(Point definitionSourceIndex, string definition)",
				"(string sourceName, Point sourceIndex)",
				"(string sourceName, Point sourceIndex, string definition)"
			);
			//=====================================================================================
			// Prefixes
			//=====================================================================================
			// Continues the sprite with the existing name.
			//AddCommandPrefix("CONTINUE", (int) Modes.Root);
			// Used to initialize a grid in a spriteset when starting a sprite.
			//AddCommandPrefix("DYNAMIC", (int) Modes.SpriteSet);
			// Used to initialize a singlular in a spriteset when starting a sprite.
			//AddCommandPrefix("SINGLE", (int) Modes.SpriteSet);
			//=====================================================================================
			// SOURCE
			//=====================================================================================
			AddCommand("SOURCE", "const none", "const null",
			delegate (CommandParam parameters) {
				source = null;
			});
			AddCommand("SOURCE", "string name",
			delegate (CommandParam parameters) {
				source = GetResource<ISpriteSource>(parameters.GetString(0));
			});
			//=====================================================================================
			// SPRITE SHEET
			//=====================================================================================
			AddCommand("SPRITESHEET",
				"string path, Point cellSize, Point spacing, Point offset",
				"string name, string path, Point cellSize, Point spacing, Point offset",
			delegate (CommandParam parameters) {
				int i = 1;
				// Create a new sprite sheet.
				Image image = null;
				string sheetName = parameters.GetString(0);
				string imagePath = sheetName;

				if (parameters.ChildCount == 5) {
					imagePath = parameters.GetString(1);
					i = 2;
				}

				if (Resources.ContainsImage(imagePath))
					image = Resources.GetResource<Image>(imagePath);
				else
					image = Resources.LoadImage(Resources.ImageDirectory + imagePath);

				if (sheetName.IndexOf('/') >= 0)
					sheetName = sheetName.Substring(sheetName.LastIndexOf('/') + 1);

				SpriteSheet sheet = new SpriteSheet(image,
						parameters.GetPoint(i + 0),
						parameters.GetPoint(i + 2),
						parameters.GetPoint(i + 1));
				AddResource<ISpriteSource>(sheetName, sheet);
			});
			//=====================================================================================
			// SPRITE SET
			//=====================================================================================
			AddCommand("SPRITESET", (int) Modes.Root,
				"string name, Point size",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				Point2I size = parameters.GetPoint(1);
				editingSpriteSet = new SpriteSet(size);
				AddResource<ISpriteSource>(spriteName, editingSpriteSet);

				Mode |= Modes.SpriteSet;
			});
			//=====================================================================================
			AddCommand("CONTINUE SPRITESET", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				editingSpriteSet = GetResource<ISpriteSource>(spriteName) as SpriteSet;
				if (editingSpriteSet == null) {
					spriteName = null;
					ThrowCommandParseError("SpriteSet with name '" + spriteName + "' does not exist in resources!");
				}

				Mode |= Modes.SpriteSet;
			});
			//=====================================================================================
			AddCommand("INSERT", (int) Modes.SpriteSet,
				"Point setIndex, Sprite sprite",
				// Int needs to go before string as int/float defaults to string.
				/*"(int insertX, int insertY), (int indexX, int indexY)",
				"(int insertX, int insertY), (string spriteName, string definition)",
				"(int insertX, int insertY), ((int indexX, int indexY), string definition)",
				"(int insertX, int insertY), (string sourceName, (int indexX, int indexY))",
				"(int insertX, int insertY), (string sourceName, (int indexX, int indexY), string definition)",*/
			delegate (CommandParam parameters) {
				Point2I setIndex = parameters.GetPoint(0);
				ISprite insertSprite = GetSpriteFromParams(parameters, 1);
				editingSpriteSet.SetSprite(setIndex, insertSprite);
			});
			//=====================================================================================
			AddCommand("APPEND", (int) Modes.SpriteSet,
				"Point insertIndex, Sprite sprite, Point drawOffset = (0, 0)",
			delegate (CommandParam parameters) {
				Point2I editPoint = parameters.GetPoint(0);
				Point2I drawOffset = parameters.GetPoint(2);
				ISprite editSprite = editingSpriteSet.GetSprite(editPoint);
				CompositeSprite composite = editSprite as CompositeSprite;
				if (composite == null) {
					composite = new CompositeSprite();
					composite.AddSprite(editSprite);
					editingSpriteSet.SetSprite(editPoint, composite);
				}
				composite.AddSprite(GetSpriteFromParams(parameters, 1), drawOffset);
			});
			//=====================================================================================
			// PALETTE
			//=====================================================================================
			AddCommand("PALETTEDICTIONARY", "const none", "const null",
			delegate (CommandParam parameters) {
				paletteArgs.Dictionary = null;
				animationBuilder.PaletteArgs = paletteArgs;
			});
			//=====================================================================================
			AddCommand("PALETTEDICTIONARY", "string name",
			delegate (CommandParam parameters) {
				paletteArgs.Dictionary = GetResource<PaletteDictionary>(parameters.GetString(0));
				animationBuilder.PaletteArgs = paletteArgs;
			});
			//=====================================================================================
			AddCommand("MAPPEDCOLORS",
				"(string colorGroup, (string subtype, Color color)...)...",
				"string palette, (string colorGroups...)",
			delegate (CommandParam parameters) {
				paletteArgs.ColorMapping.Clear();
				if (parameters.GetParam(0).Type == CommandParamType.Array) {
					int count = parameters.ChildCount;
					paletteArgs.IndexedPossibleColorGroups = new int[count];
					paletteArgs.PossibleColorGroups = new string[count];
					for (int i = 0; i < count; i++) {
						CommandParam groupParam = parameters.GetParam(i);
						string colorGroup = groupParam.GetString(0);
						if (!paletteArgs.Dictionary.Contains(colorGroup)) {
							ThrowCommandParseError("Color group '" + colorGroup + "' does not exist in palette dictionary!");
						}
						for (int j = 1; j < groupParam.ChildCount; j++) {
							CommandParam param = groupParam.GetParam(j);
							LookupSubtypes subtype = ParseSubtype(param.GetString(0));
							Color color = ParseColor(param, 1);
							/*if (paletteArgs.ColorMapping.ContainsKey(color)) {
								ThrowCommandParseError("Color already defined in MAPPEDCOLORS!");
							}*/
							Dictionary<int, ColorGroupSubtypePair> subMapping;
							if (!paletteArgs.ColorMapping.ContainsKey(color)) {
								subMapping = new Dictionary<int, ColorGroupSubtypePair>();
								paletteArgs.ColorMapping.Add(color, subMapping);
							}
							else {
								subMapping = paletteArgs.ColorMapping[color];
							}
							subMapping.Add(i, new ColorGroupSubtypePair(colorGroup, subtype, paletteArgs.Dictionary));
						}
						paletteArgs.IndexedPossibleColorGroups[i] = i;
						paletteArgs.PossibleColorGroups[i] = colorGroup;
					}
				}
				else {
					Palette palette = GetResource<Palette>(parameters.GetString(0));
					var groupsParam = parameters.GetParam(1);
					int count = groupsParam.ChildCount;
					paletteArgs.IndexedPossibleColorGroups = new int[count];
					paletteArgs.PossibleColorGroups = new string[count];
					for (int i = 0; i < count; i++) {
						string colorGroup = groupsParam.GetString(i);
						if (!paletteArgs.Dictionary.Contains(colorGroup)) {
							ThrowCommandParseError("Color group '" + colorGroup + "' does not exist in palette dictionary!");
						}
						for (int j = 0; j < PaletteDictionary.ColorGroupSize; j++) {
							LookupSubtypes subtype = (LookupSubtypes) j;
							Color color = palette.LookupColor(colorGroup, subtype);

							Dictionary<int, ColorGroupSubtypePair> subMapping;
							if (!paletteArgs.ColorMapping.ContainsKey(color)) {
								subMapping = new Dictionary<int, ColorGroupSubtypePair>();
								paletteArgs.ColorMapping.Add(color, subMapping);
							}
							else {
								subMapping = paletteArgs.ColorMapping[color];
							}
							if (!subMapping.ContainsKey(i))
								subMapping.Add(i, new ColorGroupSubtypePair(colorGroup, subtype, paletteArgs.Dictionary));
						}
						paletteArgs.IndexedPossibleColorGroups[i] = i;
						paletteArgs.PossibleColorGroups[i] = colorGroup;
					}
				}
				animationBuilder.PaletteArgs = paletteArgs;
			});
			//=====================================================================================
			AddCommand("IGNORECOLORS", "const none", "const null",
			delegate (CommandParam parameters) {
				paletteArgs.IgnoreColors.Clear();
				animationBuilder.PaletteArgs = paletteArgs;
			});
			//=====================================================================================
			AddCommand("IGNORECOLORS", "(Color colors...)",
			delegate (CommandParam parameters) {
				paletteArgs.IgnoreColors.Clear();
				var colorParams = parameters.GetParam(0);
				for (int i = 0; i < colorParams.ChildCount; i++) {
					Color color = ParseColor(colorParams, i);
					paletteArgs.IgnoreColors.Add(color);
				}
				animationBuilder.PaletteArgs = paletteArgs;
			});
			//=====================================================================================
			AddCommand("CHUNKSIZE", "const none", "const null",
			delegate (CommandParam parameters) {
				paletteArgs.ChunkSize = Point2I.Zero;
				animationBuilder.PaletteArgs = paletteArgs;
			});
			//=====================================================================================
			AddCommand("CHUNKSIZE", "Point chunkSize",
			delegate (CommandParam parameters) {
				paletteArgs.ChunkSize = parameters.GetPoint(0);
				animationBuilder.PaletteArgs = paletteArgs;
			});
			//=====================================================================================
			// BEGIN/END
			//=====================================================================================
			AddCommand("END", "",
			delegate (CommandParam parameters) {
				if (Mode != Modes.Root) {
					if ((Mode & Modes.SpriteMask) != 0) {
						if (Mode.HasFlag(Modes.SpriteSet)) {
							Mode &= ~Modes.SpriteMask;
						}
						else {
							spriteName = null;
							sprite = null;
							Mode = Modes.Root;
						}
					}
					else if (Mode == Modes.SpriteSet) {
						editingSpriteSet = null;
						Mode = Modes.Root;
					}
				}
				else {
					ThrowCommandParseError("Nothing to end!");
				}
			});
			//=====================================================================================
			// ADD NAME
			//=====================================================================================
			AddCommand("NAME", (int) Modes.Root,
				"string name, Sprite sprite",
			delegate (CommandParam parameters) {
				AddResource<ISprite>(parameters.GetString(0), GetSpriteFromParams(parameters, 1));
			});
			//=====================================================================================
			AddBasicCommands();
			AddCompositeCommands();
			AddStyleCommands();
			AddColorCommands();
			AddColorStyleCommands();
			AddColorMultiStyleCommands();
			AddStyleColorCommands();
			AddAnimationCommands();
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			sprite = null;
			spriteName = "";
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {
			sprite = null;
		}
		
		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new ISpriteSR();
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Parses the color from a string with error handling.</summary>
		private Color ParseColor(CommandParam parameters, int index) {
			Color color = parameters.GetColor(index);
			if (color.A != 255 && color.A != 0)
				ThrowCommandParseError("Color alpha must be either 0 or 255!");
			return color;
			/*if (colorParam.ChildCount <= 4) {
				int a = 255;
				if (colorParam.ChildCount == 4) {
					a = colorParam.GetInt(3);
					if (a != 255 && a != 0)
				}
				return new Color(
					colorParam.GetInt(0),
					colorParam.GetInt(1),
					colorParam.GetInt(2),
					a);
			}
			else {
				ThrowCommandParseError("Number of color channels specified is too many!");
			}
			// Unreachable code
			return Color.Black;*/
		}

		/// <summary>Parses the lookup subtype from a string with error handling.</summary>
		private LookupSubtypes ParseSubtype(string subtypeStr) {
			LookupSubtypes subtype;
			if (!Enum.TryParse(subtypeStr, true, out subtype))
				ThrowCommandParseError("Invalid subtype!");

			if (subtypeStr == "medium" && paletteArgs.Dictionary.PaletteType == PaletteTypes.Entity)
				ThrowCommandParseError("Medium color not available in entity color group!");
			else if (subtypeStr == "transparent" && paletteArgs.Dictionary.PaletteType == PaletteTypes.Tile)
				ThrowCommandParseError("Transparent color not available in tile color group!");

			return subtype;
		}

		/// <summary>Parses sprite flipping.</summary>
		private Flip ParseFlip(string flipStr) {
			if (string.Compare(flipStr, "none", true) == 0)
				return Flip.None;

			Flip flip = Flip.None;
			for (int i = 0; i < flipStr.Length; i++) {
				if (char.ToLower(flipStr[i]) == 'h') {
					if (flip.HasFlag(Flip.Horizontal))
						ThrowCommandParseError("Horizontal flip already specified!");
					flip |= Flip.Horizontal;
				}
				else if (char.ToLower(flipStr[i]) == 'v') {
					if (flip.HasFlag(Flip.Vertical))
						ThrowCommandParseError("Vertical flip already specified!");
					flip |= Flip.Vertical;
				}
				else {
					ThrowCommandParseError("Flip must specify only 'none', 'h', 'v', or both 'h' and 'v'!");
				}
			}
			return flip;
		}

		/// <summary>Parses sprite rotation.</summary>
		private Rotation ParseRotation(string rotStr) {
			if (string.Compare(rotStr, "none", true) == 0)
				return Rotation.None;

			Rotation rotation = Rotation.None;
			if (rotStr.EndsWith("ccw", StringComparison.OrdinalIgnoreCase)) {
				rotStr = rotStr.Substring(2);
				if (!Enum.TryParse("Counter" + rotStr, out rotation))
					ThrowCommandParseError("Invalid rotation specified!");
			}
			else {
				if (rotStr.EndsWith("cw"))
					rotStr = rotStr.Substring(1);
				if (!Enum.TryParse("Clockwise" + rotStr, out rotation))
					ThrowCommandParseError("Invalid rotation specified!");
			}
			return rotation;
		}

		/// <summary>Continues a sprite that already exists.</summary>
		private T ContinueSprite<T>(string name) where T : class, ISprite {
			sprite = GetResource<ISprite>(name) as T;
			if (sprite == null) {
				spriteName = null;
				ThrowCommandParseError(typeof(T).Name + " with name '" + name + "' does not exist in resources!");
			}
			return sprite as T;
		}

		/// <summary>Gets a sprite.</summary>
		private ISprite GetSprite(string name) {
			ISprite sprite = GetResource<ISprite>(name);
			if (sprite == null) {
				ThrowCommandParseError("Sprite with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite.</summary>
		private ISprite GetSprite(ISpriteSource source, Point2I index) {
			if (source == null)
				ThrowCommandParseError("Cannot get sprite from source with no sprite source!");
			if (source is SpriteSheet && paletteArgs.Dictionary != null) {
				paletteArgs.Image = SpriteSheet.Image;
				paletteArgs.SourceRect = SpriteSheet.GetSourceRect(index);
				return Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
			}
			else {
				ISprite sprite = source.GetSprite(index);
				if (sprite == null) {
					ThrowCommandParseError("Sprite at source index '" + index + "' does not exist!");
				}
				return sprite;
			}
		}

		/// <summary>Gets a sprite and confirms its type.</summary>
		private T GetSprite<T>(string name) where T : class, ISprite {
			T sprite = GetResource<ISprite>(name) as T;
			if (sprite == null) {
				ThrowCommandParseError(typeof(T).Name + " with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite and confirms its type.</summary>
		private T GetSprite<T>(ISpriteSource source, Point2I index) where T : class, ISprite {
			if (SourceMode == SourceModes.None)
				ThrowCommandParseError("Cannot get sprite from source with no sprite source!");
			T sprite = source.GetSprite(index) as T;
			if (sprite == null) {
				ThrowCommandParseError(typeof(T).Name + " at source index '" + index + "' does not exist!");
			}
			return sprite;
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(string name, string definition) {
			return GetDefinedSprite(GetSprite<DefinitionSprite>(name), definition);
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(ISpriteSource source, Point2I index, string definition) {
			return GetDefinedSprite(GetSprite<DefinitionSprite>(source, index), definition);
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(DefinitionSprite sprite, string definition) {
			ISprite defSprite = sprite.Get(definition);
			if (defSprite == null)
				ThrowCommandParseError("Defined sprite with definition '" + definition + "' does not exist!");
			return defSprite;
		}

		/// <summary>Gets the sprite from one of the many parameter overloads.</summary>
		private ISprite GetSpriteFromParams(CommandParam param, int startIndex = 0) {
			ISpriteSource source;
			Point2I index;
			string definition;
			return GetSpriteFromParams(param, startIndex, out source, out index, out definition);
		}

		/// <summary>Gets the sprite from one of the many parameter overloads and returns the source.</summary>
		private ISprite GetSpriteFromParams(CommandParam param, int startIndex, out ISpriteSource source, out Point2I index, out string definition) {
			// 1: string spriteName
			// 2: (int indexX, int indexY)
			// 3: (string animationName, int substrip)
			// 4: (string spriteName, string definition)
			// 5: ((int indexX, int indexY), string definition)
			// 6: (string sourceName, (int indexX, int indexY))
			// 7: (string sourceName, (int indexX, int indexY), string definition)
			source = null;
			index = Point2I.Zero;
			definition = null;

			var param0 = param.GetParam(startIndex);
			if (param0.IsValidType(CommandParamType.String)) {
				// Overload 1:
				return GetResource<ISprite>(param.GetString(startIndex));
			}
			else if (param0.GetParam(0).IsValidType(CommandParamType.Integer)) {
				// Overload 2:
				source = this.source;
				index = param.GetPoint(startIndex);
				return GetSprite(source, index);
			}
			else if (param0.GetParam(0).IsValidType(CommandParamType.String)) {
				if (param0.GetParam(1).IsValidType(CommandParamType.Integer)) {
					// Overload 3:
					return GetSprite<Animation>(param0.GetString(0)).GetSubstrip(param0.GetInt(1));
				}
				else if (param0.GetParam(1).IsValidType(CommandParamType.String)) {
					// Overload 4:
					return GetDefinedSprite(param0.GetString(0), param0.GetString(1));
				}
				else if (param0.ChildCount == 2) {
					// Overload 6:
					source = GetResource<ISpriteSource>(param0.GetString(0));
					index = param0.GetPoint(1);
					return GetSprite(source, index);
				}
				else {
					// Overload 7:
					source = GetResource<ISpriteSource>(param0.GetString(0));
					index = param0.GetPoint(1);
					definition = param0.GetString(2);
					return GetDefinedSprite(source, index, definition);
				}
			}
			else {
				// Overload 5:
				source = this.source;
				index = param0.GetPoint(0);
				definition = param0.GetString(1);
				return GetDefinedSprite(source, index, definition);
			}
		}

		/// <summary>Builds a basic sprite from a spritesheet and palettizes it if possible.</summary>
		private BasicSprite BuildBasicSprite(ISpriteSource source, Point2I index, Point2I drawOffset, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			if (!(source is SpriteSheet)) {
				ThrowCommandParseError("SOURCE must be specified as a sprite sheet when building a BASIC sprite!");
			}
			if (paletteArgs.Dictionary != null) {
				paletteArgs.Image = ((SpriteSheet) source).Image;
				paletteArgs.SourceRect = ((SpriteSheet) source).GetSourceRect(index);
				BasicSprite sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
				sprite.DrawOffset = drawOffset;
				sprite.FlipEffects = flip;
				sprite.Rotation = rotation;
				return sprite;
			}
			else {
				return new BasicSprite(
					(SpriteSheet) source,
					index,
					drawOffset,
					flip,
					rotation);
			}
		}

		/// <summary>Builds a basic sprite from an image and palettizes it if possible.</summary>
		private BasicSprite BuildBasicSprite(string imagePath, Point2I drawOffset, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			Image image;
			if (Resources.ContainsImage(imagePath))
				image = GetResource<Image>(imagePath);
			else
				image = Resources.LoadImage(Resources.ImageDirectory + imagePath);

			SpriteSheet tempSheet = new SpriteSheet(image);
			return BuildBasicSprite(tempSheet, Point2I.Zero, drawOffset, flip, rotation);
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>The mode of the ISprite script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}

		/// <summary>The type of the current source sprite sheet.</summary>
		private SourceModes SourceMode {
			get {
				if (source is SpriteSheet)
					return SourceModes.SpriteSheet;
				else if (source is SpriteSet)
					return SourceModes.SpriteSet;
				else
					return SourceModes.None;
			}
		}

		/// <summary>Gets the source as a sprite sheet.</summary>
		private SpriteSheet SpriteSheet {
			get { return source as SpriteSheet; }
		}

		/// <summary>Gets the source as a sprite set.</summary>
		private SpriteSet SpriteSet {
			get { return source as SpriteSet; }
		}

		/// <summary>Gets the current sprite as an empty sprite.</summary>
		private EmptySprite EmptySprite {
			get { return sprite as EmptySprite; }
		}

		/// <summary>Gets the current sprite as a basic sprite.</summary>
		private BasicSprite BasicSprite {
			get { return sprite as BasicSprite; }
		}

		/// <summary>Gets the current sprite as an offset sprite.</summary>
		private OffsetSprite OffsetSprite {
			get { return sprite as OffsetSprite; }
		}

		/// <summary>Gets the current sprite as a composite sprite.</summary>
		private CompositeSprite CompositeSprite {
			get { return sprite as CompositeSprite; }
		}

		/// <summary>Gets the current sprite as a color sprite.</summary>
		private ColorSprite ColorSprite {
			get { return sprite as ColorSprite; }
		}

		/// <summary>Gets the current sprite as a style sprite.</summary>
		private StyleSprite StyleSprite {
			get { return sprite as StyleSprite; }
		}

		/// <summary>Gets the current sprite as an animation.</summary>
		private Animation Animation {
			get { return sprite as Animation; }
		}
		
	}
}

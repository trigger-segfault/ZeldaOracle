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

		private enum SourceModes {
			None,
			SpriteSheet,
			SpriteSet
		}

		[Flags]
		private enum Modes {
			Root			= 0,
			SpriteSet		= 1 << 0,
			Dynamic			= 1 << 1,
			EmptySprite		= 1 << 2,
			BasicSprite		= 1 << 3,
			OffsetSprite	= 1 << 4,
			StyleSprite		= 1 << 5,
			CompositeSprite	= 1 << 6,
			Animation		= 1 << 7,
			SpriteMask		= EmptySprite | BasicSprite | OffsetSprite |
							StyleSprite | CompositeSprite | Animation
		}

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------
		
		private string spriteName;
		private AnimationBuilder animationBuilder;
		
		private SpritePaletteArgs paletteArgs;
		
		private SpriteSet spriteSet;
		
		private ISpriteSheet source;
		private ISprite sprite;

		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public ISpriteSR() {
			
			this.source         = null;
			this.sprite         = null;
			this.paletteArgs.ColorMapping = new Dictionary<Color, ColorGroupSubtypePair>();
			this.paletteArgs.IgnoreColors = new HashSet<Color>();
			this.animationBuilder = new AnimationBuilder();
			this.animationBuilder.PaletteArgs = paletteArgs;

			//=====================================================================================
			// Prefixes
			//=====================================================================================
			// Continues the sprite with the existing name.
			AddCommandPrefix("CONTINUE", (int) Modes.Root);
			// Used to initialize a grid in a spriteset when starting a sprite.
			AddCommandPrefix("DYNAMIC", (int) Modes.SpriteSet);
			//=====================================================================================
			// SOURCE
			//=====================================================================================
			AddCommand("SOURCE", "string name",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix()) {
					ThrowCommandParseError("Invalid use of prefix");
				}
				string name = parameters.GetString(0);
				if (name.ToLower() == "none") {
					source = null;
					return;
				}
				if (!Resources.ContainsResource<ISpriteSheet>(name)) {
					ThrowCommandParseError("No sprite sheet with the name '" + name + "' exists in resources!");
				}
				source = Resources.GetResource<ISpriteSheet>(name);
				animationBuilder.Source = source;
			});
			//=====================================================================================
			// SPRITE SHEET
			//=====================================================================================
			AddCommand("SPRITESHEET",
				"string path, (int cellWidth, int cellHeight), (int spacingX, int spacingY), (int offsetX, int offsetY)",
				"string name, string path, (int cellWidth, int cellHeight), (int spacingX, int spacingY), (int offsetX, int offsetY)",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix()) {
					ThrowCommandParseError("Invalid use of prefix");
				}
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
				AddResource<ISpriteSheet>(sheetName, sheet);
				source = sheet;
				animationBuilder.Source = source;
			});
			//=====================================================================================
			// SPRITE SET
			//=====================================================================================
			AddCommand("SPRITESET", (int) Modes.Root,
				"string name",
				"string name, (int width, int height)",
			delegate (CommandParam parameters) {
				bool continueSprite = parameters.HasPrefix("continue");
				if (!continueSprite && parameters.HasPrefix())
					ThrowCommandParseError("Invalid use of prefix");
				spriteName = parameters.GetString(0);
				if (continueSprite) {
					if (parameters.ChildCount == 2)
						ThrowCommandParseError("Invalid use of prefix");
					spriteSet = GetResource<ISpriteSheet>(spriteName) as SpriteSet;
					if (spriteSet == null) {
						spriteName = null;
						ThrowCommandParseError("SpriteSet with name '" + spriteName + "' does not exist in resources!");
					}
				}
				else {
					if (parameters.ChildCount == 1)
						ThrowCommandParseError("This command requires the CONTINUE prefix!");
					spriteSet = new SpriteSet(parameters.GetPoint(1));
					AddResource<ISpriteSheet>(spriteName, spriteSet);
				}
				Mode |= Modes.SpriteSet;
			});
			//=====================================================================================
			AddCommand("INSERT", (int) Modes.SpriteSet,
				"(int insertX, int insertY), string name",
				"(int insertX, int insertY), (int indexX, int indexY)",
				"(int insertX, int insertY), string sourceName, (int indexX, int indexY)",
			delegate (CommandParam parameters) {
				bool continueSprite = parameters.HasPrefix("continue");
				if (parameters.HasPrefix()) {
					ThrowCommandParseError("Invalid use of prefix");
				}
				if (parameters.ChildCount == 1 && parameters.GetParam(1).Type == CommandParamType.String) {
					ISprite spriteResource = GetResource<ISprite>(parameters.GetString(1));
					spriteSet.SetSprite(parameters.GetPoint(0), spriteResource);
				}
				else {
					ISpriteSheet newSource = source;

					Point2I index;
					if (parameters.GetParam(1).Type == CommandParamType.String) {
						newSource = GetResource<ISpriteSheet>(parameters.GetString(1));
						index = parameters.GetPoint(2);
					}
					else {
						index = parameters.GetPoint(1);
					}

					spriteSet.SetSprite(parameters.GetPoint(0), newSource.GetSprite(index));
				}
			});
			//=====================================================================================
			// PALETTE
			//=====================================================================================
			AddCommand("PALETTEDICTIONARY", "", "string name",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix()) {
					ThrowCommandParseError("Invalid use of prefix");
				}
				string name = parameters.GetString(0);
				if (name.ToLower() == "none") {
					source = null;
					return;
				}
				if (!Resources.ContainsResource<PaletteDictionary>(name)) {
					ThrowCommandParseError("Specified PALETTEDICTIONARY does not exist in resources!");
				}
				paletteArgs.Dictionary = Resources.GetResource<PaletteDictionary>(name);
				animationBuilder.PaletteArgs = paletteArgs;
			});
			//=====================================================================================
			AddCommand("MAPPEDCOLORS",
				"(string colorGroup, (string subtype, (int r, int g, int b...))...)...",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix()) {
					ThrowCommandParseError("Invalid use of prefix");
				}
				paletteArgs.ColorMapping.Clear();
				for (int i = 0; i < parameters.ChildCount; i++) {
					CommandParam groupParam = parameters.GetParam(i);
					if (!paletteArgs.Dictionary.Contains(groupParam.GetString(0))) {
						ThrowCommandParseError("Specified color group does not exist in palette dictionary!");
					}
					string colorGroup = groupParam.GetString(0);
					for (int j = 1; j < groupParam.ChildCount; j++) {
						CommandParam param = groupParam.GetParam(j);
						LookupSubtypes subtype = ParseSubtype(param.GetString(0));
						Color color = ParseColor(param.GetParam(1));
						if (paletteArgs.ColorMapping.ContainsKey(color)) {
							ThrowCommandParseError("Color already defined in MAPPEDCOLORS!");
						}
						paletteArgs.ColorMapping.Add(color, new ColorGroupSubtypePair(colorGroup, subtype, paletteArgs.Dictionary));
					}
				}
				animationBuilder.PaletteArgs = paletteArgs;
			});
			//=====================================================================================
			AddCommand("IGNORECOLORS",
				"string none",
				"(int r, int g, int b...)...",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix()) {
					ThrowCommandParseError("Invalid use of prefix");
				}
				paletteArgs.IgnoreColors.Clear();
				if (parameters.GetParam(0).Name == "none") {
					if (string.Compare(parameters.GetString(0), "none", true) != 0)
						ThrowCommandParseError("Must specify 'none' or a list of colors!");
				}
				else {
					for (int i = 0; i < parameters.ChildCount; i++) {
						Color color = ParseColor(parameters.GetParam(i));
						paletteArgs.IgnoreColors.Add(color);
					}
				}
				animationBuilder.PaletteArgs = paletteArgs;
			});
			//=====================================================================================
			// BEGIN/END
			//=====================================================================================
			AddCommand("END", "",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix()) {
					ThrowCommandParseError("Invalid use of prefix");
				}
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
						spriteSet = null;
						Mode = Modes.Root;
					}
				}
			});
			//=====================================================================================
			// ADD NAME
			//=====================================================================================
			AddCommand("NAME", (int) Modes.Root,
				"string name, (int indexX, int indexY)",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix()) {
					ThrowCommandParseError("Invalid use of prefix");
				}
				if (SourceMode == SourceModes.None) {
					ThrowCommandParseError("Cannot name sprite with no source sprite sheet set!");
				}
				AddResource<ISprite>(parameters.GetString(0), source.GetSprite(parameters.GetPoint(1)));
			});
			//=====================================================================================
			AddBasicCommands();
			AddCompositeCommands();
			AddStyleCommands();
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
			animationBuilder.Source = null;
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
		private Color ParseColor(CommandParam colorParam) {
			if (colorParam.ChildCount <= 4) {
				int a = 255;
				if (colorParam.ChildCount == 4) {
					a = colorParam.GetInt(3);
					if (a != 255 && a != 0)
						ThrowCommandParseError("Color alpha must be either 0 or 255!");
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
			return Color.Black;
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

		/// <summary>Continues a sprite that already exists.</summary>
		private T ContinueSprite<T>(string name) where T : class, ISprite {
			sprite = GetResource<ISprite>(name) as T;
			if (sprite == null) {
				spriteName = null;
				ThrowCommandParseError(typeof(T).Name + " with name '" + name + "' does not exist in resources!");
			}
			return sprite as T;
		}

		/// <summary>Gets a sprite and confirms its type.</summary>
		private T GetSprite<T>(string name) where T : class, ISprite {
			ISprite sprite = GetResource<ISprite>(name) as T;
			if (sprite == null) {
				spriteName = null;
				ThrowCommandParseError(typeof(T).Name + " with name '" + name + "' does not exist in resources!");
			}
			return sprite as T;
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

		/// <summary>Gets the sprite set being edited.</summary>
		private SpriteSet EditingSpriteSet {
			get { return spriteSet; }
		}

		/// <summary>Gets the current sprite as an empty sprite.</summary>
		private EmptySprite EmptySprite {
			get { return sprite as EmptySprite; }
		}

		/// <summary>Gets the current sprite as a basic sprite.</summary>
		private BasicSprite BasicSprite {
			get { return sprite as BasicSprite; }
		}

		/// <summary>Gets the current sprite as a composite sprite.</summary>
		private CompositeSprite CompositeSprite {
			get { return sprite as CompositeSprite; }
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
} // end namespace

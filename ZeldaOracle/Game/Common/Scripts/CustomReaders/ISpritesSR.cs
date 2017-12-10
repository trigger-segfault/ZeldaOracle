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

	public partial class ISpritesSR : ScriptReader {

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

		public ISpritesSR() {
			
			this.source         = null;
			this.sprite         = null;
			this.animationBuilder = new AnimationBuilder();

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
			AddBasicCommands();
			AddCompositeCommands();
			AddStyleCommands();
			AddAnimationCommands();
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		// Begins reading the script.
		protected override void BeginReading() {
			sprite = null;
			spriteName = "";
			animationBuilder.Source = null;
		}

		// Ends reading the script.
		protected override void EndReading() {
			sprite = null;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

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

		private T ContinueSprite<T>(string name) where T : class, ISprite {
			sprite = GetResource<ISprite>(name) as T;
			if (sprite == null) {
				spriteName = null;
				ThrowCommandParseError(typeof(T).Name + " with name '" + name + "' does not exist in resources!");
			}
			return sprite as T;
		}

		private T GetSprite<T>(string name) where T : class, ISprite {
			sprite = GetResource<ISprite>(name) as T;
			if (sprite == null) {
				spriteName = null;
				ThrowCommandParseError(typeof(T).Name + " with name '" + name + "' does not exist in resources!");
			}
			return sprite as T;
		}

		private ISprite GetSprite<T1, T2>(string name)
			where T1 : class, ISprite where T2 : class, ISprite
		{
			sprite = GetResource<ISprite>(name);
			if (!(sprite is T1) && !(sprite is T2)) {
				spriteName = null;
				ThrowCommandParseError(typeof(T1).Name + " or " + typeof(T2) + " with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}

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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SpriteSheet SpriteSheet {
			get { return source as SpriteSheet; }
		}

		public SpriteSet SpriteSet {
			get { return source as SpriteSet; }
		}

		public EmptySprite EmptySprite {
			get { return sprite as EmptySprite; }
		}

		public BasicSprite BasicSprite {
			get { return sprite as BasicSprite; }
		}

		public CompositeSprite CompositeSprite {
			get { return sprite as CompositeSprite; }
		}

		public StyleSprite StyleSprite {
			get { return sprite as StyleSprite; }
		}

		public Animation Animation {
			get { return sprite as Animation; }
		}
	}
} // end namespace

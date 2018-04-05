using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Conscripts.CustomReaders {
	public partial class SpriteSR : ConscriptRunner {

		/// <summary>Adds ColorSprite+(StyleSprite/ColorGroup) commands to the script reader.</summary>
		public void AddColorMultiStyleCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("COLORMULTISTYLE", (int) Modes.Root,
				"string name, ((string styleGroup, string colorGroup)...)",
			delegate (CommandParam parameters) {
				editingColorStyleGroups.Clear();

				CommandParam colorStyleParams = parameters.GetParam(1);
				for (int i = 0; i < colorStyleParams.ChildCount; i++) {
					CommandParam colorStyle = colorStyleParams.GetParam(i);
					string styleGroup = colorStyle.GetString(0);
					string colorGroup = colorStyle.GetString(1);
					editingColorStyleGroups.Add(new ColorStyleGroup(styleGroup, colorGroup));
					Resources.RegisterStyleGroup(styleGroup, null);
				}

				spriteName = parameters.GetString(0);
				sprite = new ColorSprite("");
				AddSprite(spriteName, sprite);

				Mode |= Modes.ColorSprite | Modes.StyleSprite | Modes.MultiStyle;
			});
			//=====================================================================================
			AddCommand("CONTINUE COLORMULTISTYLE", (int) Modes.Root,
				"string name, ((string styleGroup, string colorGroup)...)",
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				ContinueSprite<ColorSprite>(spriteName);
				
				editingColorStyleGroups.Clear();

				foreach (DefinedSprite definition in ColorSprite.GetDefinitions()) {
					editingColorStyleGroups.Add(new ColorStyleGroup(
						((StyleSprite) definition.Sprite).Group, definition.Definition));
				}

				// Add the new color mappings and style groups to all existing styles
				if (parameters.ChildCount == 2) {
					Color[][] defaultMappings = null;
					CommandParam colorStyleParams = parameters.GetParam(1);
					for (int i = 0; i < colorStyleParams.ChildCount; i++) {
						CommandParam colorStyle = colorStyleParams.GetParam(i);
						editingColorStyleGroups.Add(new ColorStyleGroup(
							colorStyle.GetString(0), colorStyle.GetString(1)));
						Resources.RegisterStyleGroup(colorStyle.GetString(0), null);
					}
					foreach (DefinedSprite definition in ColorSprite.DefaultStyleSprite.GetDefinitions()) {
						defaultMappings = ColorizeColorMultiStyleSprite(
							definition.Definition, ColorSprite, Point2I.Zero, defaultMappings);
					}
				}

				Mode |= Modes.ColorSprite | Modes.StyleSprite | Modes.MultiStyle;
			});
			//=====================================================================================
			// SETUP SpriteSet
			//=====================================================================================
			AddCommand("MULTIPLE COLORMULTISTYLE", (int) Modes.SpriteSet,
				"((string styleGroup, string colorGroup)...), Point start = (0, 0), Point span = (0, 0)",
			delegate (CommandParam parameters) {
				editingColorStyleGroups.Clear();

				CommandParam colorStyleParams = parameters.GetParam(0);
				for (int i = 0; i < colorStyleParams.ChildCount; i++) {
					CommandParam colorStyle = colorStyleParams.GetParam(i);
					editingColorStyleGroups.Add(new ColorStyleGroup(
						colorStyle.GetString(0), colorStyle.GetString(1)));
					Resources.RegisterStyleGroup(colorStyle.GetString(0), null);
				}

				editingSetStart = parameters.GetPoint(1);
				editingSetDimensions = parameters.GetPoint(2);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						editingSpriteSet.SetSprite(editingSetStart + new Point2I(x, y), new ColorSprite(""));
					}
				}
				
				Mode |= Modes.ColorSprite | Modes.StyleSprite | Modes.MultiStyle;
			});
			//=====================================================================================
			AddCommand("CONTINUE MULTIPLE COLORMULTISTYLE", (int) Modes.SpriteSet,
				"((string styleGroup, string colorGroup)...), Point start = (0, 0), Point span = (0, 0)",
				"Point start = (0, 0), Point dimensions = (0, 0)",
			delegate (CommandParam parameters) {
				editingSetStart = parameters.GetPoint(parameters.ChildCount - 2);
				editingSetDimensions = parameters.GetPoint(parameters.ChildCount - 1);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				ColorSprite firstColorSprite = GetSprite<ColorSprite>(editingSpriteSet, editingSetStart);
				editingColorStyleGroups.Clear();

				foreach (DefinedSprite definition in firstColorSprite.GetDefinitions()) {
					editingColorStyleGroups.Add(new ColorStyleGroup(
						((StyleSprite) definition.Sprite).Group, definition.Definition));
				}

				// Add the new color mappings and style groups to all existing styles
				if (parameters.ChildCount == 3) {
					Color[][] defaultMappings = null;
					CommandParam colorStyleParams = parameters.GetParam(0);
					for (int i = 0; i < colorStyleParams.ChildCount; i++) {
						CommandParam colorStyle = colorStyleParams.GetParam(i);
						editingColorStyleGroups.Add(new ColorStyleGroup(
							colorStyle.GetString(0), colorStyle.GetString(1)));
						Resources.RegisterStyleGroup(colorStyle.GetString(0), null);
					}
					for (int x = 0; x < editingSetDimensions.X; x++) {
						for (int y = 0; y < editingSetDimensions.Y; y++) {
							foreach (DefinedSprite definition in firstColorSprite.DefaultStyleSprite.GetDefinitions()) {
								Point2I point = new Point2I(x, y);
								ColorSprite colorSprite = GetSprite<ColorSprite>(
									editingSpriteSet, editingSetStart + point);

								defaultMappings = ColorizeColorMultiStyleSprite(
									definition.Definition, colorSprite, Point2I.Zero, defaultMappings);
							}
						}
					}
				}

				// Confirm all sprites are colorSprites
				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						GetSprite<ColorSprite>(editingSpriteSet, editingSetStart + new Point2I(x, y));
					}
				}
				
				Mode |= Modes.ColorSprite | Modes.StyleSprite | Modes.MultiStyle;
			});
			//=====================================================================================
			AddCommand("SINGLE COLORMULTISTYLE", (int) Modes.SpriteSet,
				"((string styleGroup, string colorGroup)...), Point setIndex",
			delegate (CommandParam parameters) {
				editingSetStart = parameters.GetPoint(3);
				editingSetDimensions = Point2I.One;

				sprite = new ColorSprite("");
				editingSpriteSet.SetSprite(editingSetStart, ColorSprite);

				editingColorStyleGroups.Clear();

				CommandParam colorStyleParams = parameters.GetParam(0);
				for (int i = 0; i < colorStyleParams.ChildCount; i++) {
					CommandParam colorStyle = colorStyleParams.GetParam(i);
					editingColorStyleGroups.Add(new ColorStyleGroup(
						colorStyle.GetString(0), colorStyle.GetString(1)));
					Resources.RegisterStyleGroup(colorStyle.GetString(0), null);
				}

				Mode |= Modes.ColorSprite | Modes.StyleSprite | Modes.MultiStyle;
			});
			//=====================================================================================
			AddCommand("CONTINUE SINGLE COLORMULTISTYLE", (int) Modes.SpriteSet,
				"((string styleGroup, string colorGroup)...), Point setIndex",
				"Point setIndex",
			delegate (CommandParam parameters) {
				sprite = GetSprite<ColorSprite>(editingSpriteSet, editingSetStart);
				
				editingSetStart = parameters.GetPoint(parameters.ChildCount - 1);
				editingSetDimensions = Point2I.One;
				
				editingColorStyleGroups.Clear();

				foreach (DefinedSprite definition in ColorSprite.GetDefinitions()) {
					editingColorStyleGroups.Add(new ColorStyleGroup(
						((StyleSprite) definition.Sprite).Group, definition.Definition));
				}

				// Add the new color mappings and style groups to all existing styles
				if (parameters.ChildCount == 2) {
					Color[][] defaultMappings = null;
					CommandParam colorStyleParams = parameters.GetParam(0);
					for (int i = 0; i < colorStyleParams.ChildCount; i++) {
						CommandParam colorStyle = colorStyleParams.GetParam(i);
						editingColorStyleGroups.Add(new ColorStyleGroup(
							colorStyle.GetString(0), colorStyle.GetString(1)));
						Resources.RegisterStyleGroup(colorStyle.GetString(0), null);
					}
					foreach (DefinedSprite definition in ColorSprite.DefaultStyleSprite.GetDefinitions()) {
						defaultMappings = ColorizeColorMultiStyleSprite(
							definition.Definition, ColorSprite, Point2I.Zero, defaultMappings);
					}
				}
				
				Mode |= Modes.ColorSprite | Modes.StyleSprite | Modes.MultiStyle;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("ADD", (int) (Modes.ColorSprite | Modes.StyleSprite | Modes.MultiStyle),
				"string style, Point sourceIndex",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				Point2I sourceIndex = parameters.GetPoint(1);

				ColorizeColorMultiStyleSprite(style, ColorSprite, sourceIndex);
			});
			//=====================================================================================
			// BUILDING SpriteSet
			//=====================================================================================
			AddCommand("ADD", (int) (Modes.SpriteSet | Modes.ColorSprite |  Modes.StyleSprite | Modes.MultiStyle),
				"string style, Point sourceIndex",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				Point2I sourceIndex = parameters.GetPoint(1);

				Color[][] defaultMappings = null;
				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						Point2I point = new Point2I(x, y);
						ColorSprite colorSprite = GetSprite<ColorSprite>(
							editingSpriteSet, editingSetStart + point);
						defaultMappings = ColorizeColorMultiStyleSprite(
							style, colorSprite, sourceIndex + point, defaultMappings);
					}
				}
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Colorizes a color sprite with the specified color parameters.</summary>
		private Color[][] ColorizeColorMultiStyleSprite(string style, ColorSprite sprite, Point2I sourceIndex, Color[][] defaultMappings = null) {
			if (paletteArgs.Dictionary == null)
				ThrowCommandParseError("Cannot create color sprite with no palette dictionary specified!");
			if (SourceMode != SourceModes.SpriteSheet && (sprite.DefaultSprite == null  ||
				sprite.DefaultStyleSprite.DefaultSprite == null)) {
				ThrowCommandParseError("Cannot create color sprite with a source that is not a sprite sheet!");
			}

			if (SourceMode == SourceModes.SpriteSheet) {
				paletteArgs.Image = SpriteSheet.Image;
				paletteArgs.SourceRect = SpriteSheet.GetSourceRect(sourceIndex);
			}
			if (defaultMappings == null) {
				defaultMappings = new Color[editingColorStyleGroups.Count][];
				for (int i = 0; i < editingColorStyleGroups.Count; i++) {
					string colorGroup = editingColorStyleGroups[i].ColorGroup;
					defaultMappings[i] = new Color[Palette.ColorGroupSize];
					for (int j = 0; j < Palette.ColorGroupSize; j++) {
						defaultMappings[i][j] = paletteArgs.Dictionary.GetMappedColor(
							colorGroup, (LookupSubtypes) j);
					}
				}
			}


			for (int i = 0; i < editingColorStyleGroups.Count; i++) {
				string colorGroup = editingColorStyleGroups[i].ColorGroup;
				string styleGroup = editingColorStyleGroups[i].StyleGroup;
				StyleSprite styleSprite;
				if (!sprite.Contains(colorGroup)) {
					styleSprite = new StyleSprite(styleGroup);
					sprite.Add(colorGroup, styleSprite);
					Resources.RegisterStylePreview(styleGroup, styleSprite);
				}
				else {
					styleSprite = (StyleSprite) sprite.Get(colorGroup);
				}
				if (!styleSprite.Contains(style)) {
					paletteArgs.DefaultMapping = defaultMappings[i];
					ISprite coloredStyledSprite;
					if (!sprite.DefaultStyleSprite.Contains(style)) {
						coloredStyledSprite = Resources.SpriteDatabase.AddSprite(paletteArgs);
					}
					else {
						coloredStyledSprite = Resources.SpriteDatabase.RepaletteSprite(
							(BasicSprite) sprite.DefaultStyleSprite.Get(style), paletteArgs);
					}

					Resources.RegisterStyle(styleGroup, style);
					styleSprite.Add(style, coloredStyledSprite);
				}
			}
			paletteArgs.DefaultMapping = null;
			return defaultMappings;
		}
	}
}

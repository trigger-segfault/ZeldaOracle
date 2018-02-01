using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripts.Commands;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	public partial class ISpriteSR : ScriptReader {

		/// <summary>Adds ColorSprite commands to the script reader.</summary>
		public void AddStyleColorCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("STYLECOLOR", (int) Modes.Root,
				"string name, string styleGroup, (string colorGroups...), Point sourceIndex",
			delegate (CommandParam parameters) {
				string styleGroup = parameters.GetString(1);
				CommandParam colorParams = parameters.GetParam(2);
				Point2I sourceIndex = parameters.GetPoint(3);

				spriteName = parameters.GetString(0);
				sprite = new StyleSprite(styleGroup);
				AddResource<ISprite>(spriteName, sprite);
				Resources.RegisterStyleGroup(styleGroup, StyleSprite);

				ColorizeStyleColorSprite(StyleSprite, colorParams, sourceIndex);

				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE STYLECOLOR", (int) Modes.Root,
				"string name, (string colorGroups...)",
			delegate (CommandParam parameters) {
				CommandParam colorParams = parameters.GetParam(2);

				spriteName = parameters.GetString(0);
				ContinueSprite<StyleSprite>(spriteName);

				ColorizeStyleColorSprite(StyleSprite, colorParams, Point2I.Zero);

				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
			// SETUP SpriteSet
			//=====================================================================================
			AddCommand("MULTIPLE STYLECOLOR", (int) Modes.SpriteSet,
				"string styleGroup, (string colorGroups...), Point sourceIndex, Point start = (0, 0), Point dimensions = (0, 0)",
			delegate (CommandParam parameters) {
				string styleGroup = parameters.GetString(0);
				var colorParams = parameters.GetParam(1);
				Point2I sourceIndex = parameters.GetPoint(2);
				editingSetStart = parameters.GetPoint(3);
				editingSetDimensions = parameters.GetPoint(4);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				Color[][] defaultMappings = null;
				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						Point2I point = new Point2I(x, y);
						StyleSprite styleSprite = new StyleSprite(styleGroup);
						if (point.IsZero)
							Resources.RegisterStyleGroup(styleGroup, styleSprite);
						defaultMappings = ColorizeStyleColorSprite(styleSprite, colorParams, sourceIndex + point, defaultMappings);
						editingSpriteSet.SetSprite(editingSetStart + point, styleSprite);
					}
				}

				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE MULTIPLE STYLECOLOR", (int) Modes.SpriteSet,
				"(string colorGroups...), Point start = (0, 0), Point dimensions = (0, 0)",
			delegate (CommandParam parameters) {
				var colorParams = parameters.GetParam(0);
				editingSetStart = parameters.GetPoint(1);
				editingSetDimensions = parameters.GetPoint(2);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				Color[][] defaultMappings = null;
				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						StyleSprite styleSprite = GetSprite<StyleSprite>(editingSpriteSet, editingSetStart + new Point2I(x, y));
						defaultMappings = ColorizeStyleColorSprite(styleSprite, colorParams, Point2I.Zero, defaultMappings);
					}
				}

				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
			AddCommand("SINGLE STYLECOLOR", (int) Modes.SpriteSet,
				"string styleGroup, (string colorGroups...), Point sourceIndex",
				"string styleGroup, (string colorGroups...), Point sourceIndex, Point setIndex",
			delegate (CommandParam parameters) {
				string styleGroup = parameters.GetString(0);
				var colorParams = parameters.GetParam(1);
				Point2I sourceIndex = parameters.GetPoint(2);
				editingSetStart = parameters.GetPoint(3, sourceIndex);
				editingSetDimensions = Point2I.One;

				sprite = new ColorSprite(styleGroup);
				ColorizeStyleColorSprite(StyleSprite, colorParams, sourceIndex);
				editingSpriteSet.SetSprite(editingSetStart, ColorSprite);
				Resources.RegisterStyleGroup(styleGroup, StyleSprite);

				Mode |= Modes.StyleColorSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE SINGLE STYLECOLOR", (int) Modes.SpriteSet,
				"string colorationGroup, (string colorGroups...), Point setIndex",
			delegate (CommandParam parameters) {
				var colorParams = parameters.GetParam(0);
				editingSetStart = parameters.GetPoint(1);
				editingSetDimensions = Point2I.One;

				sprite = GetSprite<StyleSprite>(editingSpriteSet, editingSetStart);
				ColorizeStyleColorSprite(StyleSprite, colorParams, Point2I.Zero);

				Mode |= Modes.StyleColorSprite;
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Colorizes a colored style sprite with the specified color parameters.</summary>
		private Color[][] ColorizeStyleColorSprite(StyleSprite sprite, CommandParam colorParams, Point2I sourceIndex, Color[][] defaultMappings = null) {
			if (paletteArgs.Dictionary == null)
				ThrowCommandParseError("Cannot create style color sprite with no palette dictionary specified!");
			if (SourceMode != SourceModes.SpriteSheet && sprite.DefaultSprite == null)
				ThrowCommandParseError("Cannot create style color sprite with a source that is not a sprite sheet!");

			if (sprite.DefaultSprite == null) {
				paletteArgs.Image = SpriteSheet.Image;
				paletteArgs.SourceRect = SpriteSheet.GetSourceRect(sourceIndex);
			}
			if (defaultMappings == null) {
				int colorCount = colorParams.ChildCount;
				defaultMappings = new Color[colorCount][];
				for (int i = 0; i < colorCount; i++) {
					string colorGroup = colorParams.GetString(i);
					defaultMappings[i] = new Color[PaletteDictionary.ColorGroupSize];
					for (int j = 0; j < PaletteDictionary.ColorGroupSize; j++) {
						defaultMappings[i][j] = paletteArgs.Dictionary.GetMappedColor(
							colorGroup, (LookupSubtypes) j);
					}
				}
			}

			for (int i = 0; i < colorParams.ChildCount; i++) {
				Resources.RegisterStyle(sprite.Group, colorParams.GetString(i));
				paletteArgs.DefaultMapping = defaultMappings[i];
				ISprite coloredSprite;
				if (sprite.DefaultSprite == null) {
					coloredSprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
				}
				else {
					coloredSprite = Resources.PalettedSpriteDatabase.RepaletteSprite(
						(BasicSprite) sprite.DefaultSprite, paletteArgs);
				}

				sprite.Add(colorParams.GetString(i), coloredSprite);
			}
			paletteArgs.DefaultMapping = null;
			return defaultMappings;
		}
	}
}

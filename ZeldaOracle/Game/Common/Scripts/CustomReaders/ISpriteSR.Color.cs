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
		public void AddColorCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("COLOR", (int) Modes.Root,
				"string name, string colorationGroup, (string colorGroups...), Point sourceIndex",
			delegate (CommandParam parameters) {
				string colorationGroup = parameters.GetString(1);
				CommandParam colorParams = parameters.GetParam(2);
				Point2I sourceIndex = parameters.GetPoint(3);

				spriteName = parameters.GetString(0);
				sprite = new ColorSprite(colorationGroup);
				AddResource<ISprite>(spriteName, sprite);
				
				ColorizeColorSprite(ColorSprite, colorParams, sourceIndex);

				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE COLOR", (int) Modes.Root,
				"string name, (string colorGroups...)",
			delegate (CommandParam parameters) {
				CommandParam colorParams = parameters.GetParam(2);

				spriteName = parameters.GetString(0);
				ContinueSprite<ColorSprite>(spriteName);

				ColorizeColorSprite(ColorSprite, colorParams, Point2I.Zero);
				
				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
			// SETUP SpriteSet
			//=====================================================================================
			AddCommand("MULTIPLE COLOR", (int) Modes.SpriteSet,
				"string colorationGroup, (string colorGroups...), Point sourceIndex, Point start = (0, 0), Point dimensions = (0, 0)",
			delegate (CommandParam parameters) {
				string colorationGroup = parameters.GetString(0);
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
						ColorSprite colorSprite = new ColorSprite(colorationGroup);
						defaultMappings = ColorizeColorSprite(colorSprite, colorParams, sourceIndex + point, defaultMappings);
						editingSpriteSet.SetSprite(editingSetStart + point, colorSprite);
					}
				}

				singular = false;
				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE MULTIPLE COLOR", (int) Modes.SpriteSet,
				"string colorationGroup, (string colorGroups...), Point start = (0, 0), Point dimensions = (0, 0)",
			delegate (CommandParam parameters) {
				string colorationGroup = parameters.GetString(0);
				var colorParams = parameters.GetParam(1);
				editingSetStart = parameters.GetPoint(3);
				editingSetDimensions = parameters.GetPoint(4);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				Color[][] defaultMappings = null;
				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						ColorSprite colorSprite = GetSprite<ColorSprite>(editingSpriteSet, editingSetStart + new Point2I(x, y));
						defaultMappings = ColorizeColorSprite(colorSprite, colorParams, Point2I.Zero, defaultMappings);
					}
				}

				singular = false;
				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
			AddCommand("SINGLE COLOR", (int) Modes.SpriteSet,
				"string colorationGroup, (string colorGroups...), Point sourceIndex",
				"string colorationGroup, (string colorGroups...), Point sourceIndex, Point setIndex",
			delegate (CommandParam parameters) {
				string colorationGroup = parameters.GetString(0);
				var colorParams = parameters.GetParam(1);
				Point2I sourceIndex = parameters.GetPoint(2);
				editingSetStart = parameters.GetPoint(3, sourceIndex);
				editingSetDimensions = Point2I.One;

				sprite = new ColorSprite(colorationGroup);
				ColorizeColorSprite(ColorSprite, colorParams, sourceIndex);
				editingSpriteSet.SetSprite(editingSetStart, ColorSprite);

				singular = true;
				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE SINGLE COLOR", (int) Modes.SpriteSet,
				"string colorationGroup, (string colorGroups...), Point setIndex",
			delegate (CommandParam parameters) {
				string colorationGroup = parameters.GetString(0);
				var colorParams = parameters.GetParam(1);
				editingSetStart = parameters.GetPoint(2);
				editingSetDimensions = Point2I.One;

				sprite = GetSprite<ColorSprite>(editingSpriteSet, editingSetStart);
				ColorizeColorSprite(ColorSprite, colorParams, Point2I.Zero);

				singular = true;
				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Colorizes a color sprite with the specified color parameters.</summary>
		private Color[][] ColorizeColorSprite(ColorSprite sprite, CommandParam colorParams, Point2I sourceIndex, Color[][] defaultMappings = null) {
			if (paletteArgs.Dictionary == null)
				ThrowCommandParseError("Cannot create color sprite with no palette dictionary specified!");
			if (SourceMode != SourceModes.SpriteSheet && sprite.DefaultSprite == null)
				ThrowCommandParseError("Cannot create color sprite with a source that is not a sprite sheet!");

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
				paletteArgs.DefaultMapping = defaultMappings[i];
				ISprite coloredSprite;
				if (sprite.DefaultSprite == null) {
					coloredSprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
				}
				else {
					coloredSprite = Resources.PalettedSpriteDatabase.RepaletteSprite(
						sprite.DefaultBasicSprite, paletteArgs);
				}

				sprite.Add(colorParams.GetString(i), coloredSprite);
			}
			paletteArgs.DefaultMapping = null;
			return defaultMappings;
		}
	}
}

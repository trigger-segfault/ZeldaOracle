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

		/// <summary>Adds CompositeSprite commands to the script reader.</summary>
		public void AddColorCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("COLOR", (int) Modes.Root,
				"string name, string colorationGroup, (string colorGroups...), (int indexX, int indexY)",
			delegate (CommandParam parameters) {
				bool continueSprite = parameters.HasPrefix("continue");
				if (!continueSprite && parameters.HasPrefix())
					ThrowCommandParseError("Invalid use of prefix");
				spriteName = parameters.GetString(0);
				if (continueSprite) {
					ContinueSprite<ColorSprite>(spriteName);
				}
				else {
					sprite = new ColorSprite(parameters.GetString(1));
					AddResource<ISprite>(spriteName, sprite);
				}

				if (paletteArgs.Dictionary == null)
					ThrowCommandParseError("Cannot create color sprite with no palette dictionary specified!");
				if (SourceMode != SourceModes.SpriteSheet)
					ThrowCommandParseError("Cannot create color sprite with a source that is not a sprite sheet!");

				paletteArgs.Image = SpriteSheet.Image;
				string colorationGroup = parameters.GetString(1);
				CommandParam colorParams = parameters.GetParam(2);
				int colorCount = colorParams.ChildCount;
				Color[][] defaultMappings = new Color[colorCount][];
				for (int i = 0; i < colorCount; i++) {
					string colorGroup = colorParams.GetString(i);
					defaultMappings[i] = new Color[PaletteDictionary.ColorGroupSize];
					for (int j = 0; j < PaletteDictionary.ColorGroupSize; j++) {
						defaultMappings[i][j] = paletteArgs.Dictionary.GetMappedColor(
							colorGroup, (LookupSubtypes) j);
					}
				}

				paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(3));
				for (int i = 0; i < colorParams.ChildCount; i++) {
					paletteArgs.DefaultMapping = defaultMappings[i];
					ISprite coloredSprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					ColorSprite.Add(colorParams.GetString(i), coloredSprite);
				}
				paletteArgs.DefaultMapping = null;

				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
			// SETUP SpriteSet
			//=====================================================================================
			AddCommand("COLOR", (int) Modes.SpriteSet,
				"string colorationGroup, (string colorGroups...), (int indexX, int indexY)",
				"string colorationGroup, (string colorGroups...), (int indexX, int indexY), (int startX, int startY), (int width, int height)",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix() && parameters.ChildCount == 5 && !parameters.HasPrefix("dynamic"))
					ThrowCommandParseError("Invalid use of prefix with overload!");

				if (paletteArgs.Dictionary == null)
					ThrowCommandParseError("Cannot create color sprite with no palette dictionary specified!");
				if (SourceMode != SourceModes.SpriteSheet)
					ThrowCommandParseError("Cannot create color sprite with a source that is not a sprite sheet!");
				paletteArgs.Image = SpriteSheet.Image;
				string colorationGroup = parameters.GetString(0);
				CommandParam colorParams = parameters.GetParam(1);
				int colorCount = colorParams.ChildCount;
				Color[][] defaultMappings = new Color[colorCount][];
				for (int i = 0; i < colorCount; i++) {
					string colorGroup = colorParams.GetString(i);
					defaultMappings[i] = new Color[PaletteDictionary.ColorGroupSize];
					for (int j = 0; j < PaletteDictionary.ColorGroupSize; j++) {
						defaultMappings[i][j] = paletteArgs.Dictionary.GetMappedColor(
							colorGroup, (LookupSubtypes) j);
					}
				}

				if (parameters.HasPrefix("dynamic")) {

					Point2I start = parameters.GetPoint(3, Point2I.Zero);
					Point2I dimensions = parameters.GetPoint(4, EditingSpriteSet.Dimensions);
					if (dimensions.X == 0) dimensions.X = EditingSpriteSet.Width;
					if (dimensions.Y == 0) dimensions.Y = EditingSpriteSet.Height;

					for (int x = 0; x < dimensions.X; x++) {
						for (int y = 0; y < dimensions.Y; y++) {
							Point2I point = new Point2I(x, y);
							ColorSprite colorSprite = new ColorSprite(colorationGroup);
							paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(2) + point);
							for (int i = 0; i < colorParams.ChildCount; i++) {
								paletteArgs.DefaultMapping = defaultMappings[i];
								ISprite coloredSprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
								colorSprite.Add(colorParams.GetString(i), coloredSprite);
							}
							EditingSpriteSet.SetSprite(start + point, colorSprite);
						}
					}
				}
				else if (parameters.HasPrefix("single")) {
					sprite = new ColorSprite(colorationGroup);
					paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(2));
					for (int i = 0; i < colorParams.ChildCount; i++) {
						paletteArgs.DefaultMapping = defaultMappings[i];
						ISprite coloredSprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
						ColorSprite.Add(colorParams.GetString(i), coloredSprite);
					}
					EditingSpriteSet.SetSprite(parameters.GetPoint(2), sprite);
				}
				else {
					ThrowCommandParseError("COLOR(string colorationGroup, (string colorGroups...), " +
						"(int indexX, int indexY)) must only be called with DYNAMIC or SINGLE prefix!");
				}
				paletteArgs.DefaultMapping = null;
				Mode |= Modes.ColorSprite;
			});
			//=====================================================================================
		}
	}
}

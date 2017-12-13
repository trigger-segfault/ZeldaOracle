using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripts.Commands;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	public partial class ISpriteSR : ScriptReader {

		/// <summary>Adds CompositeSprite commands to the script reader.</summary>
		public void AddStyleCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("STYLE", (int) Modes.Root,
				"string name, string styleGroup",
			delegate (CommandParam parameters) {
				bool continueSprite = parameters.HasPrefix("continue");
				if (!continueSprite && parameters.HasPrefix())
					ThrowCommandParseError("Invalid use of prefix");
				spriteName = parameters.GetString(0);
				if (continueSprite) {
					ContinueSprite<StyleSprite>(spriteName);
				}
				else {
					sprite = new StyleSprite(parameters.GetString(1));
					AddResource<ISprite>(spriteName, sprite);
				}
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			// SETUP SpriteSet
			//=====================================================================================
			AddCommand("STYLE", (int) Modes.SpriteSet,
				"string styleGroup",
				"string styleGroup, (int indexX, int indexY)",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix() && (parameters.ChildCount == 1) != parameters.HasPrefix("dynamic"))
					ThrowCommandParseError("Invalid use of prefix with overload!");
				string styleGroup = parameters.GetString(0);
				if (parameters.HasPrefix("dynamic")) {
					for (int x = 0; x < EditingSpriteSet.Width; x++) {
						for (int y = 0; y < EditingSpriteSet.Height; y++) {
							EditingSpriteSet.SetSprite(x, y, new StyleSprite(styleGroup));
						}
					}
					singular = false;
				}
				else if (parameters.HasPrefix("single")) {
					sprite = new StyleSprite(styleGroup);
					EditingSpriteSet.SetSprite(parameters.GetPoint(1), sprite);
					singular = true;
				}
				else {
					ThrowCommandParseError("STYLE(string styleGroup) must only be called with DYNAMIC or SINGLE prefix!");
				}
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("ADD", (int) Modes.StyleSprite,
				"string style, string name",
				"string style, (int indexX, int indexY)",
			delegate (CommandParam parameters) {
				if (parameters.GetParam(1).Name == "name") {
					StyleSprite.Add(parameters.GetString(0),
						GetResource<ISprite>(parameters.GetString(1)));
				}
				else if (SourceMode != SourceModes.None) {
					ISprite sprite = source.GetSprite(parameters.GetPoint(1));
					if (paletteArgs.Dictionary != null && SourceMode == SourceModes.SpriteSheet) {
						paletteArgs.Image = SpriteSheet.Image;
						paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(1));
						sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					StyleSprite.Add(parameters.GetString(0),
						sprite);
				}
				else {
					ThrowCommandParseError("Cannot add sprite style with no sprite sheet source!");
				}
			});
			//=====================================================================================
			AddCommand("REPLACE", (int) Modes.StyleSprite,
				"string style, string name",
				"string style, (int indexX, int indexY)",
			delegate (CommandParam parameters) {
				if (parameters.GetParam(1).Name == "name") {
					StyleSprite.Set(parameters.GetString(0),
						GetResource<ISprite>(parameters.GetString(1)));
				}
				else if (SourceMode != SourceModes.None) {
					ISprite sprite = source.GetSprite(parameters.GetPoint(1));
					if (paletteArgs.Dictionary != null && SourceMode == SourceModes.SpriteSheet) {
						paletteArgs.Image = SpriteSheet.Image;
						paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(1));
						sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					StyleSprite.Set(parameters.GetString(0),
						sprite);
				}
				else {
					ThrowCommandParseError("Cannot replace sprite style with no sprite sheet source!");
				}
			});
			//=====================================================================================
			AddCommand("REMOVE", (int) Modes.StyleSprite,
				"string style",
			delegate (CommandParam parameters) {
				StyleSprite.Remove(parameters.GetString(0));
			});
			//=====================================================================================
			// BUILDING SpriteSet
			//=====================================================================================
			AddCommand("ADD", (int) (Modes.SpriteSet |  Modes.StyleSprite),
				"string style, (int indexX, int indexY)",
				"string style, (int indexX, int indexY), (int startX, int startY), (int width, int height)",
			delegate (CommandParam parameters) {
				if (SourceMode != SourceModes.SpriteSheet) {
					ThrowCommandParseError("Cannot add sprite style with no sprite sheet source!");
				}

				Point2I start = parameters.GetPoint(2, Point2I.Zero);
				Point2I dimensions = parameters.GetPoint(3, EditingSpriteSet.Dimensions);
				if (dimensions.X == 0) dimensions.X = EditingSpriteSet.Width;
				if (dimensions.Y == 0) dimensions.Y = EditingSpriteSet.Height;
				if (singular) {
					if (parameters.ChildCount != 2)
						ThrowCommandParseError("Invalid use of overload with SINGLE!");
					dimensions = Point2I.One;
					ISprite sprite = source.GetSprite(parameters.GetPoint(1));
					if (paletteArgs.Dictionary != null && SourceMode == SourceModes.SpriteSheet) {
						paletteArgs.Image = SpriteSheet.Image;
						paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(1));
						sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					StyleSprite.Add(parameters.GetString(0), sprite);
				}

				for (int x = 0; x < dimensions.X; x++) {
					for (int y = 0; y < dimensions.Y; y++) {
						Point2I point = new Point2I(x, y);
						ISprite sprite = source.GetSprite(parameters.GetPoint(1) + point);
						if (paletteArgs.Dictionary != null && SourceMode == SourceModes.SpriteSheet) {
							paletteArgs.Image = SpriteSheet.Image;
							paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(1) + point);
							sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
						}
						StyleSprite styleSprite = EditingSpriteSet.GetSprite(start + point) as StyleSprite;
						styleSprite.Add(parameters.GetString(0), sprite);
					}
				}
			});
			//=====================================================================================
		}
	}
}

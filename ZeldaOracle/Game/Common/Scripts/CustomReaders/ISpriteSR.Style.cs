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
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix("dynamic")) {
					for (int x = 0; x < EditingSpriteSet.Width; x++) {
						for (int y = 0; y < EditingSpriteSet.Height; y++) {
							EditingSpriteSet.SetSprite(x, y, new StyleSprite(parameters.GetString(0)));
						}
					}
					Mode |= Modes.StyleSprite;
				}
				else {
					ThrowCommandParseError("STYLE (string name, string styleGroup) must only be called with DYNAMIC prefix!");
				}
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("ADD", (int) Modes.StyleSprite,
				"string style, string name",
				"string style, (int indexX, int indexY)",
			delegate (CommandParam parameters) {
				if (parameters.GetParam(1).Name == "name") {
					StyleSprite.AddStyle(parameters.GetString(0),
						GetResource<ISprite>(parameters.GetString(1)));
				}
				else if (SourceMode != SourceModes.None) {
					ISprite sprite = source.GetSprite(parameters.GetPoint(1));
					if (paletteArgs.Dictionary != null && SourceMode == SourceModes.SpriteSheet) {
						paletteArgs.Image = SpriteSheet.Image;
						paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(1));
						sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					StyleSprite.AddStyle(parameters.GetString(0),
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
					StyleSprite.SetStyle(parameters.GetString(0),
						GetResource<ISprite>(parameters.GetString(1)));
				}
				else if (SourceMode != SourceModes.None) {
					ISprite sprite = source.GetSprite(parameters.GetPoint(1));
					if (paletteArgs.Dictionary != null && SourceMode == SourceModes.SpriteSheet) {
						paletteArgs.Image = SpriteSheet.Image;
						paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(1));
						sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					StyleSprite.SetStyle(parameters.GetString(0),
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
				StyleSprite.RemoveStyle(parameters.GetString(0));
			});
			//=====================================================================================
			// BUILDING SpriteSet
			//=====================================================================================
			AddCommand("ADD", (int) (Modes.SpriteSet |  Modes.StyleSprite),
				"string style, (int indexX, int indexY)",
			delegate (CommandParam parameters) {
				if (SourceMode != SourceModes.SpriteSheet) {
					ThrowCommandParseError("Cannot add sprite style with no sprite sheet source!");
				}
				for (int x = 0; x < EditingSpriteSet.Width; x++) {
					for (int y = 0; y < EditingSpriteSet.Height; y++) {
						ISprite sprite = source.GetSprite(parameters.GetPoint(1) + new Point2I(x, y));
						if (paletteArgs.Dictionary != null && SourceMode == SourceModes.SpriteSheet) {
							paletteArgs.Image = SpriteSheet.Image;
							paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(1) + new Point2I(x, y));
							sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
						}
						StyleSprite styleSprite = EditingSpriteSet.GetSprite(x, y) as StyleSprite;
						styleSprite.AddStyle(parameters.GetString(0), sprite);
					}
				}
			});
			//=====================================================================================
		}
	}
}

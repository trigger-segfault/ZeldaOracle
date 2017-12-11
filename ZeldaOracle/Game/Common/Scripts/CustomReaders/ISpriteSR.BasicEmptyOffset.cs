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
		public void AddBasicCommands() {

			//=====================================================================================
			// EMPTY SETUP
			//=====================================================================================
			AddCommand("EMPTY", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix()) {
					ThrowCommandParseError("Invalid use of prefix");
				}
				spriteName = parameters.GetString(0);
				sprite = new EmptySprite();
				AddResource<ISprite>(spriteName, sprite);
				Mode |= Modes.EmptySprite;
			});
			//=====================================================================================
			AddCommand("EMPTY", (int) Modes.SpriteSet,
				"",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix("dynamic")) {
					for (int x = 0; x < EditingSpriteSet.Width; x++) {
						for (int y = 0; y < EditingSpriteSet.Height; y++) {
							EditingSpriteSet.SetSprite(x, y, new EmptySprite());
						}
					}
				}
				else {
					ThrowCommandParseError("EMPTY (void) must only be called with DYNAMIC prefix!");
				}
			});
			//=====================================================================================
			// BASIC SETUP
			//=====================================================================================
			AddCommand("BASIC", (int)Modes.Root,
				"string name, (int gridLocationX, int gridLocationY), (int drawOffsetX, int drawOffsetY) = (0, 0)",
			delegate (CommandParam parameters) {
				if (SourceMode != SourceModes.SpriteSheet) {
					ThrowCommandParseError("SOURCE must be specified as a sprite sheet when building a BASIC sprite!");
				}
				bool continueSprite = parameters.HasPrefix("continue");
				if (!continueSprite && parameters.HasPrefix())
					ThrowCommandParseError("Invalid use of prefix");
				spriteName = parameters.GetString(0);
				sprite = new BasicSprite(
					SpriteSheet,
					parameters.GetPoint(1),
					parameters.GetPoint(2, Point2I.Zero));
				if (continueSprite) {
					ContinueSprite<BasicSprite>(spriteName);
					SetResource<ISprite>(spriteName, sprite);
				}
				else {
					if (paletteArgs.Dictionary != null) {
						paletteArgs.Image = SpriteSheet.Image;
						paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(1));
						sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					AddResource<ISprite>(spriteName, sprite);
				}
				Mode |= Modes.BasicSprite;
			});
			//=====================================================================================
			AddCommand("BASIC", (int) Modes.SpriteSet,
				"(int gridLocationX, int gridLocationY), (int drawOffsetX, int drawOffsetY) = (0, 0)",
			delegate (CommandParam parameters) {
				if (SourceMode != SourceModes.SpriteSheet) {
					ThrowCommandParseError("SOURCE must be specified as a sprite sheet when building a BASIC sprite!");
				}
				if (parameters.HasPrefix("dynamic")) {
					for (int x = 0; x < EditingSpriteSet.Width; x++) {
						for (int y = 0; y < EditingSpriteSet.Height; y++) {
							ISprite sprite = new BasicSprite(
								SpriteSheet,
								parameters.GetPoint(0) + new Point2I(x, y),
								parameters.GetPoint(1, Point2I.Zero));
							if (paletteArgs.Dictionary != null) {
								paletteArgs.Image = SpriteSheet.Image;
								paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(0) + new Point2I(x, y));
								sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
							}
							EditingSpriteSet.SetSprite(x, y, sprite);
						}
					}
				}
				else {
					ThrowCommandParseError("BASIC (int gridLocationX, int gridLocationY), " +
						"(int drawOffsetX, int drawOffsetY) = (0, 0) must only be called with DYNAMIC prefix!");
				}
			});
			//=====================================================================================
			// OFFSET SETUP
			//=====================================================================================
			AddCommand("OFFSET", (int) Modes.Root,
				"string name, string spriteName, (int drawOffsetX, int drawOffsetY)",
				"string name, (int indexX, int indexY), (int drawOffsetX, int drawOffsetY)",
				"string name, string sourceName, (int indexX, int indexY), (int drawOffsetX, int drawOffsetY)",
			delegate (CommandParam parameters) {
				bool continueSprite = parameters.HasPrefix("continue");
				if (!continueSprite && parameters.HasPrefix())
					ThrowCommandParseError("Invalid use of prefix");
				ISprite newSprite;
				if (parameters.ChildCount == 3 && parameters.GetParam(1).Type == CommandParamType.String) {
					ISprite resourceSprite = GetResource<ISprite>(parameters.GetString(1));
					newSprite = new OffsetSprite(resourceSprite, parameters.GetPoint(2));
				}
				else {
					if (SourceMode != SourceModes.None) {
						ThrowCommandParseError("Cannot build an offset sprite from source when no source is set!");
					}
					ISpriteSheet newSource = source;
					Point2I index;
					Point2I drawOffset;
					if (parameters.GetParam(1).Type == CommandParamType.String) {
						source = GetResource<ISpriteSheet>(parameters.GetString(1));
						index = parameters.GetPoint(2);
						drawOffset = parameters.GetPoint(3);
					}
					else {
						index = parameters.GetPoint(1);
						drawOffset = parameters.GetPoint(2);
					}
					
					newSprite = new OffsetSprite(newSource.GetSprite(index), drawOffset);
				}
				if (continueSprite) {
					ContinueSprite<OffsetSprite>(spriteName);
					sprite = newSprite;
					SetResource<ISprite>(spriteName, sprite);
				}
				else {
					sprite = newSprite;
					AddResource<ISprite>(spriteName, sprite);
				}
				Mode |= Modes.OffsetSprite;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("SIZE", (int) Modes.BasicSprite,
				"(int width, int height)",
			delegate (CommandParam parameters) {
				BasicSprite.SourceRect = new Rectangle2I(
					BasicSprite.SourceRect.Point, parameters.GetPoint(0));
			});
			//=====================================================================================
		}
	}
}

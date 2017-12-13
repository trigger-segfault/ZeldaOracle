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
				"(int indexX, int indexY)",
				"(int startX, int startY), (int width, int height)",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix() && (parameters.ChildCount != 1) != parameters.HasPrefix("dynamic"))
					ThrowCommandParseError("Invalid use of prefix with overload!");
				if (parameters.HasPrefix("dynamic")) {

					Point2I start = parameters.GetPoint(0, Point2I.Zero);
					Point2I dimensions = parameters.GetPoint(1, EditingSpriteSet.Dimensions);
					if (dimensions.X == 0) dimensions.X = EditingSpriteSet.Width;
					if (dimensions.Y == 0) dimensions.Y = EditingSpriteSet.Height;

					for (int x = 0; x < dimensions.X; x++) {
						for (int y = 0; y < dimensions.Y; y++) {
							EditingSpriteSet.SetSprite(start + new Point2I(x, y), new EmptySprite());
						}
					}
					singular = false;
				}
				else if (parameters.HasPrefix("single")) {
					sprite = new EmptySprite();
					EditingSpriteSet.SetSprite(parameters.GetPoint(0), sprite);
					singular = true;
				}
				else {
					ThrowCommandParseError("EMPTY (void) must only be called with DYNAMIC or SINGLE prefix!");
				}
				Mode |= Modes.EmptySprite;
			});
			//=====================================================================================
			// BASIC SETUP
			//=====================================================================================
			AddCommand("BASIC", (int)Modes.Root,
				"string name, (int indexX, int indexY), (int drawOffsetX, int drawOffsetY) = (0, 0)",
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
				"(int indexX, int indexY), (int drawOffsetX, int drawOffsetY) = (0, 0)",
				"(int indexX, int indexY), (int drawOffsetX, int drawOffsetY), (int startX, int startY), (int width, int height)",
			delegate (CommandParam parameters) {
				if (SourceMode != SourceModes.SpriteSheet) {
					ThrowCommandParseError("SOURCE must be specified as a sprite sheet when building a BASIC sprite!");
				}
				if (parameters.HasPrefix() && parameters.ChildCount != 2 && parameters.HasPrefix("single"))
					ThrowCommandParseError("Invalid use of prefix with overload!");
				if (parameters.HasPrefix("dynamic")) {

					Point2I start = parameters.GetPoint(2, Point2I.Zero);
					Point2I dimensions = parameters.GetPoint(3, EditingSpriteSet.Dimensions);
					if (dimensions.X == 0) dimensions.X = EditingSpriteSet.Width;
					if (dimensions.Y == 0) dimensions.Y = EditingSpriteSet.Height;

					for (int x = 0; x < dimensions.X; x++) {
						for (int y = 0; y < dimensions.Y; y++) {
							Point2I point = new Point2I(x, y);
							ISprite sprite = new BasicSprite(
								SpriteSheet,
								parameters.GetPoint(0) + point,
								parameters.GetPoint(1, Point2I.Zero));
							if (paletteArgs.Dictionary != null) {
								paletteArgs.Image = SpriteSheet.Image;
								paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(0) + point);
								sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
							}
							EditingSpriteSet.SetSprite(start + point, sprite);
						}
					}
					singular = false;
				}
				else if (parameters.HasPrefix("single")) {
					sprite = new BasicSprite(
								SpriteSheet,
								parameters.GetPoint(0),
								parameters.GetPoint(1, Point2I.Zero));
					if (paletteArgs.Dictionary != null) {
						paletteArgs.Image = SpriteSheet.Image;
						paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(0));
						sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					EditingSpriteSet.SetSprite(parameters.GetPoint(0), sprite);
					singular = true;
				}
				else {
					ThrowCommandParseError("BASIC (int gridLocationX, int gridLocationY), " +
						"(int drawOffsetX, int drawOffsetY) = (0, 0) must only be called with DYNAMIC prefix!");
				}
				Mode |= Modes.BasicSprite;
			});
			//=====================================================================================
			// OFFSET SETUP
			//=====================================================================================
			AddCommand("OFFSET", (int) Modes.Root, new string[] {
				"string name, string spriteName, (int drawOffsetX, int drawOffsetY)",
				// Int needs to go before string as int/float defaults to string.
				"string name, (int indexX, int indexY), (int drawOffsetX, int drawOffsetY)",
				"string name, (string spriteName, string definition), (int drawOffsetX, int drawOffsetY)",
				"string name, ((int indexX, int indexY), string definition), (int drawOffsetX, int drawOffsetY)",
				"string name, (string sourceName, (int indexX, int indexY)), (int drawOffsetX, int drawOffsetY)",
				"string name, (string sourceName, (int indexX, int indexY), string definition), (int drawOffsetX, int drawOffsetY)",
				"string name, (int drawOffsetX, int drawOffsetY)",
			}, delegate (CommandParam parameters) {
				bool continueSprite = parameters.HasPrefix("continue");
				if (!continueSprite && parameters.HasPrefix())
					ThrowCommandParseError("Invalid use of prefix");
				spriteName = parameters.GetString(0);
				Point2I drawOffset = parameters.GetPoint(parameters.ChildCount - 1);
				if (continueSprite) {
					if (parameters.ChildCount != 2)
						ThrowCommandParseError("Cannot use CONTINUE with this overload!");
					ContinueSprite<OffsetSprite>(spriteName);
					OffsetSprite.DrawOffset += drawOffset;
				}
				else {
					if (parameters.ChildCount != 3)
						ThrowCommandParseError("Must use CONTINUE with this overload!");
					ISprite spriteToOffset = GetSpriteFromParams(parameters, 1);
					sprite = new OffsetSprite(spriteToOffset, drawOffset);
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

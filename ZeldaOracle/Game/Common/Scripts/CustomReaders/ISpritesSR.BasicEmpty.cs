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
	public partial class ISpritesSR : ScriptReader {

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
				string spriteName = parameters.GetString(0);
				AddResource<ISprite>(spriteName, new EmptySprite());
			});
			//=====================================================================================
			AddCommand("EMPTY", (int) Modes.SpriteSet,
				"",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix("dynamic")) {
					for (int x = 0; x < SpriteSet.Width; x++) {
						for (int y = 0; y < SpriteSet.Height; y++) {
							SpriteSet.SetSprite(x, y, new EmptySprite());
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
					sprite = null;
					spriteName = null;
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
					for (int x = 0; x < SpriteSet.Width; x++) {
						for (int y = 0; y < SpriteSet.Height; y++) {
							ISprite sprite = new BasicSprite(
								SpriteSheet,
								parameters.GetPoint(0) + new Point2I(x, y),
								parameters.GetPoint(1, Point2I.Zero));
							if (paletteArgs.Dictionary != null) {
								paletteArgs.Image = SpriteSheet.Image;
								paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(0) + new Point2I(x, y));
								sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
							}
							SpriteSet.SetSprite(x, y, sprite);
						}
					}
				}
				else {
					ThrowCommandParseError("BASIC (int gridLocationX, int gridLocationY), " +
						"(int drawOffsetX, int drawOffsetY) = (0, 0) must only be called with DYNAMIC prefix!");
				}
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

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
		public void AddCompositeCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("COMPOSITE", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				bool continueSprite = parameters.HasPrefix("continue");
				if (parameters.HasPrefix())
					ThrowCommandParseError("Invalid use of prefix");
				spriteName = parameters.GetString(0);
				sprite = new CompositeSprite();
				AddResource<ISprite>(spriteName, sprite);
				Mode |= Modes.CompositeSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE COMPOSITE", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix())
					ThrowCommandParseError("Invalid use of prefix");
				spriteName = parameters.GetString(0);
				ContinueSprite<CompositeSprite>(spriteName);
				Mode |= Modes.CompositeSprite;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("ADD", (int) Modes.CompositeSprite, new string[] {
				"Sprite sprite, Point drawOffset = (0, 0), string flip = none, string rotation = none",
				/*"string spriteName, drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				// Int needs to go before string as int/float defaults to string.
				"(int indexX, int indexY), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"(string animationName, int substrip), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"(string spriteName, string definition), drawOffset(int x, int y) = (0, 0), string flip = none, string rotation = none",
				"((int indexX, int indexY), string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"(string sourceName, (int indexX, int indexY)), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"(string sourceName, (int indexX, int indexY), string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",*/
			}, delegate (CommandParam parameters) {
				ISprite addSprite = GetSpriteFromParams(parameters);
				Point2I drawOffset = parameters.GetPoint(1);
				Flip flip = ParseFlip(parameters.GetString(2));
				Rotation rotation = ParseRotation(parameters.GetString(3));
				CompositeSprite.AddSprite(addSprite, drawOffset, flip, rotation);
				/*if (parameters.GetParam(0).Type == CommandParamType.String) {

				}
				if (parameters.GetParam(0).Name == "name") {
					CompositeSprite.AddSprite(
						GetResource<ISprite>(parameters.GetString(0)),
						parameters.GetPoint(1, Point2I.Zero));
				}
				else if (SourceMode != SourceModes.None) {
					ISprite sprite = source.GetSprite(parameters.GetPoint(0));
					if (paletteArgs.Dictionary != null && SourceMode == SourceModes.SpriteSheet) {
						paletteArgs.Image = SpriteSheet.Image;
						paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(0));
						sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					CompositeSprite.AddSprite(
						sprite,
						parameters.GetPoint(1, Point2I.Zero));
				}
				else {
					ThrowCommandParseError("Cannot add sprite with no sprite sheet source!");
				}*/
			});
			//=====================================================================================
			AddCommand("INSERT", (int) Modes.CompositeSprite, new string[] {
				"int index, Sprite sprite, Point drawOffset = (0, 0), string flip = none, string rotation = none",
				// Int needs to go before string as int/float defaults to string.
				/*"int index, (int indexX, int indexY), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"int index, (string animationName, int substrip), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"int index, (string spriteName, string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"int index, ((int indexX, int indexY), string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"int index, (string sourceName, (int indexX, int indexY)), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"int index, (string sourceName, (int indexX, int indexY), string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",*/
			}, delegate (CommandParam parameters) {
				int index = parameters.GetInt(0);
				ISprite addSprite = GetSpriteFromParams(parameters, 1);
				Point2I drawOffset = parameters.GetPoint(2);
				Flip flip = ParseFlip(parameters.GetString(3));
				Rotation rotation = ParseRotation(parameters.GetString(4));
				CompositeSprite.InsertSprite(index, addSprite, drawOffset, flip, rotation);
				/*if (parameters.GetParam(1).Name == "name") {
					CompositeSprite.InsertSprite(
						parameters.GetInt(0),
						GetResource<ISprite>(parameters.GetString(1)),
						parameters.GetPoint(2, Point2I.Zero));
				}
				else if (SourceMode != SourceModes.None) {
					ISprite sprite = source.GetSprite(parameters.GetPoint(1));
					if (paletteArgs.Dictionary != null && SourceMode == SourceModes.SpriteSheet) {
						paletteArgs.Image = SpriteSheet.Image;
						paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(1));
						sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					CompositeSprite.InsertSprite(
						parameters.GetInt(0),
						sprite,
						parameters.GetPoint(2, Point2I.Zero));
				}
				else {
					ThrowCommandParseError("Cannot insert sprite with no sprite sheet source!");
				}*/
			});
			//=====================================================================================
			// NOTE: Unlike other drawOffset functions. Not setting the draw offset 
			// will cause the new sprite to retain the last sprite's draw offset.
			AddCommand("REPLACE", (int) Modes.CompositeSprite, new string[] {
				"int index, Sprite sprite",
				"int index, Sprite sprite, Point drawOffset = (0, 0), string flip = none, string rotation = none",
				// Int needs to go before string as int/float defaults to string.
				/*"int index, (int indexX, int indexY)",
				"int index, (int indexX, int indexY), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"int index, (string animationName, int substrip)",
				"int index, (string animationName, int substrip), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"int index, (string spriteName, string definition)",
				"int index, (string spriteName, string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"int index, ((int indexX, int indexY), string definition)",
				"int index, ((int indexX, int indexY), string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"int index, (string sourceName, (int indexX, int indexY))",
				"int index, (string sourceName, (int indexX, int indexY)), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"int index, (string sourceName, (int indexX, int indexY), string definition)",
				"int index, (string sourceName, (int indexX, int indexY), string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",*/
			}, delegate (CommandParam parameters) {
				int index = parameters.GetInt(0);
				ISprite addSprite = GetSpriteFromParams(parameters, 1);
				if (parameters.ChildCount == 2) {
					CompositeSprite.ReplaceSprite(index, addSprite);
				}
				else {
					Point2I drawOffset = parameters.GetPoint(2);
					Flip flip = ParseFlip(parameters.GetString(3));
					Rotation rotation = ParseRotation(parameters.GetString(4));
					CompositeSprite.ReplaceSprite(index, addSprite, drawOffset, flip, rotation);
				}
				/*if (parameters.GetParam(1).Name == "name") {
					if (parameters.ChildCount == 2) {
						CompositeSprite.ReplaceSprite(
							parameters.GetInt(0),
							GetResource<ISprite>(parameters.GetString(1)));
					}
					else {
						CompositeSprite.ReplaceSprite(
							parameters.GetInt(0),
							GetResource<ISprite>(parameters.GetString(1)),
							parameters.GetPoint(2, Point2I.Zero));
					}
				}
				else if (SourceMode != SourceModes.None) {
					ISprite sprite = source.GetSprite(parameters.GetPoint(1));
					if (paletteArgs.Dictionary != null && SourceMode == SourceModes.SpriteSheet) {
						paletteArgs.Image = SpriteSheet.Image;
						paletteArgs.SourceRect = SpriteSheet.GetSourceRect(parameters.GetPoint(1));
						sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					if (parameters.ChildCount == 2) {
						CompositeSprite.ReplaceSprite(
							parameters.GetInt(0), sprite);
					}
					else {
						CompositeSprite.ReplaceSprite(
							parameters.GetInt(0), sprite,
							parameters.GetPoint(2, Point2I.Zero));
					}
				}
				else {
					ThrowCommandParseError("Cannot replace sprite with no sprite sheet source!");
				}*/
			});
			//=====================================================================================
			AddCommand("REMOVE", (int) Modes.CompositeSprite,
				"int index",
			delegate (CommandParam parameters) {
				CompositeSprite.RemoveSprite(parameters.GetInt(0));
			});
			//=====================================================================================
			AddCommand("COMBINE", (int) Modes.CompositeSprite,
				"string compositeSpriteName, Point drawOffset = (0, 0)",
				"int index, string compositeSpriteName, Point drawOffset = (0, 0)",
			delegate (CommandParam parameters) {
				if (parameters.GetParam(0).IsValidType(CommandParamType.Integer)) {
					CompositeSprite combineSprite = GetSprite<CompositeSprite>(parameters.GetString(1));
					int index = parameters.GetInt(0);
					foreach (OffsetSprite part in combineSprite.GetSprites()) {
						CompositeSprite.InsertSprite(index, part.Sprite,
							part.DrawOffset + parameters.GetPoint(2));
						index++;
					}
				}
				else {
					CompositeSprite combineSprite = GetSprite<CompositeSprite>(parameters.GetString(0));
					foreach (OffsetSprite part in combineSprite.GetSprites()) {
						CompositeSprite.AddSprite(part.Sprite,
							part.DrawOffset + parameters.GetPoint(1));
					}
				}
			});
			//=====================================================================================
			AddCommand("SIZE", (int) Modes.CompositeSprite,
				"Point size",
			delegate (CommandParam parameters) {
				if (CompositeSprite.SpriteCount > 0) {
					OffsetSprite sprite = CompositeSprite.LastSprite();
					if (sprite.Sprite is BasicSprite) {
						BasicSprite basic = (BasicSprite) sprite.Sprite;
						basic.SourceRect = new Rectangle2I(basic.SourceRect.Point, parameters.GetPoint(0));
					}
					else {
						ThrowCommandParseError("Cannot call SIZE when the last sprite is not a basic sprite!");
					}
				}
				else {
					ThrowCommandParseError("Cannot call SIZE when no sprites have been added!");
				}
			});
			//=====================================================================================
			AddCommand("Offset", (int) Modes.CompositeSprite,
				"Point offset",
			delegate (CommandParam parameters) {
				//var subSprites = CompositeSprite.GetSprites();
				foreach (OffsetSprite subSprite in CompositeSprite.GetSprites()) {
					subSprite.DrawOffset += parameters.GetPoint(0);
				}
			});
			//=====================================================================================
		}
	}
}

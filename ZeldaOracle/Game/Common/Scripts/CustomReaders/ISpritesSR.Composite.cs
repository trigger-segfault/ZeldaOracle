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
		public void AddCompositeCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("COMPOSITE", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				if (parameters.HasPrefix("continue")) {
					ContinueSprite<CompositeSprite>(spriteName);
				}
				else {
					sprite = new CompositeSprite();
					AddResource<ISprite>(spriteName, sprite);
				}
				Mode |= Modes.CompositeSprite;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("ADD", (int) Modes.CompositeSprite,
				"string name, (int drawOffsetX, int drawOffsetY) = (0, 0)",
				"(int indexX, int indexY), (int drawOffsetX, int drawOffsetY) = (0, 0)",
			delegate (CommandParam parameters) {
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
				}
			});
			//=====================================================================================
			AddCommand("INSERT", (int) Modes.CompositeSprite,
				"int index, string name, (int drawOffsetX, int drawOffsetY) = (0, 0)",
				"int index, (int indexX, int indexY), (int drawOffsetX, int drawOffsetY) = (0, 0)",
			delegate (CommandParam parameters) {
				if (parameters.GetParam(1).Name == "name") {
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
				}
			});
			//=====================================================================================
			AddCommand("REPLACE", (int) Modes.CompositeSprite,
				"int index, string name, (int drawOffsetX, int drawOffsetY) = (0, 0)",
				"int index, (int indexX, int indexY), (int drawOffsetX, int drawOffsetY) = (0, 0)",
			delegate (CommandParam parameters) {
				if (parameters.GetParam(0).Name == "name") {
					CompositeSprite.ReplaceSprite(
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
					CompositeSprite.ReplaceSprite(
						parameters.GetInt(0),
						sprite,
						parameters.GetPoint(2, Point2I.Zero));
				}
				else {
					ThrowCommandParseError("Cannot replace sprite with no sprite sheet source!");
				}
			});
			//=====================================================================================
			AddCommand("REMOVE", (int) Modes.CompositeSprite,
				"int index",
			delegate (CommandParam parameters) {
				CompositeSprite.RemoveSprite(parameters.GetInt(0));
			});
			//=====================================================================================
			AddCommand("COMBINE", (int) Modes.CompositeSprite,
				"string name, (int drawOffsetX, int drawOffsetY) = (0, 0)",
				"int index, string name, (int drawOffsetX, int drawOffsetY) = (0, 0)",
			delegate (CommandParam parameters) {
				if (parameters.GetParam(0).Name == "name") {
					CompositeSprite combineSprite = GetSprite<CompositeSprite>(parameters.GetString(0));
					foreach (OffsetSprite part in combineSprite.GetSprites()) {
						CompositeSprite.AddSprite(part.Sprite,
							part.DrawOffset + parameters.GetPoint(1, Point2I.Zero));
					}
				}
				else {
					CompositeSprite combineSprite = GetSprite<CompositeSprite>(parameters.GetString(1));
					int index = parameters.GetInt(0);
					foreach (OffsetSprite part in combineSprite.GetSprites()) {
						CompositeSprite.InsertSprite(index, part.Sprite,
							part.DrawOffset + parameters.GetPoint(2, Point2I.Zero));
						index++;
					}
				}
			});
			//=====================================================================================
			AddCommand("SIZE", (int) Modes.CompositeSprite,
				"(int width, int height)",
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
		}
	}
}

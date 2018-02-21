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
	public partial class SpriteSR : ScriptReader {

		/// <summary>Adds CompositeSprite commands to the script reader.</summary>
		public void AddCompositeCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("COMPOSITE", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				sprite = new CompositeSprite();
				AddResource<ISprite>(spriteName, sprite);
				Mode |= Modes.CompositeSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE COMPOSITE", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				ContinueSprite<CompositeSprite>(spriteName);
				Mode |= Modes.CompositeSprite;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("ADD", (int) Modes.CompositeSprite,
				"Sprite sprite, Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1), string flip = none, string rotation = none",
			delegate (CommandParam parameters) {
				ISprite addSprite = GetSpriteFromParams(parameters);
				Point2I drawOffset = parameters.GetPoint(1);
				Rectangle2I? clipping = parameters.GetRectangle(2);
				if (clipping.Value.Size == -Point2I.One)
					clipping = null;
				Flip flip = ParseFlip(parameters.GetString(3));
				Rotation rotation = ParseRotation(parameters.GetString(4));
				CompositeSprite.AddSprite(addSprite, drawOffset, clipping, flip, rotation);
			});
			//=====================================================================================
			AddCommand("ADDTILED", (int) Modes.CompositeSprite,
				"Point sourceIndex, Point range, Point spacing, Point drawOffset = (0, 0)",
			delegate (CommandParam parameters) {
				if (SourceMode == SourceModes.None) {
					ThrowCommandParseError("Cannot add tiled sprites without a sprite source!");
				}
				Point2I sourceIndex = parameters.GetPoint(0);
				Point2I range = parameters.GetPoint(1);
				Point2I spacing = parameters.GetPoint(2);
				Point2I drawOffset = parameters.GetPoint(3);
				for (int x = 0; x < range.X; x++) {
					for (int y = 0; y < range.Y; y++) {
						Point2I point = new Point2I(x, y);
						ISprite addSprite = GetSprite(source, sourceIndex + point);
						CompositeSprite.AddSprite(addSprite, drawOffset + (spacing * point));
					}
				}
			});
			//=====================================================================================
			AddCommand("INSERT", (int) Modes.CompositeSprite,
				"int index, Sprite sprite, Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1), string flip = none, string rotation = none",
			delegate (CommandParam parameters) {
				int index = parameters.GetInt(0);
				ISprite addSprite = GetSpriteFromParams(parameters, 1);
				Point2I drawOffset = parameters.GetPoint(2);
				Rectangle2I? clipping = parameters.GetRectangle(3);
				if (clipping.Value.Size == -Point2I.One)
					clipping = null;
				Flip flip = ParseFlip(parameters.GetString(4));
				Rotation rotation = ParseRotation(parameters.GetString(5));
				CompositeSprite.InsertSprite(index, addSprite, drawOffset, clipping, flip, rotation);
			});
			//=====================================================================================
			// NOTE: Unlike other drawOffset functions. Not setting the draw offset 
			// will cause the new sprite to retain the last sprite's draw offset.
			AddCommand("REPLACE", (int) Modes.CompositeSprite,
				"int index, Sprite sprite",
				"int index, Sprite sprite, Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1), string flip = none, string rotation = none",
			delegate (CommandParam parameters) {
				int index = parameters.GetInt(0);
				ISprite addSprite = GetSpriteFromParams(parameters, 1);
				if (parameters.ChildCount == 2) {
					CompositeSprite.ReplaceSprite(index, addSprite);
				}
				else {
					Point2I drawOffset = parameters.GetPoint(2);
					Rectangle2I? clipping = parameters.GetRectangle(3);
					if (clipping.Value.Size == -Point2I.One)
						clipping = null;
					Flip flip = ParseFlip(parameters.GetString(4));
					Rotation rotation = ParseRotation(parameters.GetString(5));
					CompositeSprite.ReplaceSprite(index, addSprite, drawOffset, clipping, flip, rotation);
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
				"string compositeSpriteName, Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1)",
				"int index, string compositeSpriteName, Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1)",
			delegate (CommandParam parameters) {
				Point2I drawOffset = parameters.GetPoint(parameters.ChildCount - 2);
				Rectangle2I? clipping = parameters.GetRectangle(parameters.ChildCount - 1);
				if (clipping.Value.Size == -Point2I.One)
					clipping = null;
				if (parameters.GetParam(0).IsValidType(CommandParamType.Integer)) {
					CompositeSprite combineSprite = GetSprite<CompositeSprite>(parameters.GetString(1));
					int index = parameters.GetInt(0);
					foreach (OffsetSprite part in combineSprite.GetSprites()) {
						CompositeSprite.InsertOffsetSprite(index, part);
						CompositeSprite.GetSprite(index).DrawOffset += drawOffset;
						if (clipping != null)
							CompositeSprite.GetSprite(index).Clip(clipping.Value);
						index++;
					}
				}
				else {
					CompositeSprite combineSprite = GetSprite<CompositeSprite>(parameters.GetString(0));
					foreach (OffsetSprite part in combineSprite.GetSprites()) {
						CompositeSprite.AddOffsetSprite(part);
						CompositeSprite.GetSprite(CompositeSprite.SpriteCount - 1).DrawOffset += drawOffset;
						if (clipping != null)
							CompositeSprite.GetSprite(CompositeSprite.SpriteCount - 1).Clip(clipping.Value);
					}
				}
			});
			//=====================================================================================
			AddCommand("OFFSET", (int) Modes.CompositeSprite,
				"Point offset",
			delegate (CommandParam parameters) {
				//var subSprites = CompositeSprite.GetSprites();
				foreach (OffsetSprite subSprite in CompositeSprite.GetSprites()) {
					subSprite.DrawOffset += parameters.GetPoint(0);
				}
			});
			//=====================================================================================
			AddCommand("CLIP", (int) Modes.CompositeSprite,
				"Rectangle clipping",
			delegate (CommandParam parameters) {
				CompositeSprite.Clip(parameters.GetRectangle(0));
			});
			//=====================================================================================
		}
	}
}

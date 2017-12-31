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
			AddCommand("MULTIPLE EMPTY", (int) Modes.SpriteSet,
				"Point start = (0, 0), Point span = (0, 0)",
			delegate (CommandParam parameters) {
				editingSetStart = parameters.GetPoint(0);
				editingSetDimensions = parameters.GetPoint(1);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				for (int x = 0; x < editingSpriteSet.Width; x++) {
					for (int y = 0; y < editingSpriteSet.Height; y++) {
						EmptySprite emptySprite = new EmptySprite();
						editingSpriteSet.SetSprite(editingSetStart + new Point2I(x, y), emptySprite);
					}
				}

				singular = false;
				Mode |= Modes.EmptySprite;
			});
			//=====================================================================================
			AddCommand("SINGLE EMPTY", (int) Modes.SpriteSet,
				"Point setIndex",
			delegate (CommandParam parameters) {
				editingSetStart = parameters.GetPoint(0);
				editingSetDimensions = Point2I.One;

				sprite = new EmptySprite();
				editingSpriteSet.SetSprite(editingSetStart, sprite);

				singular = false;
				Mode |= Modes.EmptySprite;
			});
			//=====================================================================================
			// BASIC SETUP
			//=====================================================================================
			AddCommand("BASIC", (int)Modes.Root,
				"string name, Point sourceIndex, Point drawOffset = (0, 0), string flip = none, string rotation = none",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				sprite = BuildBasicSprite(
					source,
					parameters.GetPoint(1),
					parameters.GetPoint(2),
					ParseFlip(parameters.GetString(3)),
					ParseRotation(parameters.GetString(4)));
				AddResource<ISprite>(spriteName, sprite);
				Mode |= Modes.BasicSprite;
			});
			//=====================================================================================
			/*AddCommand("CONTINUE BASIC", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				if (parameters.HasPrefix())
					ThrowCommandParseError("Invalid use of prefix");
				spriteName = parameters.GetString(0);
				ContinueSprite<BasicSprite>(spriteName);
				SetResource<ISprite>(spriteName, sprite);
				Mode |= Modes.BasicSprite;
			});*/
			//=====================================================================================
			AddCommand("MULTIPLE BASIC", (int) Modes.SpriteSet,
				"Point sourceIndex, Point start = (0, 0), Point span = (0, 0), " +
					"Point drawOffset = (0, 0), string flip = none, string rotation = none",
			delegate (CommandParam parameters) {
				Point2I sourceIndex = parameters.GetPoint(0);
				editingSetStart = parameters.GetPoint(1);
				editingSetDimensions = parameters.GetPoint(2);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				Point2I drawOffset = parameters.GetPoint(3);
				Flip flip = ParseFlip(parameters.GetString(4));
				Rotation rotation = ParseRotation(parameters.GetString(5));

				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						Point2I point = new Point2I(x, y);
						BasicSprite basicSprite = BuildBasicSprite(
							source,
							sourceIndex + point,
							drawOffset,
							flip,
							rotation);
						editingSpriteSet.SetSprite(editingSetStart + point, basicSprite);
					}
				}

				singular = false;
				Mode |= Modes.BasicSprite;
			});
			//=====================================================================================
			AddCommand("SINGLE BASIC", (int) Modes.SpriteSet,
				"Point sourceIndex",
			delegate (CommandParam parameters) {
				Point2I sourceIndex = parameters.GetPoint(0);
				editingSetStart = sourceIndex;
				editingSetDimensions = Point2I.One;

				sprite = BuildBasicSprite(
							source,
							sourceIndex,
							Point2I.Zero);
				editingSpriteSet.SetSprite(editingSetStart, sprite);

				singular = true;
				Mode |= Modes.BasicSprite;
			});
			//=====================================================================================
			AddCommand("SINGLE BASIC", (int) Modes.SpriteSet,
				"Point sourceIndex, Point2I setIndex, Point drawOffset = (0, 0), string flip = none, string rotation = none",
			delegate (CommandParam parameters) {
				Point2I sourceIndex = parameters.GetPoint(0);
				editingSetStart = parameters.GetPoint(1);
				editingSetDimensions = Point2I.One;

				Point2I drawOffset = parameters.GetPoint(2);
				Flip flip = ParseFlip(parameters.GetString(3));
				Rotation rotation = ParseRotation(parameters.GetString(4));

				sprite = BuildBasicSprite(
							source,
							sourceIndex,
							drawOffset,
							flip,
							rotation);
				editingSpriteSet.SetSprite(editingSetStart, sprite);

				singular = true;
				Mode |= Modes.BasicSprite;
			});
			//=====================================================================================
			// OFFSET SETUP
			//=====================================================================================
			AddCommand("OFFSET", (int) Modes.Root, new string[] {
				"string name, Sprite sprite, Point drawOffset = (0, 0), string flip = none, string rotation = none",
				// Int needs to go before string as int/float defaults to string.
				/*"string name, (int indexX, int indexY), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"string name, (string spriteName, string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"string name, ((int indexX, int indexY), string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"string name, (string sourceName, (int indexX, int indexY)), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"string name, (string sourceName, (int indexX, int indexY), string definition), drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",
				"string name, drawOffset (int x, int y) = (0, 0), string flip = none, string rotation = none",*/
			}, delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				Point2I drawOffset = parameters.GetPoint(2);
				Flip flip = ParseFlip(parameters.GetString(3));
				Rotation rotation = ParseRotation(parameters.GetString(4));
				ISprite spriteToOffset = GetSpriteFromParams(parameters, 1);
				sprite = new OffsetSprite(spriteToOffset, drawOffset, flip, rotation);
				AddResource<ISprite>(spriteName, sprite);
				Mode |= Modes.OffsetSprite;
			});
			//=====================================================================================
			/*AddCommand("CONTINUE OFFSET", (int) Modes.Root, new string[] {
				"string name, Point drawOffset = (0, 0), string flip = none, string rotation = none",
			}, delegate (CommandParam parameters) {
				bool continueSprite = parameters.HasPrefix("continue");
				if (!continueSprite && parameters.HasPrefix())
					ThrowCommandParseError("Invalid use of prefix");
				spriteName = parameters.GetString(0);
				Point2I drawOffset = parameters.GetPoint(1);
				Flip flip = ParseFlip(parameters.GetString(2));
				Rotation rotation = ParseRotation(parameters.GetString(3));
				ContinueSprite<OffsetSprite>(spriteName);
				OffsetSprite.DrawOffset += drawOffset;
				OffsetSprite.FlipEffects = Flipping.Add(OffsetSprite.FlipEffects, flip);
				OffsetSprite.Rotation = Rotating.Add(OffsetSprite.Rotation, rotation);
				Mode |= Modes.OffsetSprite;
			});*/
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("SIZE", (int) Modes.BasicSprite,
				"Point size",
			delegate (CommandParam parameters) {
				BasicSprite.SourceRect = new Rectangle2I(
					BasicSprite.SourceRect.Point, parameters.GetPoint(0));
			});
			//=====================================================================================
		}
	}
}

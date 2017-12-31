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
				string styleGroup = parameters.GetString(1);

				spriteName = parameters.GetString(0);
				sprite = new StyleSprite(styleGroup);
				AddResource<ISprite>(spriteName, sprite);
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE STYLE", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				ContinueSprite<StyleSprite>(spriteName);
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			// SETUP SpriteSet
			//=====================================================================================
			AddCommand("MULTIPLE STYLE", (int) Modes.SpriteSet,
				"string styleGroup, Point start = (0, 0), Point dimensions = (0, 0)",
			delegate (CommandParam parameters) {
				string styleGroup = parameters.GetString(0);
				editingSetStart = parameters.GetPoint(1);
				editingSetDimensions = parameters.GetPoint(2);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				for (int x = 0; x < editingSpriteSet.Width; x++) {
					for (int y = 0; y < editingSpriteSet.Height; y++) {
						editingSpriteSet.SetSprite(editingSetStart + new Point2I(x, y), new StyleSprite(styleGroup));
					}
				}

				singular = false;
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE MULTIPLE STYLE", (int) Modes.SpriteSet,
				"Point start = (0, 0), Point dimensions = (0, 0)",
			delegate (CommandParam parameters) {
				editingSetStart = parameters.GetPoint(0);
				editingSetDimensions = parameters.GetPoint(1);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				for (int x = 0; x < editingSpriteSet.Width; x++) {
					for (int y = 0; y < editingSpriteSet.Height; y++) {
						GetSprite<StyleSprite>(editingSpriteSet, editingSetStart + new Point2I(x, y));
					}
				}

				singular = false;
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("SINGLE STYLE", (int) Modes.SpriteSet,
				"string styleGroup, Point setIndex",
			delegate (CommandParam parameters) {
				string styleGroup = parameters.GetString(0);
				editingSetStart = parameters.GetPoint(1);
				editingSetDimensions = Point2I.One;

				sprite = new StyleSprite(styleGroup);
				EditingSpriteSet.SetSprite(editingSetStart, sprite);

				singular = true;
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE SINGLE STYLE", (int) Modes.SpriteSet,
				"Point setIndex",
			delegate (CommandParam parameters) {
				editingSetStart = parameters.GetPoint(0);
				editingSetDimensions = Point2I.One;
				
				sprite = GetSprite<StyleSprite>(editingSpriteSet, editingSetStart);

				singular = true;
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("ADD", (int) Modes.StyleSprite,
				"string style, Sprite sprite",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				StyleSprite.Add(style, GetSpriteFromParams(parameters, 1));
			});
			//=====================================================================================
			AddCommand("REPLACE", (int) Modes.StyleSprite,
				"string style, Sprite sprite",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				StyleSprite.Set(style, GetSpriteFromParams(parameters, 1));
			});
			//=====================================================================================
			AddCommand("REMOVE", (int) Modes.StyleSprite,
				"string style",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				StyleSprite.Remove(style);
			});
			//=====================================================================================
			// BUILDING SpriteSet
			//=====================================================================================
			AddCommand("ADD", (int) (Modes.SpriteSet |  Modes.StyleSprite),
				"string style, Point sourceIndex",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				Point2I sourceIndex = parameters.GetPoint(1);

				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						Point2I point = new Point2I(x, y);
						StyleSprite styleSprite = GetSprite<StyleSprite>(editingSpriteSet, editingSetStart + point);
						styleSprite.Add(style, GetSprite(source, sourceIndex + point));
					}
				}
			});
			//=====================================================================================
		}
	}
}

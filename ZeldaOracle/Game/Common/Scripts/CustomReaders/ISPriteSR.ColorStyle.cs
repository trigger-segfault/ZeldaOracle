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

		/// <summary>Adds ColorSprite+StyleSprite commands to the script reader.</summary>
		public void AddColorStyleCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("COLORSTYLE", (int) Modes.Root,
				"string name, string styleGroup, string colorationGroup, (string colorGroups...)",
			delegate (CommandParam parameters) {
				string colorationGroup = parameters.GetString(2);
				editingStyleGroup = parameters.GetString(1);
				editingColorGroups.Clear();

				CommandParam colorParams = parameters.GetParam(3);
				for (int i = 0; i < colorParams.ChildCount; i++) {
					editingColorGroups.Add(colorParams.GetString(i));
				}

				spriteName = parameters.GetString(0);
				sprite = new ColorSprite(colorationGroup);
				AddResource<ISprite>(spriteName, sprite);
				Resources.RegisterStyleGroup(editingStyleGroup, null);

				Mode |= Modes.ColorSprite | Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE COLORSTYLE", (int) Modes.Root,
				"string name, (string colorGroups...)",
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				ContinueSprite<ColorSprite>(spriteName);

				editingStyleGroup = ColorSprite.DefaultStyleSprite.Group;
				editingColorGroups.Clear();

				foreach (DefinedSprite definition in ColorSprite.GetDefinitions()) {
					editingColorGroups.Add(definition.Definition);
				}

				// Add the new color mappings to all existing styles
				if (parameters.ChildCount == 2) {
					Color[][] defaultMappings = null;
					CommandParam colorParams = parameters.GetParam(2);
					for (int i = 0; i < colorParams.ChildCount; i++) {
						editingColorGroups.Add(colorParams.GetString(i));
					}
					foreach (DefinedSprite definition in ColorSprite.DefaultStyleSprite.GetDefinitions()) {
						defaultMappings = ColorizeColorStyleSprite(
							definition.Definition, ColorSprite, Point2I.Zero, defaultMappings);
					}
				}
				
				Mode |= Modes.ColorSprite | Modes.StyleSprite;
			});
			//=====================================================================================
			// SETUP SpriteSet
			//=====================================================================================
			AddCommand("MULTIPLE COLORSTYLE", (int) Modes.SpriteSet,
				"string styleGroup, string colorationGroup, (string colorGroups...), Point start = (0, 0), Point span = (0, 0)",
			delegate (CommandParam parameters) {
				string colorationGroup = parameters.GetString(1);
				editingStyleGroup = parameters.GetString(0);
				editingColorGroups.Clear();

				CommandParam colorParams = parameters.GetParam(2);
				for (int i = 0; i < colorParams.ChildCount; i++) {
					editingColorGroups.Add(colorParams.GetString(i));
				}

				editingSetStart = parameters.GetPoint(3);
				editingSetDimensions = parameters.GetPoint(4);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						editingSpriteSet.SetSprite(editingSetStart + new Point2I(x, y), new ColorSprite(colorationGroup));
					}
				}
				Resources.RegisterStyleGroup(editingStyleGroup, null);
				
				Mode |= Modes.ColorSprite | Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE MULTIPLE COLORSTYLE", (int) Modes.SpriteSet,
				"(string colorGroups...), Point start = (0, 0), Point span = (0, 0)",
				"Point start = (0, 0), Point span = (0, 0)",
			delegate (CommandParam parameters) {
				editingSetStart = parameters.GetPoint(parameters.ChildCount - 2);
				editingSetDimensions = parameters.GetPoint(parameters.ChildCount - 1);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				ColorSprite firstColorSprite = GetSprite<ColorSprite>(editingSpriteSet, editingSetStart);
				editingStyleGroup = firstColorSprite.DefaultStyleSprite.Group;

				foreach (DefinedSprite definition in firstColorSprite.GetDefinitions()) {
					editingColorGroups.Add(definition.Definition);
				}
				
				// Add the new color mappings to all existing styles
				if (parameters.ChildCount == 3) {
					Color[][] defaultMappings = null;
					CommandParam colorParams = parameters.GetParam(0);
					for (int i = 0; i < colorParams.ChildCount; i++) {
						editingColorGroups.Add(colorParams.GetString(i));
					}
					for (int x = 0; x < editingSetDimensions.X; x++) {
						for (int y = 0; y < editingSetDimensions.Y; y++) {
							foreach (DefinedSprite definition in firstColorSprite.DefaultStyleSprite.GetDefinitions()) {
								Point2I point = new Point2I(x, y);
								ColorSprite colorSprite = GetSprite<ColorSprite>(
									editingSpriteSet, editingSetStart + point);

								defaultMappings = ColorizeColorStyleSprite(
									definition.Definition, colorSprite, Point2I.Zero, defaultMappings);
							}
						}
					}
				}
				
				// Confirm all sprites are colorSprites
				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						GetSprite<ColorSprite>(editingSpriteSet, editingSetStart + new Point2I(x, y));
					}
				}
				
				Mode |= Modes.ColorSprite | Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("SINGLE COLORSTYLE", (int) Modes.SpriteSet,
				"string styleGroup, string colorationGroup, (string colorGroups...), Point setIndex",
			delegate (CommandParam parameters) {
				string colorationGroup = parameters.GetString(1);
				editingStyleGroup = parameters.GetString(0);
				editingSetStart = parameters.GetPoint(3);
				editingSetDimensions = Point2I.One;

				sprite = new ColorSprite(colorationGroup);
				editingSpriteSet.SetSprite(editingSetStart, ColorSprite);
				
				editingColorGroups.Clear();
				CommandParam colorParams = parameters.GetParam(2);
				for (int i = 0; i < colorParams.ChildCount; i++) {
					editingColorGroups.Add(colorParams.GetString(i));
				}
				Resources.RegisterStyleGroup(editingStyleGroup, null);

				Mode |= Modes.ColorSprite | Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE SINGLE COLORSTYLE", (int) Modes.SpriteSet,
				"(string colorGroups...), Point setIndex",
				"Point setIndex",
			delegate (CommandParam parameters) {
				sprite = GetSprite<ColorSprite>(editingSpriteSet, editingSetStart);

				editingStyleGroup = ColorSprite.DefaultStyleSprite.Group;
				editingSetStart = parameters.GetPoint(parameters.ChildCount - 1);
				editingSetDimensions = Point2I.One;


				editingColorGroups.Clear();
				foreach (DefinedSprite definition in ColorSprite.GetDefinitions()) {
					editingColorGroups.Add(definition.Definition);
				}

				// Add the new color mappings to all existing styles
				if (parameters.ChildCount == 2) {
					Color[][] defaultMappings = null;
					CommandParam colorParams = parameters.GetParam(2);
					for (int i = 0; i < colorParams.ChildCount; i++) {
						editingColorGroups.Add(colorParams.GetString(i));
					}
					foreach (DefinedSprite definition in ColorSprite.DefaultStyleSprite.GetDefinitions()) {
						defaultMappings = ColorizeColorStyleSprite(
							definition.Definition, ColorSprite, Point2I.Zero, defaultMappings);
					}
				}
				
				Mode |= Modes.ColorSprite | Modes.StyleSprite;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("ADD", (int) (Modes.ColorSprite | Modes.StyleSprite),
				"string style, Point sourceIndex",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				Point2I sourceIndex = parameters.GetPoint(1);

				ColorizeColorStyleSprite(style, ColorSprite, sourceIndex);
			});
			//=====================================================================================
			// BUILDING SpriteSet
			//=====================================================================================
			AddCommand("ADD", (int) (Modes.SpriteSet | Modes.ColorSprite |  Modes.StyleSprite),
				"string style, Point sourceIndex",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				Point2I sourceIndex = parameters.GetPoint(1);

				Color[][] defaultMappings = null;
				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						Point2I point = new Point2I(x, y);
						ColorSprite colorSprite = GetSprite<ColorSprite>(
							editingSpriteSet, editingSetStart + point);
						defaultMappings = ColorizeColorStyleSprite(
							style, colorSprite, sourceIndex + point, defaultMappings);
					}
				}
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Colorizes a color sprite with the specified color parameters.</summary>
		private Color[][] ColorizeColorStyleSprite(string style, ColorSprite sprite, Point2I sourceIndex, Color[][] defaultMappings = null) {
			if (paletteArgs.Dictionary == null)
				ThrowCommandParseError("Cannot create color sprite with no palette dictionary specified!");
			if (SourceMode != SourceModes.SpriteSheet && (sprite.DefaultSprite == null  ||
				sprite.DefaultStyleSprite.DefaultSprite == null))
			{
				ThrowCommandParseError("Cannot create color sprite with a source that is not a sprite sheet!");
			}

			if (SourceMode == SourceModes.SpriteSheet) {
				paletteArgs.Image = SpriteSheet.Image;
				paletteArgs.SourceRect = SpriteSheet.GetSourceRect(sourceIndex);
			}
			if (defaultMappings == null) {
				defaultMappings = new Color[editingColorGroups.Count][];
				for (int i = 0; i < editingColorGroups.Count; i++) {
					string colorGroup = editingColorGroups[i];
					defaultMappings[i] = new Color[PaletteDictionary.ColorGroupSize];
					for (int j = 0; j < PaletteDictionary.ColorGroupSize; j++) {
						defaultMappings[i][j] = paletteArgs.Dictionary.GetMappedColor(
							colorGroup, (LookupSubtypes) j);
					}
				}
			}


			for (int i = 0; i < editingColorGroups.Count; i++) {
				string colorGroup = editingColorGroups[i];
				StyleSprite styleSprite;
				if (!sprite.Contains(colorGroup)) {
					styleSprite = new StyleSprite(editingStyleGroup);
					sprite.Add(colorGroup, styleSprite);
					Resources.RegisterStylePreview(editingStyleGroup, styleSprite);
				}
				else {
					styleSprite = (StyleSprite) sprite.Get(colorGroup);
				}
				if (!styleSprite.Contains(style)) {
					paletteArgs.DefaultMapping = defaultMappings[i];
					ISprite coloredStyledSprite;
					if (!sprite.DefaultStyleSprite.Contains(style)) {
						coloredStyledSprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
					}
					else {
						coloredStyledSprite = Resources.PalettedSpriteDatabase.RepaletteSprite(
							(BasicSprite) sprite.DefaultStyleSprite.Get(style), paletteArgs);
					}

					Resources.RegisterStyle(editingStyleGroup, style);
					styleSprite.Add(style, coloredStyledSprite);
				}
			}
			paletteArgs.DefaultMapping = null;
			return defaultMappings;
		}
	}
}

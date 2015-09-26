using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripts {

	public class SpritesSR : NewScriptReader {

		private SpriteBuilder spriteBuilder;
		private Sprite sprite;
		private string spriteName;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public SpritesSR() {
			spriteBuilder = new SpriteBuilder();

			// SPRITE SHEET.

			AddCommand("SpriteSheet", delegate(CommandParam parameters) {
				if (parameters.Count == 1) {
					// Start using the given sprite sheet.
					SpriteSheet sheet = Resources.GetSpriteSheet(parameters.GetString(0));
					spriteBuilder.SpriteSheet = sheet;
				}
				else {
					int i = 1;
					// Create a new sprite sheet.
					Image image = null;
					string imagePath = parameters.GetString(0);
					string sheetName = imagePath;

					if (parameters.Count == 5) {
						imagePath = parameters.GetString(1);
						i = 2;
					}
					
					if (Resources.ImageExists(imagePath))
						image = Resources.GetImage(imagePath);
					else
						image = Resources.LoadImage(Resources.ImageDirectory + imagePath);

					if (sheetName.IndexOf('/') >= 0)
						sheetName = sheetName.Substring(sheetName.LastIndexOf('/') + 1);

					SpriteSheet sheet = new SpriteSheet(image,
							parameters.GetPoint(i + 0),
							parameters.GetPoint(i + 2),
							parameters.GetPoint(i + 1));
					Resources.AddSpriteSheet(sheetName, sheet);
					spriteBuilder.SpriteSheet = sheet;
				}
			});

			// BEGIN/END.
			
			// Sprite <name> <grid-location> <draw-offset = (0, 0)>
			AddCommand("Sprite", delegate(CommandParam parameters) {
				spriteName = parameters.GetString(0);
				sprite = new Sprite(
					spriteBuilder.SpriteSheet,
					parameters.GetPoint(1),
					parameters.GetPoint(2, Point2I.Zero));
				spriteBuilder.Begin(sprite);
			});
			AddCommand("End", delegate(CommandParam parameters) {
				if (sprite != null) {
					spriteBuilder.End();
					Resources.AddSprite(spriteName, sprite);
					sprite = null;
				}
			});

			// BUILDING.
			
			// Add <grid-location> <draw-offset = (0, 0)>
			AddCommand("Add", delegate(CommandParam parameters) {
				spriteBuilder.AddPart(
					parameters.GetPoint(0).X,
					parameters.GetPoint(0).Y,
					parameters.GetPoint(1, Point2I.Zero).X,
					parameters.GetPoint(1, Point2I.Zero).Y);
			});
			// Size <size>
			AddCommand("Size", delegate(CommandParam parameters) {
				spriteBuilder.SetSize(
					parameters.GetPoint(0).X,
					parameters.GetPoint(0).Y);
			});
		}

		// Begins reading the script.
		protected override void BeginReading() {
			sprite = null;
			spriteName = "";
			spriteBuilder.SpriteSheet = null;
		}

		// Ends reading the script.
		protected override void EndReading() {
			sprite = null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
} // end namespace

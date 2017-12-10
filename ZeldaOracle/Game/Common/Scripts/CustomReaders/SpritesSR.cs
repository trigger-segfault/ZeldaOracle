using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripts.Commands;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripts.CustomReaders {

	public class SpritesSR : ScriptReader {

		private SpriteBuilder spriteBuilder;
		private SpriteOld sprite;
		private string spriteName;
		private TemporaryResources resources;
		private bool useTemporary;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public SpritesSR(TemporaryResources resources = null) {

			this.resources		= resources;
			this.useTemporary	= resources != null;
			this.spriteBuilder	= new SpriteBuilder();
			
			//=====================================================================================
			// SPRITE SHEET
			//=====================================================================================
			AddCommand("SpriteSheet",
				"string name",
				"string path, (int cellWidth, int cellHeight), (int spacingX, int spacingY), (int offsetX, int offsetY)",
				"string name, string path, (int cellWidth, int cellHeight), (int spacingX, int spacingY), (int offsetX, int offsetY)",
			delegate(CommandParam parameters) {
				if (parameters.ChildCount == 1) {
					// Start using the given sprite sheet.
					SpriteSheet sheet;
					if (useTemporary && resources != null)
						sheet = resources.GetResource<SpriteSheet>(parameters.GetString(0));
					else
						sheet = Resources.GetResource<SpriteSheet>(parameters.GetString(0));
					spriteBuilder.SpriteSheet = sheet;
				}
				else {
					int i = 1;
					// Create a new sprite sheet.
					Image image = null;
					string imagePath = parameters.GetString(0);
					string sheetName = imagePath;

					if (parameters.ChildCount == 5) {
						imagePath = parameters.GetString(1);
						i = 2;
					}
					
					if (Resources.ContainsImage(imagePath))
						image = Resources.GetResource<Image>(imagePath);
					else
						image = Resources.LoadImage(Resources.ImageDirectory + imagePath);

					if (sheetName.IndexOf('/') >= 0)
						sheetName = sheetName.Substring(sheetName.LastIndexOf('/') + 1);

					SpriteSheet sheet = new SpriteSheet(image,
							parameters.GetPoint(i + 0),
							parameters.GetPoint(i + 2),
							parameters.GetPoint(i + 1));
					Resources.AddResource<SpriteSheet>(sheetName, sheet);
					spriteBuilder.SpriteSheet = sheet;
				}
			});
			//=====================================================================================
			// BEGIN/END
			//=====================================================================================
			AddCommand("Sprite", "string name, (int gridLocationX, int gridLocationY), (int drawOffsetX, int drawOffsetY) = (0, 0)",
			delegate(CommandParam parameters) {
				spriteName = parameters.GetString(0);
				sprite = new SpriteOld(
					spriteBuilder.SpriteSheet,
					parameters.GetPoint(1),
					parameters.GetPoint(2, Point2I.Zero));
				spriteBuilder.Begin(sprite);
			});
			//=====================================================================================
			AddCommand("End", "",
			delegate(CommandParam parameters) {
				if (sprite != null) {
					spriteBuilder.End();
					if (useTemporary && resources != null)
						resources.AddResource<SpriteOld>(spriteName, sprite);
					else
						Resources.AddResource<SpriteOld>(spriteName, sprite);
					sprite = null;
				}
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("Add", "(int gridLocationX, int gridLocationY), (int drawOffsetX, int drawOffsetY) = (0, 0)",
			delegate(CommandParam parameters) {
				spriteBuilder.AddPart(
					parameters.GetPoint(0).X,
					parameters.GetPoint(0).Y,
					parameters.GetPoint(1, Point2I.Zero).X,
					parameters.GetPoint(1, Point2I.Zero).Y);
			});
			//=====================================================================================
			AddCommand("Size", "(int width, int height)",
			delegate(CommandParam parameters) {
				spriteBuilder.SetSize(
					parameters.GetPoint(0).X,
					parameters.GetPoint(0).Y);
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

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

		public bool UseTemporaryResources {
			get { return useTemporary; }
			set { useTemporary = value; }
		}
	}
} // end namespace

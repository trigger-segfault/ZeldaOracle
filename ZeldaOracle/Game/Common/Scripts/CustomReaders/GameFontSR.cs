using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Scripts {
	/*
	 * FORMAT:
	 *
	 * @font [name]
	 * @grid [char_width] [char_height] [char_spacing_x] [char_spacing_y] [offset_x] [offset_y]
	 * @spacing [char_spacing] [line_spacing] [chars_per_row]
	 * @end
	 */
	public class GameFontSR : NewScriptReader {

		// The current font being created.
		private GameFont font;
		// The last loaded font.
		private GameFont finalFont;

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the last loaded font.
		public GameFont Font {
			get { return finalFont; }
		}


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public GameFontSR() {
			
			//=====================================================================================
			AddCommand("Font", "string name",
			delegate(CommandParam parameters) {
				String path = parameters.GetString(0);

				Image image = null;
				if (path.EndsWith(".png")) {
					image = Resources.LoadImageFromFile(Resources.ImageDirectory + path);
					path = path.Substring(0, path.LastIndexOf('.'));
				}
				else {
					image = Resources.LoadImage(Resources.ImageDirectory + path);
				}

				SpriteSheet sheet = new SpriteSheet(image, Point2I.One, Point2I.Zero, Point2I.Zero);
				font = new GameFont(sheet, 1, 0, 1);
				finalFont = font;
				Resources.AddSpriteSheet(path, sheet);
				Resources.AddGameFont(path, font);
			});
			//=====================================================================================
			AddCommand("Grid", "(int charWidth, int charHeight), (int charSpacingX, int charSpacingY), (int offsetX, int offsetY)",
			delegate(CommandParam parameters) {
				font.SpriteSheet.CellSize	= parameters.GetPoint(0);
				font.SpriteSheet.Spacing	= parameters.GetPoint(1);
				font.SpriteSheet.Offset		= parameters.GetPoint(2);
			});
			//=====================================================================================
			AddCommand("Spacing", "int charSpacing, int lineSpacing, int charsPerRow",
			delegate(CommandParam parameters) {
				font.CharacterSpacing	= parameters.GetInt(0);
				font.LineSpacing		= parameters.GetInt(1);
				font.CharactersPerRow	= parameters.GetInt(2);
			});
			//=====================================================================================
			AddCommand("End", "",
			delegate(CommandParam parameters) {
				font = null;
			});
			//=====================================================================================
		}

	
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		// Begins reading the script.
		protected override void BeginReading() {
			font		= null;
			finalFont	= null;
		}

		// Ends reading the script.
		protected override void EndReading() {
			font = null;
		}

	}
}

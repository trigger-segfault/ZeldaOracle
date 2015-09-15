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
public class GameFontSR : ScriptReader {

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
	// Override
	//-----------------------------------------------------------------------------

	// Begins reading the script.
	protected override void BeginReading() {
		font				= null;
		finalFont			= null;
	}

	// Ends reading the script.
	protected override void EndReading() {
		font				= null;
	}

	// Reads a line in the script as a command.
	protected override bool ReadCommand(string command, List<string> args) {
		// Create a new font.
		// @font [fontName]
		if (command == "font") {
			Image image = null;
			if (args[0].EndsWith(".png")) {
				image = Resources.LoadImageFromFile(Resources.FontDirectory + args[0]);
				args[0] = args[0].Substring(0, args[0].LastIndexOf('.'));
			}
			else {
				image = Resources.LoadImage(Resources.FontDirectory + args[0]);
			}
			SpriteSheet sheet = new SpriteSheet(image, Point2I.One, Point2I.Zero, Point2I.Zero);
			font = new GameFont(sheet, 1, 0, 1);
			finalFont = font;
			Resources.AddSpriteSheet(args[0], sheet);
			Resources.AddGameFont(args[0], font);
		}

		// Define the sprite sheet.
		// @grid [char_width] [char_height] [char_spacing_x] [char_spacing_y] [offset_x] [offset_y]
		if (command == "grid") {
			font.SpriteSheet.CellSize = new Point2I(Int32.Parse(args[0]), Int32.Parse(args[1]));
			font.SpriteSheet.Spacing = new Point2I(Int32.Parse(args[2]), Int32.Parse(args[3]));
			font.SpriteSheet.Offset = new Point2I(Int32.Parse(args[4]), Int32.Parse(args[5]));
		}

		// Define the font spacing.
		// @spacing [char_spacing] [line_spacing] [chars_per_row]
		if (command == "spacing") {
			font.CharacterSpacing = Int32.Parse(args[0]);
			font.LineSpacing = Int32.Parse(args[1]);
			font.CharactersPerRow = Int32.Parse(args[2]);
		}

		// Finish creating the current font.
		// @end
		else if (command == "end") {
			font = null;
		}

		// Invalid command
		else {
			return false;
		}

		return true;
	}

}
} // end namespace

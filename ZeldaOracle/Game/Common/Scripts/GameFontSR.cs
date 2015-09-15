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
/** <summary>
 * Script reader for sprite sheets. The script can contain
 * information for multiple sprite sheets with corresponding
 * images in the content folder.
 *
 * FORMAT:
 *
 * @font [name]
 * @grid [char_width] [char_height] [char_spacing_x] [char_spacing_y] [offset_x] [offset_y]
 * @end
 *
 * Note: All declared sprites must be within the
 * @spritesheet and @end commands.
 * </summary> */
public class GameFontSR : ScriptReader {

	//=========== MEMBERS ============
	#region Members

	/** <summary> The current sprite sheet being created. </summary> */
	private GameFont font;
	/** <summary> The last loaded sprite sheet. </summary> */
	private GameFont finalFont;

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets the last loaded sprite sheet. </summary> */
	public GameFont Font {
		get { return finalFont; }
	}

	#endregion
	//=========== OVERRIDE ===========
	#region Override

	/** <summary> Begins reading the script. </summary> */
	protected override void BeginReading() {
		font				= null;
		finalFont			= null;
	}
	/** <summary> Ends reading the script. </summary> */
	protected override void EndReading() {
		font				= null;
		finalFont			= null;
	}
	/** <summary> Reads a line in the script as a command. </summary> */
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
			//Resources.AddSpriteSheet(sheet);
			//Resources.AddSpriteSheet(sheet);
		}

		// Create a new sprite grid to then define grid sprites for.
		// @grid [sprWidth] [sprHeight] [sepX] [sepY] [offsetX] [offsetY]
		if (command == "grid") {
			/*grid = new SpriteGrid();
			grid.spriteWidth  = Int32.Parse(args[0]);
			grid.spriteHeight = Int32.Parse(args[1]);
			grid.spacingX     = Int32.Parse(args[2]);
			grid.spacingY     = Int32.Parse(args[3]);
			grid.offsetX      = Int32.Parse(args[4]);
			grid.offsetY      = Int32.Parse(args[5]);*/
		}

		// Finish creating the current sprite sheet.
		// @end
		else if (command == "end") {
			/*sheet = null;
			grid = null;
			defaultOrigin = Vector2F.Zero;
			defaultOriginCenter = false;*/
		}

		// Invalid command
		else {
			return false;
		}

		return true;
	}

	#endregion
}
} // end namespace

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
 * Used for grid-based sprite sheets
 * </summary> */
public class SpriteGrid {
	public int spriteWidth;
	public int spriteHeight;
	public int spacingX;
	public int spacingY;
	public int offsetX;
	public int offsetY;
}
/** <summary>
 * Script reader for sprite sheets. The script can contain
 * information for multiple sprite sheets with corresponding
 * images in the content folder.
 *
 * FORMAT:
 *
 * @spritesheet [name]
 * @size [width] [height]
 * @sprite [frame_x] [frame_y] [frame_width] [frame_height] [offset_x] [offset_y] [source_width] [source_height]
 * @end
 *
 * Note: All declared sprites must be within the
 * @spritesheet and @end commands.
 * </summary> */
public class SpriteSheetSR : ScriptReader {

	//=========== MEMBERS ============
	#region Members

	/** <summary> The current sprite sheet being created. </summary> */
	private SpriteAtlas sheet;
	/** <summary> The current sprite sheet grid settings being used. </summary> */
	private SpriteGrid grid;
	/** <summary> The last loaded sprite sheet. </summary> */
	private SpriteAtlas finalSheet;
	/** <summary> The default origin used for sprites. </summary> */
	private Vector2F defaultOrigin;
	/** <summary> True if the default sprite origin should be centered. </summary> */
	private bool defaultOriginCenter;

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets the last loaded sprite sheet. </summary> */
	public SpriteAtlas Sheet {
		get { return finalSheet; }
	}

	#endregion
	//=========== OVERRIDE ===========
	#region Override

	/** <summary> Begins reading the script. </summary> */
	protected override void BeginReading() {
		sheet				= null;
		grid				= null;
		finalSheet			= null;
		defaultOrigin		= Vector2F.Zero;
		defaultOriginCenter	= false;
	}
	/** <summary> Ends reading the script. </summary> */
	protected override void EndReading() {
		sheet				= null;
		grid				= null;
	}
	/** <summary> Reads a line in the script as a command. </summary> */
	protected override bool ReadCommand(string command, List<string> args) {
		// Create a new sprite sheet to then define sprites for.
		// @spritesheet [sheetName]
		if (command == "spritesheet") {
			Image image = null;
			if (args[0].EndsWith(".png")) {
				image = Resources.LoadImageFromFile(Resources.SpriteSheetDirectory + args[0]);
				args[0] = args[0].Substring(0, args[0].LastIndexOf('.'));
			}
			else {
				image = Resources.LoadImage(Resources.SpriteSheetDirectory + args[0]);
			}
			sheet = new SpriteAtlas(args[0], image);
			finalSheet = sheet;
			Resources.AddSpriteSheet(sheet);
		}

		// Create a new sprite grid to then define grid sprites for.
		// @grid [sprWidth] [sprHeight] [sepX] [sepY] [offsetX] [offsetY]
		if (command == "grid") {
			grid = new SpriteGrid();
			grid.spriteWidth  = Int32.Parse(args[0]);
			grid.spriteHeight = Int32.Parse(args[1]);
			grid.spacingX     = Int32.Parse(args[2]);
			grid.spacingY     = Int32.Parse(args[3]);
			grid.offsetX      = Int32.Parse(args[4]);
			grid.offsetY      = Int32.Parse(args[5]);
		}

		// Finish creating the current sprite sheet.
		// @end
		else if (command == "end") {
			sheet = null;
			grid = null;
			defaultOrigin = Vector2F.Zero;
			defaultOriginCenter = false;
		}

		// Set the size of the sprite sheet's texture.
		// @size [width] [height]
		else if (command == "size") {
			// (This command is currently unnecessary)
		}

		// Set the default origin of each sprite.
		// @origin [x] [y]
		// origin can be [x] [y], or center
		else if (command == "origin") {
			if (args.Count >= 1 && args[0] == "center")
				defaultOriginCenter = true;
			else
				defaultOrigin = new Vector2F(Single.Parse(args[0]), Single.Parse(args[1]));
		}

		// Define a new sprite in the sheet.
		// @sprite [name] [frameX] [frameY] [frameWidth] [frameHeight] [offsetX] [offsetY] [sourceWidth] [sourceHeight] [originX] [originY]
		// origin can be [x] [y], center, or default
		else if (command == "sprite") {
			Vector2F origin = defaultOrigin;
			if (args.Count >= 10 && args[9] != "default") {
				if (args[9] == "center")
					origin = new Vector2F(Single.Parse(args[3]), Single.Parse(args[4])) / 2.0f;
				else if (args.Count >= 11)
					origin = new Vector2F(Single.Parse(args[9]), Single.Parse(args[10]));
			}
			else if (defaultOriginCenter)  {
				origin = new Vector2F(Single.Parse(args[3]), Single.Parse(args[4])) / 2.0f;
			}

			sheet.AddSprite(args[0],  // Name
				Int32.Parse(args[1]), // Frame X
				Int32.Parse(args[2]), // Frame Y
				Int32.Parse(args[3]), // Frame Width
				Int32.Parse(args[4]), // Frame Height
				Int32.Parse(args[5]), // Offset X
				Int32.Parse(args[6]), // Offset Y
				Int32.Parse(args[7]), // Source Width
				Int32.Parse(args[8]), // Source Height
				origin.X, // Origin X
				origin.Y  // Origin Y
			);
		}

		// Define a new sprite in the sheet from a grid.
		// @gridsprite [locX] [locY] [name]
		else if (command == "gridsprite") {
			sheet.AddSprite(args[2],  // Name
				grid.offsetX + (grid.spriteWidth  + grid.spacingX) * Int32.Parse(args[0]), // Frame X
				grid.offsetY + (grid.spriteHeight + grid.spacingY) * Int32.Parse(args[1]), // Frame Y
				grid.spriteWidth, grid.spriteHeight, 0, 0,
				grid.spriteWidth, grid.spriteHeight, 0, 0
			);
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

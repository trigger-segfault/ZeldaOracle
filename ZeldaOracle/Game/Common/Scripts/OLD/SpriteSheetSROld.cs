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
// Used for grid-based sprite sheets.
public class SpriteGrid {
	public int spriteWidth;
	public int spriteHeight;
	public int spacingX;
	public int spacingY;
	public int offsetX;
	public int offsetY;
}
/*
 * Script reader for sprite sheets. The script can contain
 * information for multiple sprite sheets with corresponding
 * images in the content folder.
 *
 * FORMAT:
 *
 * @spritesheet [name]
 * @grid [cell_width] [cell_height] [cell_spacing_x] [cell_spacing_y] [offset_x] [offset_y]
 * 
 * @sprite [name]
 * @next [x_index] [y_index] [offset_x] [offset_y]
 * @next [x_index] [y_index] [offset_x] [offset_y]
 * 
 * @sprite [name]
 * @next [x_index] [y_index] [offset_x] [offset_y]
 * 
 * @end
 *
 * Note: All declared sprites must be within the
 * @spritesheet and @end commands.
 */
public class SpriteSheetSROld : ScriptReader {

	// The current sprite sheet being created.
	private SpriteSheet sheet;
	// The last loaded sprite sheet.
	private SpriteSheet finalSheet;
	// The current sprite being created.
	private Sprite sprite;
	// The current sprite's name
	private string spriteName;
	// The default origin used for sprites.
	private Point2I defaultDrawOffset;
	// True if the default sprite origin should be centered.
	private bool defaultDrawOffsetCenter;

	//-----------------------------------------------------------------------------
	// Properties
	//-----------------------------------------------------------------------------

	// Gets the last loaded sprite sheet.
	public SpriteSheet Sheet {
		get { return finalSheet; }
	}
	
	//-----------------------------------------------------------------------------
	// Override
	//-----------------------------------------------------------------------------

	// Begins reading the script.
	protected override void BeginReading() {
		sheet					= null;
		finalSheet				= null;
		sprite					= null;
		spriteName				= "";
		defaultDrawOffset		= Point2I.Zero;
		defaultDrawOffsetCenter	= false;
	}

	// Ends reading the script.
	protected override void EndReading() {
		sheet					= null;
		sprite					= null;
		spriteName				= "";
	}

	// Reads a line in the script as a command.
	protected override bool ReadCommand(string command, List<string> args) {
		// Create a new sprite sheet to then define sprites for.
		// @spritesheet [sheetName/imagePath]
		// @spritesheet [sheetName] [imagePath]
		if (command == "spritesheet") {
			string imagePath = args[args.Count - 1];
			string sheetName = imagePath;
			Image image = null;

			if (Resources.ImageExists(imagePath)) {			
				image = Resources.GetImage(imagePath);
			}
			else {
				if (imagePath.EndsWith(".png")) {
					sheetName = imagePath.Substring(0, imagePath.LastIndexOf('.'));
					image = Resources.LoadImageFromFile(Resources.ImageDirectory + imagePath);
				}
				else {
					image = Resources.LoadImage(Resources.ImageDirectory + imagePath);
				}
			}

			if (args.Count == 2)
				sheetName = args[0];

			sheet = new SpriteSheet(image);
			finalSheet = sheet;
			Resources.AddSpriteSheet(sheetName, sheet);
		}

		// Create a new sprite grid to then define grid sprites for.
		// @grid [cell_width] [cell_height] [cell_spacing_x] [cell_spacing_y] [offset_x] [offset_y]
		if (command == "grid") {
			sheet.CellSize = new Point2I(Int32.Parse(args[0]), Int32.Parse(args[1]));
			sheet.Spacing = new Point2I(Int32.Parse(args[2]), Int32.Parse(args[3]));
			sheet.Offset = new Point2I(Int32.Parse(args[4]), Int32.Parse(args[5]));
		}

		// Finish creating the current sprite sheet.
		// @end
		else if (command == "end") {
			sheet					= null;
			sprite					= null;
			spriteName				= "";
			defaultDrawOffset		= Point2I.Zero;
			defaultDrawOffsetCenter	= false;
		}

		// Set the default origin of each sprite.
		// @origin [x] [y]
		// origin can be [x] [y], or center
		else if (command == "origin") {
			if (args.Count >= 1 && args[0] == "center")
				defaultDrawOffsetCenter = true;
			else
				defaultDrawOffset = new Point2I(Int32.Parse(args[0]), Int32.Parse(args[1]));
		}
			
		// Define a new sprite in the sheet.
		// @sprite [spriteName]
		else if (command == "sprite") {
			sprite = null;
			spriteName = args[0];
		}

		// Define the next part of the sprite.
		// @next [x_index] [y_index] [offset_x] [offset_y]
		// draw offset can be [x] [y], center, or default
		else if (command == "next") {
			Point2I drawOffset = defaultDrawOffset;
			if (args.Count >= 3 && args[2] != "default") {
				if (args[2] == "center")
					drawOffset = new Point2I(sheet.CellSize.X, sheet.CellSize.Y) / 2;
				else if (args.Count >= 4)
					drawOffset = new Point2I(Int32.Parse(args[2]), Int32.Parse(args[3]));
			}
			else if (defaultDrawOffsetCenter) {
				drawOffset = new Point2I(sheet.CellSize.X, sheet.CellSize.Y) / 2;
			}

			if (sprite == null) {
				if (spriteName != "") {
					sprite = new Sprite(sheet, new Point2I(Int32.Parse(args[0]), Int32.Parse(args[1])), drawOffset);
					Resources.AddSprite(args[0], sprite);
				}
			}
			else {
				sprite.NextPart = new Sprite(sheet, new Point2I(Int32.Parse(args[0]), Int32.Parse(args[1])), drawOffset);
				sprite = sprite.NextPart;
			}
		}

		// Invalid command
		else {
			return false;
		}

		return true;
	}

}
} // end namespace

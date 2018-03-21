using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.Control {

	// This class is UNUSED at the moment.

	public class EditorWorld {
		
		private EditorControl editorControl;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EditorWorld(EditorControl editorControl) {
			this.editorControl = editorControl;
		}

		
		//-----------------------------------------------------------------------------
		// Tile Methods
		//-----------------------------------------------------------------------------
		
		// Returns true if there is free space to place the given tile at a location.
		public bool CanPlaceTile(TileData tile, Room room, Point2I location, int layer) {
			Point2I size = tile.TileSize;
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					Point2I loc = location + new Point2I(x, y);
					if (room.GetTile(loc, layer) != null)
						return false;
				}
			}
			return true;
		}

		// Place a tile in a room, deleting any other tiles in the way.
		public void PlaceTile(TileDataInstance tile, Room room, Point2I location, int layer) {
			// Remove any tiles in the way.
			Point2I size = tile.PixelSize;
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					TileDataInstance t = room.GetTile(location, layer);
					if (t != null)
						DeleteTile(t);
				}
			}

			// Place the tile.
			room.PlaceTile(tile, location, layer);
		}

		// Delete an already-placed tile.
		public void DeleteTile(TileDataInstance tile) {
			tile.Room.RemoveTile(tile);
			editorControl.OnDeleteObject(tile);
		}
		
		// Delete an already-placed event tile.
		public void DeleteEventTile(ActionTileDataInstance eventTile) {
			eventTile.Room.RemoveActionTile(eventTile);
			editorControl.OnDeleteObject(eventTile);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public EditorControl EditorControl {
			get { return editorControl; }
		}

		public World World {
			get { return editorControl.World; }
		}
	}
}

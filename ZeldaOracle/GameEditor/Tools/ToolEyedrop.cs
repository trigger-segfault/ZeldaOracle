using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using Key = System.Windows.Input.Key;
using ZeldaOracle.Game;

namespace ZeldaEditor.Tools {
	public class ToolEyedrop : EditorTool {

		private static readonly Cursor EyedropperCursor = LoadCursor("Eyedropper");

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolEyedrop() : base("Eyedropper Tool", Key.K) {
			
		}

		
		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------
		
		protected override void OnInitialize() {
			MouseCursor = EyedropperCursor;
		}

		protected override void OnBegin() {
			EditorControl.HighlightMouseTile = false;
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseMove(MouseEventArgs e) {
			Point2I mousePos = e.MousePos();

			if (!ActionMode) {
				// Highlight tiles.
				TileDataInstance tile = LevelDisplay.SampleTile(mousePos, Layer);
				EditorControl.HighlightMouseTile = (tile != null);
				LevelDisplay.CursorPixelSize = (tile != null ? tile.TileSize :
					new Point2I(GameSettings.TILE_SIZE));
			}
			else {
				// Highlight action tiles.
				ActionTileDataInstance actionTile = LevelDisplay.SampleActionTile(mousePos);
				EditorControl.HighlightMouseTile = (actionTile != null);
				if (actionTile != null) {
					LevelDisplay.CursorHalfTileLocation =
						LevelDisplay.SampleLevelHalfTileCoordinates(
							LevelDisplay.GetRoomDrawPosition(actionTile.Room) + actionTile.Position);
					LevelDisplay.CursorPixelSize = actionTile.PixelSize;
				}
			}
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			OnMouseDragMove(e);
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			// Switch back to last placement-based tool.
			if (DragButton == MouseButtons.Left) {
				if (EditorControl.PreviousTool is ToolFill)
					EditorControl.CurrentTool = EditorControl.ToolFill;
				else if (EditorControl.PreviousTool is ToolSquare)
					EditorControl.CurrentTool = EditorControl.ToolSquare;
				else
					EditorControl.CurrentTool = EditorControl.ToolPlace;
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			Point2I mousePos	= e.MousePos();

			if (DragButton == MouseButtons.Left) {
				// Sample the tile.
				if (!ActionMode) {
					TileDataInstance tile = LevelDisplay.SampleTile(mousePos, Layer);
					if (tile != null) {
						EditorControl.SelectedTilesetLocation = -Point2I.One;
						EditorControl.SelectedTileset = null;
						EditorControl.SelectedTileData = tile.TileData;
					}
				}
				else {
					ActionTileDataInstance actionTile = LevelDisplay.SampleActionTile(mousePos);
					if (actionTile != null) {
						EditorControl.SelectedTilesetLocation = -Point2I.One;
						EditorControl.SelectedTileset = null;
						EditorControl.SelectedTileData = actionTile.ActionTileData;
					}
				}
			}
		}
	}
}

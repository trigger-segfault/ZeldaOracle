using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using System.Windows.Input;
using Cursor = System.Windows.Forms.Cursor;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds.Editing;

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

		protected override void OnBegin(ToolEventArgs e) {
			ShowCursor = false;
			CursorPosition = e.SnappedPosition;
			CursorTileSize = Point2I.One;
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseMove(ToolEventArgs e) {
			if (!ActionMode) {
				// Highlight tiles
				TileDataInstance tile = e.SampleTile;
				ShowCursor = (tile != null);
				if (tile != null) {
					CursorPosition = tile.LevelPosition;
					CursorSize = tile.PixelSize;
				}
			}
			else {
				// Highlight action tiles
				ActionTileDataInstance actionTile = e.SampleActionTile;
				ShowCursor = (actionTile != null);
				if (actionTile != null) {
					CursorPosition = actionTile.LevelPosition;
					CursorSize = actionTile.PixelSize;
				}
			}
		}

		protected override void OnMouseDragBegin(ToolEventArgs e) {
			OnMouseDragMove(e);
		}

		protected override void OnMouseDragEnd(ToolEventArgs e) {
			// Switch back to last placement-based tool.
			if (DragButton == MouseButton.Left) {
				if (EditorControl.PreviousTool is ToolFill ||
					EditorControl.PreviousTool is ToolSquare)
					EditorControl.CurrentTool = EditorControl.PreviousTool;
				else
					EditorControl.CurrentTool = EditorControl.ToolPlace;
			}
		}

		protected override void OnMouseDragMove(ToolEventArgs e) {
			if (DragButton == MouseButton.Left) {
				if (!ActionMode) {
					// Sample the tile
					TileDataInstance tile = e.SampleTile;
					if (tile != null) {
						EditorControl.SelectedTilesetLocation = -Point2I.One;
						EditorControl.SelectedTileset = null;
						EditorControl.SelectedTileData = tile.TileData;
					}
				}
				else {
					// Sample the action tile
					ActionTileDataInstance actionTile = e.SampleActionTile;
					if (actionTile != null) {
						EditorControl.SelectedTilesetLocation = -Point2I.One;
						EditorControl.SelectedTileset = null;
						EditorControl.SelectedTileData = actionTile.ActionTileData;
					}
				}
			}
		}

		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		public override int Snapping {
			// Always snap directly
			get { return 1; }
		}
	}
}

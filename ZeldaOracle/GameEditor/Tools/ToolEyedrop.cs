using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using Key = System.Windows.Input.Key;

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

			if (!EditorControl.EventMode) {
				// Highlight tiles.
				TileDataInstance tile = LevelDisplay.SampleTile(mousePos, EditorControl.CurrentLayer);
				EditorControl.HighlightMouseTile = (tile != null);
				LevelDisplay.CursorTileSize = (tile != null ? tile.Size : Point2I.One);
			}
			else {
				// Highlight event tiles.
				EventTileDataInstance eventTile = LevelDisplay.SampleEventTile(mousePos);
				EditorControl.HighlightMouseTile = (eventTile != null);
				if (eventTile != null) {
					LevelDisplay.CursorHalfTileLocation =
						LevelDisplay.SampleLevelHalfTileCoordinates(
							LevelDisplay.GetRoomDrawPosition(eventTile.Room) + eventTile.Position);
					LevelDisplay.CursorTileSize = eventTile.Size;
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
				if (!EditorControl.EventMode) {
					TileDataInstance tile = LevelDisplay.SampleTile(mousePos, EditorControl.CurrentLayer);
					if (tile != null) {
						EditorControl.SelectedTilesetTile = -Point2I.One;
						EditorControl.SelectedTilesetTileData = tile.TileData;
					}
				}
				else {
					EventTileDataInstance eventTile = LevelDisplay.SampleEventTile(mousePos);
				}
			}
		}
	}
}

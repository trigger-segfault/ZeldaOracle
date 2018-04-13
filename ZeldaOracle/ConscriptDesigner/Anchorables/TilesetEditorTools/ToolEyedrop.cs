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
using ConscriptDesigner.Control;
using ZeldaWpf.Resources;

namespace ConscriptDesigner.Anchorables.TilesetEditorTools {
	public class ToolEyedrop : TilesetEditorTool {

		private static readonly Cursor EyedropperCursor = WpfCursors.Eyedropper;//LoadCursor("Eyedropper");

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
			
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseMove(MouseEventArgs e) {
			Point2I mousePos = e.MousePos();
			
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			OnMouseDragMove(e);
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			// Switch back to last placement-based tool.
			if (DragButton == MouseButtons.Left) {
				TilesetEditor.CurrentTool = TilesetEditor.ToolPlace;
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			Point2I mousePos	= e.MousePos();

			if (DragButton == MouseButtons.Left) {
				BaseTileData tile = Tileset.GetTileData(HoverPoint);
				if (tile != null)
					DesignerControl.SelectedTileData = tile;
			}
		}
	}
}

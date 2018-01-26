using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Common.Graphics;
using ConscriptDesigner.Control;
using Key = System.Windows.Input.Key;

namespace ConscriptDesigner.Anchorables.TilesetEditorTools {
	public class ToolPlace : TilesetEditorTool {

		private static readonly Cursor PencilCursor = LoadCursor("Pencil");
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolPlace() : base("Place Tool", Key.P) {
		}


		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------

		protected override void OnInitialize() {
			MouseCursor = PencilCursor;
		}

		protected override void OnBegin() {
			
		}

		protected override void OnCancel() {
			
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseDown(MouseEventArgs e) {
			if (IsDrawing && e.Button.IsOpposite(DragButton)) {
				Cancel();
			}
			else if (e.Button == MouseButtons.Left) {
				if (SelectedTileData == null)
					return;
				Rectangle2I tilesetBounds = new Rectangle2I(Tileset.Dimensions);
				if (tilesetBounds.Contains(TilesetEditor.TrueHoverPoint)) {
					Tileset.SetTileData(TilesetEditor.TrueHoverPoint, SelectedTileData);
					TilesetEditor.Modfied();
				}
			}
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			if (DragButton == MouseButtons.Right) {
				IsDrawing = true;
				OnMouseDragMove(e);
			}
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			if (IsDrawing) {
				IsDrawing = false;
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			if (IsDrawing) {
				Rectangle2I tilesetBounds = new Rectangle2I(Tileset.Dimensions);
				if (tilesetBounds.Contains(TilesetEditor.TrueHoverPoint) && Tileset.GetTileData(TilesetEditor.TrueHoverPoint) != null) {
					Tileset.RemoveTileData(TilesetEditor.TrueHoverPoint);
					TilesetEditor.Modfied();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Drawing Methods
		//-----------------------------------------------------------------------------

		public override void DrawTiles(Graphics2D g, Zone zone) {
			Rectangle2I tilesetBounds = new Rectangle2I(Tileset.Dimensions);
			if (SelectedTileData != null && tilesetBounds.Contains(TilesetEditor.TrueHoverPoint)) {
				DrawTile(g, SelectedTileData, TilesetEditor.TrueHoverPoint, zone, TilesetEditor.FadeColor);
			}
		}

	}
}

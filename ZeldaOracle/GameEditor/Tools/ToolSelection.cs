using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor.Tools {
	public class ToolSelection : EditorTool {
		private Point2I dragBeginTileCoord;
		private bool isCreatingSelectionBox;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolSelection() {
			name = "Selection Tool";
		}

		
		//-----------------------------------------------------------------------------
		// Selection
		//-----------------------------------------------------------------------------

		public override void Cut() {
			Deselect();
		}
		
		public override void Copy() {

		}
		
		public override void Paste() {
			Deselect();
		}
		
		public override void Delete() {
			Deselect();
		}

		public override void SelectAll() {
			isCreatingSelectionBox = false;
			LevelDisplayControl.SetSelectionBox(Point2I.Zero,
				EditorControl.Level.RoomSize * EditorControl.Level.Dimensions);
		}

		public override void Deselect() {
			isCreatingSelectionBox = false;
			LevelDisplayControl.ClearSelectionBox();
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {

		}

		public override void OnBegin() {
			isCreatingSelectionBox = false;
		}

		public override void OnMouseDragBegin(MouseEventArgs e) {
			// Draw a new selecion box.
			if (e.Button == MouseButtons.Left) {
				isCreatingSelectionBox = true;
				Point2I mousePos	= new Point2I(e.X, e.Y);
				dragBeginTileCoord	= LevelDisplayControl.SampleLevelTileCoordinates(mousePos);
				LevelDisplayControl.SetSelectionBox(dragBeginTileCoord, Point2I.One);
			}
		}

		public override void OnMouseDragEnd(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left && isCreatingSelectionBox) {
				isCreatingSelectionBox = false;
			}
		}

		public override void OnMouseDragMove(MouseEventArgs e) {
			// Update selection box.
			if (e.Button == MouseButtons.Left && isCreatingSelectionBox) {
				Point2I mousePos  = new Point2I(e.X, e.Y);
				Point2I tileCoord = LevelDisplayControl.SampleLevelTileCoordinates(mousePos);
				Point2I minCoord  = GMath.Min(dragBeginTileCoord, tileCoord);
				Point2I maxCoord  = GMath.Max(dragBeginTileCoord, tileCoord);
				LevelDisplayControl.SetSelectionBox(minCoord, maxCoord - minCoord + Point2I.One);
			}
		}

		public override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
		}

		public override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
		}

		public override void OnMouseMove(MouseEventArgs e) {
			// Check if mouse is over selection.
			Point2I point = new Point2I(e.X, e.Y);
			Point2I tileCoord = LevelDisplayControl.SampleLevelTileCoordinates(point);
			if (!isCreatingSelectionBox && LevelDisplayControl.SelectionBox.Contains(tileCoord)) {
				MouseCursor = Cursors.SizeAll;
				editorControl.HighlightMouseTile = false;
			}
			else {
				MouseCursor = Cursors.Default;
				editorControl.HighlightMouseTile = true;
			}

			base.OnMouseMove(e);
		}

	}
}

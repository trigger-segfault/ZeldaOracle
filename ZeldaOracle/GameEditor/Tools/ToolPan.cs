using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using Key = System.Windows.Input.Key;

namespace ZeldaEditor.Tools {
	public class ToolPan : EditorTool {
		private static readonly Cursor HandOpenCursor = LoadCursor("HandOpen");
		private static readonly Cursor HandClosedCursor = LoadCursor("HandClosed");

		private bool isPanning;
		private Point2I panStart;
		private Point2I scrollStart;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolPan() : base("Pan Tool", Key.H) {

		}


		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------
		
		protected override void OnBegin() {
			isPanning = false;
			MouseCursor = HandOpenCursor;
		}

		protected override void OnCancel() {
			isPanning = false;
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			if (DragButton == MouseButtons.Left) {
				isPanning = true;
				panStart = e.MousePos() - LevelDisplay.ScrollPosition;
				scrollStart = LevelDisplay.ScrollPosition;
				MouseCursor = HandClosedCursor;
			}
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			if (isPanning) {
				isPanning = false;
				MouseCursor = HandOpenCursor;
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			if (isPanning) {
				LevelDisplay.ScrollPosition = scrollStart + panStart - (e.MousePos() - LevelDisplay.ScrollPosition);
			}
		}
	}
}

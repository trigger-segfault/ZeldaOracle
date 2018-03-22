using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using System.Windows.Input;
using Cursor = System.Windows.Forms.Cursor;

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
		
		protected override void OnBegin(ToolEventArgs e) {
			isPanning = false;
			MouseCursor = HandOpenCursor;
			ShowCursor = false;
		}

		protected override void OnCancel(ToolEventArgs e) {
			isPanning = false;
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseDragBegin(ToolEventArgs e) {
			if (DragButton == MouseButton.Left) {
				isPanning = true;
				scrollStart = ScrollPosition;
				panStart = e.Position - scrollStart;
				MouseCursor = HandClosedCursor;
			}
		}

		protected override void OnMouseDragEnd(ToolEventArgs e) {
			if (isPanning) {
				isPanning = false;
				MouseCursor = HandOpenCursor;
			}
		}

		protected override void OnMouseDragMove(ToolEventArgs e) {
			if (isPanning) {
				ScrollPosition = scrollStart + panStart - (e.Position - ScrollPosition);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.Tools {
	
	public enum EditorTools {
		Pointer		= 0,
		Place		= 1,
		Select		= 2,
		Eyedrop		= 3,
	}


	public abstract class EditorTool {
		protected EditorControl editorControl;
		protected string name;

		private bool			isDragging;
		private MouseButtons	dragButton;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EditorTool() {
			name = "";
		}

		
		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------


		public void Initialize(EditorControl editorControl) {
			this.editorControl = editorControl;
			Initialize();
		}

		
		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public virtual void Initialize() {}
		
		public virtual void OnBegin() {}
		
		public virtual void OnEnd() {}

		public virtual void OnMouseDown(MouseEventArgs e, Room room, Point2I tileLocation) {
			if (!isDragging) {
				isDragging = true;
				dragButton = e.Button;
				OnMouseDragBegin(dragButton, room, tileLocation);
			}
		}

		public virtual void OnMouseUp(MouseEventArgs e, Room room, Point2I tileLocation) {
			if (isDragging && e.Button.HasFlag(dragButton)) {
				isDragging = false;
				OnMouseDragEnd(dragButton, room, tileLocation);
				dragButton = MouseButtons.None;
			}
		}

		public virtual void OnMouseMove(MouseEventArgs e, Room room, Point2I tileLocation) {
			if (isDragging) {
				OnMouseDragMove(dragButton, room, tileLocation);
			}
		}
		
		public virtual void OnMouseDragBegin(MouseButtons buttons, Room room, Point2I tileLocation) {}
		public virtual void OnMouseDragEnd(MouseButtons buttons, Room room, Point2I tileLocation) {}
		public virtual void OnMouseDragMove(MouseButtons buttons, Room room, Point2I tileLocation) {}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public string Name {
			get { return name; }
			set { name = value; }
		}

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}
	}
}

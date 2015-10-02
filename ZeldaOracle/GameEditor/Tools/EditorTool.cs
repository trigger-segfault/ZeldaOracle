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
	public abstract class EditorTool {
		protected EditorControl editorControl;
		protected string		name;
		private bool			isDragging;
		private MouseButtons	dragButton;
		private Cursor			mouseCursor;


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
			this.mouseCursor = Cursors.Default;
			Initialize();
		}

		public void StopDragging() {
			isDragging = false;
		}

		
		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------
		
		// Called when the current layer is changed (or switched to events).
		public virtual void OnChangeLayer() {}

		public virtual void Cut() {}

		public virtual void Copy() {}

		public virtual void Paste() {}

		public virtual void Delete() {}

		public virtual void SelectAll() {}

		public virtual void Deselect() {}


		public virtual void Initialize() {}
		
		public virtual void OnBegin() {}
		
		public virtual void OnEnd() {}
		
		public virtual void OnMouseDragBegin(MouseEventArgs e) {}

		public virtual void OnMouseDragEnd(MouseEventArgs e) {}

		public virtual void OnMouseDragMove(MouseEventArgs e) {}

		public virtual void OnMouseDown(MouseEventArgs e) {
			if (!isDragging) {
				isDragging = true;
				dragButton = e.Button;
				OnMouseDragBegin(e);
			}
		}

		public virtual void OnMouseUp(MouseEventArgs e) {
			if (isDragging && e.Button.HasFlag(dragButton)) {
				isDragging = false;
				OnMouseDragEnd(e);
				dragButton = MouseButtons.None;
			}
		}

		public virtual void OnMouseMove(MouseEventArgs e) {
			if (isDragging && e.Button.HasFlag(dragButton)) {
				OnMouseDragMove(e);
			}
		}


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

		public LevelDisplay LevelDisplayControl {
			get { return editorControl.LevelDisplay; }
		}

		public Cursor MouseCursor {
			get { return mouseCursor; }
			set { mouseCursor = value; }
		}

		public MouseButtons DragButton {
			get { return dragButton; }
		}
		
		public bool IsDragging {
			get { return isDragging; }
		}
	}
}

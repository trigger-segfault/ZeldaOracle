using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;
using ZeldaEditor.WinForms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using System.IO;
using System.Reflection;
using System.Resources;
using ZeldaEditor.Util;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using Key = System.Windows.Input.Key;

namespace ZeldaEditor.Tools {
	public abstract class EditorTool {
		private EditorControl	editorControl;
		private string			name;
		private bool			isDragging;
		private MouseButtons	dragButton;
		private Cursor			mouseCursor;
		private bool            isDrawing;
		private Key				hotKey;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		protected EditorTool(string name, Key hotKey) {
			this.name       = name;
			this.hotKey     = hotKey;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		protected static Cursor LoadCursor(string name) {
			ResourceManager rm = new ResourceManager("ZeldaEditor.g", Assembly.GetExecutingAssembly());
			var input = (Stream)rm.GetObject("resources/cursors/" + name.ToLower() + "cursor.cur");
			input.Position = 0;
			string path = "CustomCursor.cur";
			using (Stream output = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) {
				output.SetLength(0);
				byte[] buffer = new byte[8 * 1024];
				int len;
				while ((len = input.Read(buffer, 0, buffer.Length)) > 0) {
					output.Write(buffer, 0, len);
				}
			}
			Cursor cursor = NativeMethods.LoadCustomCursor(path);
			File.Delete("CustomCursor.cur");
			return cursor;
		}

		protected void StopDragging() {
			isDragging = false;
		}

		protected void UpdateCommands() {
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
		}


		//-----------------------------------------------------------------------------
		// State Methods
		//-----------------------------------------------------------------------------

		public void Initialize(EditorControl editorControl) {
			this.editorControl = editorControl;
			this.mouseCursor = Cursors.Default;
			OnInitialize();
		}
		
		public void Begin() {
			OnBegin();
		}

		public void End() {
			Cancel();
			OnEnd();
		}

		public void Cancel() {
			OnCancel();
			isDrawing = false;
			isDragging = false;
		}

		// Called when the current layer is changed (or switched to events).
		public virtual void LayerChanged() {
			if (CancelOnLayerChange)
				Cancel();
		}


		//-----------------------------------------------------------------------------
		// Mouse Methods
		//-----------------------------------------------------------------------------

		public void MouseDown(MouseEventArgs e) {
			OnMouseDown(e);
			if (!isDragging) {
				isDragging = true;
				dragButton = e.Button;
				OnMouseDragBegin(e);
			}

			// Shortcut open properties
			if (!IsDrawing && e.Button == MouseButtons.Middle) {
				BaseTileDataInstance selectedTile = null;

				if (editorControl.EventMode)
					selectedTile = LevelDisplay.SampleEventTile(e.MousePos());
				else
					selectedTile = LevelDisplay.SampleTile(e.MousePos(), editorControl.CurrentLayer);

				if (selectedTile != null)
					editorControl.PropertyGrid.OpenProperties(selectedTile);
				else
					editorControl.PropertyGrid.CloseProperties();
			}
		}

		public void MouseUp(MouseEventArgs e) {
			OnMouseUp(e);
			if (isDragging && e.Button.HasFlag(dragButton)) {
				isDragging = false;
				OnMouseDragEnd(e);
				dragButton = MouseButtons.None;
			}
		}

		public void MouseMove(MouseEventArgs e) {
			OnMouseMove(e);
			if (isDragging && e.Button.HasFlag(dragButton)) {
				OnMouseDragMove(e);
			}
		}

		public void MouseDoubleClick(MouseEventArgs e) {
			OnMouseDoubleClick(e);
		}

		//-----------------------------------------------------------------------------
		// Virtual Clipboard Methods
		//-----------------------------------------------------------------------------

		public virtual void Cut() { }

		public virtual void Copy() { }

		public virtual void Paste() { }

		public virtual void Delete() { }

		public virtual void SelectAll() { }

		public virtual void Deselect() { }


		//-----------------------------------------------------------------------------
		// Virtual State Methods
		//-----------------------------------------------------------------------------

		protected virtual void OnInitialize() { }

		protected virtual void OnBegin() { }

		protected virtual void OnEnd() { }

		protected virtual void OnCancel() { }

		
		//-----------------------------------------------------------------------------
		// Virtual Mouse Methods
		//-----------------------------------------------------------------------------

		protected virtual void OnMouseDown(MouseEventArgs e) { }

		protected virtual void OnMouseUp(MouseEventArgs e) { }

		protected virtual void OnMouseMove(MouseEventArgs e) { }

		protected virtual void OnMouseDoubleClick(MouseEventArgs e) { }

		protected virtual void OnMouseDragBegin(MouseEventArgs e) { }

		protected virtual void OnMouseDragEnd(MouseEventArgs e) { }

		protected virtual void OnMouseDragMove(MouseEventArgs e) { }


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		public virtual bool CancelOnLayerChange { get { return false; } }

		public virtual bool CancelCountsAsUndo { get { return false; } }

		public virtual bool CanCopyCut { get { return false; } }

		public virtual bool CanDeleteDeselect { get { return false; } }


		//-----------------------------------------------------------------------------
		// Virtual Drawing
		//-----------------------------------------------------------------------------

		public virtual bool DrawHideTile(TileDataInstance tile, Room room, Point2I levelCoord, int layer) {
			return false;
		}
		public virtual bool DrawHideEventTile(EventTileDataInstance eventTile, Room room, Point2I levelPosition) {
			return false;
		}

		public virtual void DrawTile(Graphics2D g, Room room, Point2I position, Point2I levelCoord, int layer) { }

		public virtual void DrawEventTiles(Graphics2D g) { }
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string Name {
			get { return name; }
		}

		public Key HotKey {
			get { return hotKey; }
		}

		public EditorControl EditorControl {
			get { return editorControl; }
		}

		public LevelDisplay LevelDisplay {
			get { return editorControl.LevelDisplay; }
		}

		public Level Level {
			get { return editorControl.Level; }
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

		public bool IsDrawing {
			get { return isDrawing; }
			set { isDrawing = value; }
		}
	}

	public static class MouseExtensions {

		public static bool IsLeftOrRight(this MouseButtons button) {
			return button == MouseButtons.Left || button == MouseButtons.Right;
		}

		public static bool IsOpposite(this MouseButtons button, MouseButtons otherButton) {
			return  (button == MouseButtons.Left && otherButton == MouseButtons.Right) ||
					(button == MouseButtons.Right && otherButton == MouseButtons.Left);
		}

		public static Point2I MousePos(this MouseEventArgs e) {
			return new Point2I(e.X, e.Y);
		}
	}
}

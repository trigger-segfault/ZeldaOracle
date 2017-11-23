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

namespace ZeldaEditor.Tools {
	public abstract class EditorTool {
		protected EditorControl editorControl;
		protected string		name;
		private bool			isDragging;
		private MouseButtons	dragButton;
		private Cursor			mouseCursor;
		private bool            isDrawing;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		static EditorTool() {

		}

		public EditorTool() {
			name = "";
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		protected static Cursor LoadCursor(string name) {
			try {
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
			catch (Exception ex) {
				throw ex;
			}
		}

		public void Initialize(EditorControl editorControl) {
			this.editorControl = editorControl;
			this.mouseCursor = Cursors.Default;
			OnInitialize();
		}

		public void StopDragging() {
			isDragging = false;
		}

		public void UpdateCommands() {
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------
		

		public virtual bool CancelCountsAsUndo { get { return false; } }

		// Called when the current layer is changed (or switched to events).
		public virtual void OnChangeLayer() {}

		public virtual void Cut() {}

		public virtual void Copy() {}

		public virtual void Paste() {}

		public virtual void Delete() {}

		public virtual void SelectAll() {}

		public virtual void Deselect() {}


		protected virtual void OnInitialize() {}

		public void Begin() {
			OnBegin();
		}

		public void End() {
			if (isDragging) {
				isDragging = false;
			}
			Cancel();
		}

		public void Cancel() {
			OnCancel();
			isDrawing = false;
			isDragging = false;
		}

		protected virtual void OnBegin() { }

		protected virtual void OnEnd() { }

		protected virtual void OnCancel() { }

		protected virtual void OnMouseDragBegin(MouseEventArgs e) {}

		protected virtual void OnMouseDragEnd(MouseEventArgs e) {}

		protected virtual void OnMouseDragMove(MouseEventArgs e) {}

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

		protected virtual void OnMouseDown(MouseEventArgs e) {
			
		}

		protected virtual void OnMouseUp(MouseEventArgs e) {
			
		}

		protected virtual void OnMouseMove(MouseEventArgs e) {
			
		}
		
		public virtual void OnMouseDoubleClick(MouseEventArgs e) { }

		public virtual bool CanCopyCut { get { return false; } }

		public virtual bool CanDeleteDeselect { get { return false; } }

		//-----------------------------------------------------------------------------
		// Virtual drawing
		//-----------------------------------------------------------------------------

		public virtual bool DrawHideTile(TileDataInstance tile, Room room, Point2I levelCoord, int layer) {
			return false;
		}
		public virtual bool DrawHideEventTile(EventTileDataInstance eventTile, Room room, Point2I levelPosition) {
			return false;
		}

		public virtual void DrawTile(Graphics2D g, Room room, Point2I position, Point2I levelCoord, int layer) { }
		public virtual void DrawEventTiles(Graphics2D g) { }
		/*public virtual void DrawEventOverride(Point2I levelPosition, out TileDrawModes drawMode) {
			drawMode = TileDrawModes.Default;
		}

		public virtual IEnumerable<KeyValuePair<Point2I, EventTileDataInstance>> DrawEventTiles(out TileDrawModes drawMode) {
			drawMode = TileDrawModes.DontOverride;
			return null;
		}

		public virtual void DrawAboveTiles(Graphics2D g, Rectangle2I viewport) { }

		public virtual void DrawAboveGrid(Graphics2D g, Rectangle2I viewport) { }*/

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

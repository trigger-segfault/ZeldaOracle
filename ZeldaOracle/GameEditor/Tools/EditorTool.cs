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
using ZeldaOracle.Game.Tiles.ActionTiles;
using Key = System.Windows.Input.Key;
using MouseButton = System.Windows.Input.MouseButton;
using ModifierKeys = System.Windows.Input.ModifierKeys;
using FormsControl = System.Windows.Forms.Control;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds.Editing;
using ZeldaOracle.Common.Util;

namespace ZeldaEditor.Tools {
	public abstract class EditorTool {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		public const MouseButton NoButton = (MouseButton) byte.MaxValue;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private EditorControl	editorControl;
		private string			name;
		private bool			isDragging;
		private MouseButton		dragButton;
		private Cursor			mouseCursor;
		private bool			isDrawing;
		private Key				hotKey;
		private HashSet<string>	options;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		protected EditorTool(string name, Key hotKey) {
			this.name       = name;
			this.hotKey     = hotKey;
			this.options	= new HashSet<string>();
		}

		protected void AddOption(string name) {
			options.Add(name);
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
			Cursor cursor = ZeldaEditor.Util.NativeMethods.LoadCustomCursor(path);
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
			LevelDisplay.Cursor = MouseCursor;
			ToolEventArgs args = new ToolEventArgs(EditorControl, this);
			OnBegin(args);
			if (EditorControl.IsLevelOpen)
				OnMouseMove(args);
		}

		public void End() {
			Cancel();
			ToolEventArgs args = new ToolEventArgs(EditorControl, this);
			OnEnd(args);
		}

		public void Finish() {
			ToolEventArgs args = new ToolEventArgs(EditorControl, this);
			OnFinish(args);
			isDrawing = false;
			isDragging = false;
		}

		public void Cancel() {
			ToolEventArgs args = new ToolEventArgs(EditorControl, this);
			OnCancel(args);
			isDrawing = false;
			isDragging = false;
		}

		public void Update() {
			ToolEventArgs args = new ToolEventArgs(EditorControl, this);
			OnUpdate(args);
		}

		// Called when the current layer is changed (or switched to actions).
		public virtual void LayerChanged() {
			if (CancelOnLayerChange)
				Cancel();
		}


		//-----------------------------------------------------------------------------
		// Mouse Methods
		//-----------------------------------------------------------------------------

		public void MouseDown(MouseEventArgs e) {
			ToolEventArgs args = new ToolEventArgs(EditorControl, this, e);
			OnMouseDown(args);
			if (!isDragging) {
				isDragging = true;
				dragButton = args.Button;
				OnMouseDragBegin(args);
			}

			// Shortcut open properties
			if (!IsDrawing && args.Button == MouseButton.Middle) {
				BaseTileDataInstance selectedTile = null;

				if (ActionMode)
					selectedTile = args.SampleActionTile;
				else
					selectedTile = args.SampleTile;

				if (selectedTile != null)
					editorControl.PropertyGrid.OpenProperties(selectedTile);
				else
					editorControl.PropertyGrid.CloseProperties();
			}
		}

		public void MouseUp(MouseEventArgs e) {
			ToolEventArgs args = new ToolEventArgs(EditorControl, this, e);
			OnMouseUp(args);
			if (isDragging && args.Button == dragButton) {
				isDragging = false;
				OnMouseDragEnd(args);
				dragButton = NoButton;
			}
		}

		public void MouseMove(MouseEventArgs e) {
			ToolEventArgs args = new ToolEventArgs(EditorControl, this, e);
			OnMouseMove(args);
			if (isDragging && args.Button == dragButton) {
				OnMouseDragMove(args);
			}
		}

		public void MouseDoubleClick(MouseEventArgs e) {
			ToolEventArgs args = new ToolEventArgs(EditorControl, this, e);
			OnMouseDoubleClick(args);
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

		protected virtual void OnBegin(ToolEventArgs e) { }

		protected virtual void OnEnd(ToolEventArgs e) { }

		protected virtual void OnCancel(ToolEventArgs e) { }

		protected virtual void OnFinish(ToolEventArgs e) { }

		protected virtual void OnUpdate(ToolEventArgs e) { }

		
		//-----------------------------------------------------------------------------
		// Virtual Mouse Methods
		//-----------------------------------------------------------------------------

		protected virtual void OnMouseDown(ToolEventArgs e) { }

		protected virtual void OnMouseUp(ToolEventArgs e) { }

		protected virtual void OnMouseMove(ToolEventArgs e) { }

		protected virtual void OnMouseDoubleClick(ToolEventArgs e) { }

		protected virtual void OnMouseDragBegin(ToolEventArgs e) { }

		protected virtual void OnMouseDragEnd(ToolEventArgs e) { }

		protected virtual void OnMouseDragMove(ToolEventArgs e) { }


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		public virtual bool CancelOnLayerChange { get { return false; } }

		public virtual bool CancelCountsAsUndo { get { return false; } }

		public virtual bool CanCopyCut { get { return false; } }

		public virtual bool CanDeleteDeselect { get { return false; } }

		public virtual int Snapping { get { return GameSettings.TILE_SIZE; } }


		//-----------------------------------------------------------------------------
		// Virtual Drawing
		//-----------------------------------------------------------------------------

		public virtual bool DrawHideTile(TileDataInstance tile, Room room, Point2I levelCoord, int layer) {
			return false;
		}
		public virtual bool DrawHideActionTile(ActionTileDataInstance actionTile, Room room, Point2I levelPosition) {
			return false;
		}

		public virtual void DrawTile(Graphics2D g, Room room, Point2I position, Point2I levelCoord, int layer) { }

		public virtual void DrawActionTiles(Graphics2D g) { }
		

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

		public int Layer {
			get { return editorControl.CurrentLayer; }
		}

		public bool ActionLayer {
			get { return editorControl.ActionLayer; }
		}

		public bool ActionMode {
			get { return editorControl.ActionMode; }
		}

		public Cursor MouseCursor {
			get { return mouseCursor; }
			set {
				mouseCursor = value;
				if (EditorControl.CurrentTool == this)
					LevelDisplay.Cursor = MouseCursor;
			}
		}

		public MouseButton DragButton {
			get { return dragButton; }
		}
		
		public bool IsDragging {
			get { return isDragging; }
		}

		public bool IsDrawing {
			get { return isDrawing; }
			set { isDrawing = value; }
		}

		public HashSet<string> Options {
			get { return options; }
		}

		public ModifierKeys Modifiers {
			get {
				ModifierKeys modifiers = ModifierKeys.None;
				Keys formModifiers = FormsControl.ModifierKeys;
				if (formModifiers.HasFlag(Keys.Control))
					modifiers |= ModifierKeys.Control;
				if (formModifiers.HasFlag(Keys.Shift))
					modifiers |= ModifierKeys.Shift;
				if (formModifiers.HasFlag(Keys.Alt))
					modifiers |= ModifierKeys.Alt;
				return modifiers;
			}
		}

		public Point2I CursorPosition {
			get { return LevelDisplay.CursorPosition; }
			set { LevelDisplay.CursorPosition = value; }
		}

		public Point2I CursorSize {
			get { return LevelDisplay.CursorSize; }
			set { LevelDisplay.CursorSize = value; }
		}

		public Point2I CursorLevelCoord {
			get { return LevelDisplay.CursorLevelCoord; }
			set { LevelDisplay.CursorLevelCoord = value; }
		}

		public Point2I CursorRoomLocation {
			get { return LevelDisplay.CursorRoomLocation; }
			set { LevelDisplay.CursorRoomLocation = value; }
		}

		public Point2I CursorTileSize {
			get { return LevelDisplay.CursorTileSize; }
			set { LevelDisplay.CursorTileSize = value; }
		}

		public Point2I ScrollPosition {
			get { return LevelDisplay.ScrollPosition; }
			set { LevelDisplay.ScrollPosition = value; }
		}

		public bool ShowCursor {
			get { return LevelDisplay.ShowCursor; }
			set { LevelDisplay.ShowCursor = value; }
		}

		/*public Point2I MousePos {
			get { return LevelDisplay.MousePosition; }
		}*/
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

		public static MouseButton ToWpfButton(this MouseButtons button) {
			switch (button) {
			case MouseButtons.Left:		return MouseButton.Left;
			case MouseButtons.Middle:	return MouseButton.Middle;
			case MouseButtons.Right:	return MouseButton.Right;
			case MouseButtons.XButton1:	return MouseButton.XButton1;
			case MouseButtons.XButton2:	return MouseButton.XButton2;

			// Some arbitrary invalid value
			default: return (MouseButton) EditorTool.NoButton;
			}
		}
	}

	public class ToolEventArgs {
		private EditorControl editorControl;
		private LevelDisplay levelDisplay;
		private Level Level { get { return levelDisplay.Level; } }
		private int Layer { get { return editorControl.CurrentLayer; } }

		public MouseButton Button { get; private set; }
		public Point2I Position { get; private set; }
		public Point2I LevelPosition { get; private set; }
		public Point2I SnappedPosition { get; private set; }
		public int Delta { get; private set; }
		public int Clicks { get; private set; }
		public bool IsOpposite(MouseButton button) {
			return	(Button == MouseButton.Left && button == MouseButton.Right) ||
					(Button == MouseButton.Right && button == MouseButton.Left);
		}
		public bool IsLeftOrRight {
			get { return (Button == MouseButton.Left || Button == MouseButton.Right); }
		}
		public bool IsInside { get; private set; }

		public Point2I LevelCoord {
			get { return Level.LevelPositionToCoord(SnappedPosition); }
		}
		public Point2I RoomLocation {
			get { return Level.LevelPositionToRoomLocation(SnappedPosition); }
		}
		public TileDataInstance SampleTile {
			get { return Level.GetTileAt(LevelCoord, Layer); }
		}
		/*public TileDataInstance HoverTile {
			get {
				return Level.GetTileAt(
					Level.LevelPositionToCoord(LevelPosition), Layer);
			}
		}*/
		public ActionTileDataInstance SampleActionTile {
			get { return Level.GetActionTileAt(SnappedPosition); }
		}
		public IEnumerable<ActionTileDataInstance> SampleActionTiles {
			get { return Level.GetActionTilesAt(SnappedPosition); }
		}
		public Room SampleRoom {
			get { return Level.GetRoomAt(RoomLocation); }
		}

		public ToolEventArgs(EditorControl editorControl, EditorTool tool, MouseEventArgs e = null) {
			this.editorControl = editorControl;
			levelDisplay = editorControl.LevelDisplay;
			if (e != null) {
				Button = e.Button.ToWpfButton();
				Position = levelDisplay.ScrollPosition + e.MousePos();
				Delta = e.Delta;
				Clicks = e.Clicks;
			}
			else {
				Button = EditorTool.NoButton;
				Position = levelDisplay.ScrollPosition +
					levelDisplay.PointToClient(Cursor.Position).ToPoint2I();
			}
			if (editorControl.IsLevelOpen)
				LevelPosition = levelDisplay.SampleLevelPosition(Position);
			else
				LevelPosition = Point2I.Zero;
			SnappedPosition = GMath.FloorI(LevelPosition, tool.Snapping);
			IsInside = levelDisplay.IsMouseOver;
		}
	}
}

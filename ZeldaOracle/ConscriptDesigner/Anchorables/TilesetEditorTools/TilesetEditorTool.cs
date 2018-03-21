using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConscriptDesigner.Control;
using ConscriptDesigner.WinForms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using System.IO;
using System.Reflection;
using System.Resources;
using ConscriptDesigner.Util;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using Key = System.Windows.Input.Key;
using ZeldaOracle.Game;
using ConscriptDesigner.Windows;

namespace ConscriptDesigner.Anchorables.TilesetEditorTools {
	public abstract class TilesetEditorTool {
		private TilesetEditorDisplay display;
		private string          name;
		private bool            isDragging;
		private MouseButtons    dragButton;
		private Cursor          mouseCursor;
		private bool            isDrawing;
		private Key             hotKey;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		protected TilesetEditorTool(string name, Key hotKey) {
			this.name       = name;
			this.hotKey     = hotKey;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		protected static Cursor LoadCursor(string name) {
			int retryCount = 0;
			while (retryCount < 4) {
				try {
					ResourceManager rm = new ResourceManager("ConscriptDesigner.g", Assembly.GetExecutingAssembly());
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
					retryCount++;
					if (retryCount == 4) {
						ErrorMessageBox.Show(ex, true);
					}
				}
			}
			return null;
		}

		protected void StopDragging() {
			isDragging = false;
		}

		protected void UpdateCommands() {
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
		}

		protected void DrawTile(Graphics2D g, BaseTileData tile, Point2I position, Zone zone, Color color) {
			position = (GameSettings.TILE_SIZE + 1) * position + 1;
			if (Tileset.UsePreviewSprites)
				TileDataDrawing.DrawTilePreview(g, tile, position, zone, color);
			else
				TileDataDrawing.DrawTile(g, tile, position, zone, color);
		}

		protected void DrawSelectionBox(Graphics2D g, Rectangle2I selectionBox) {
			selectionBox.Point = (GameSettings.TILE_SIZE + 1) * selectionBox.Point;
			selectionBox.Size = (GameSettings.TILE_SIZE + 1) * selectionBox.Size + 1;
			g.DrawRectangle(selectionBox, 1, Color.Black);
			g.DrawRectangle(selectionBox.Inflated(1, 1), 1, Color.White);
			g.DrawRectangle(selectionBox.Inflated(2, 2), 1, Color.Black);
		}

		protected Point2I MouseTile(MouseEventArgs e) {
			Point2I mouse = (TilesetEditor.ScrollPosition + new Point2I(e.X, e.Y)) / TilesetEditor.Scale;
			return mouse / (GameSettings.TILE_SIZE + 1);
		}

		//-----------------------------------------------------------------------------
		// State Methods
		//-----------------------------------------------------------------------------

		public void Initialize(TilesetEditorDisplay display) {
			this.display = display;
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

			// Shortcut eyedropper
			if (!IsDrawing && e.Button == MouseButtons.Middle) {
				BaseTileData tile = HoverTileData;

				if (tile != null) {
					DesignerControl.SelectedTileData = tile;
					DesignerControl.SelectedTileset = null;
					DesignerControl.SelectedTileLocation = -Point2I.One;
					TilesetEditor.Invalidate();
				}
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

		public virtual bool CanCopyCut { get { return false; } }

		public virtual bool CanDeleteDeselect { get { return false; } }


		//-----------------------------------------------------------------------------
		// Virtual Drawing
		//-----------------------------------------------------------------------------

		public virtual void DrawTiles(Graphics2D g, Zone zone) { }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string Name {
			get { return name; }
		}

		public Key HotKey {
			get { return hotKey; }
		}

		public TilesetEditorDisplay TilesetEditor {
			get { return display; }
		}

		public Tileset Tileset {
			get { return display.Tileset; }
		}

		public BaseTileData HoverTileData {
			get { return display.HoverTileData; }
		}

		public Point2I HoverPoint {
			get { return display.HoverPoint; }
		}

		public BaseTileData SelectedTileData {
			get { return DesignerControl.SelectedTileData; }
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
			return (button == MouseButtons.Left && otherButton == MouseButtons.Right) ||
					(button == MouseButtons.Right && otherButton == MouseButtons.Left);
		}

		public static Point2I MousePos(this MouseEventArgs e) {
			return new Point2I(e.X, e.Y);
		}

		public static Point2I MouseTile(this MouseEventArgs e) {
			return GMath.FloorI((new Vector2F(e.X, e.Y) - 1) / (GameSettings.TILE_SIZE + 1));
		}
	}
}

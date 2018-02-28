using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;
using ZeldaEditor.Control;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using ZeldaOracle.Common.Graphics.Sprites;
using System.Windows.Threading;
using Size = System.Drawing.Size;
using ZeldaEditor.Util;

namespace ZeldaEditor.WinForms {

	public class TilesetDisplay : GraphicsDeviceControl {

		private static ContentManager content;
		private static SpriteBatch spriteBatch;

		private EditorWindow	editorWindow;
		private EditorControl	editorControl;

		private StoppableTimer dispatcherTimer;

		private bool needsToInvalidate;

		protected Point2I mouse;
		protected Point2I hoverPoint;
		private Point2I hoverSize;
		private List<BaseTileData> filteredTileData;
		private int columns;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			content		= new ContentManager(Services, "Content");
			spriteBatch	= new SpriteBatch(GraphicsDevice);

			editorControl.SetGraphics(spriteBatch, GraphicsDevice, content);

			filteredTileData = null;
			needsToInvalidate = false;
			columns = 1;

			// Wire the events.
			MouseMove	+= OnMouseMove;
			MouseDown	+= OnMouseDown;
			MouseLeave	+= OnMouseLeave;
			PostReset	+= OnPostReset;

			// Start the timer to refresh the panel.
			//Application.Idle += delegate { Invalidate(); };
			this.ResizeRedraw = true;

			//UpdateTileset();

			dispatcherTimer = StoppableTimer.StartNew(
				TimeSpan.FromMilliseconds(15),
				DispatcherPriority.Render,
				delegate {
					if (editorControl.IsActive)
						TimerUpdate();
				});
			/*dispatcherTimer = new DispatcherTimer(
				TimeSpan.FromMilliseconds(15),
				DispatcherPriority.Render,
				delegate {
					if (editorControl.IsActive)
						TimerUpdate();
				},
				System.Windows.Application.Current.Dispatcher);*/
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				content.Unload();
			}

			base.Dispose(disposing);
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public event EventHandler HoverChanged;

		
		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/*public Point2I GetTileCoord(Point2I point) {
			return ((point - Point2I.One) / (GameSettings.TILE_SIZE + 1));
		}

		public Point2I GetSelectedTileLocation() {
			for (int x = 0; x < Tileset.Width; x++) {
				for (int y = 0; y < Tileset.Height; y++) {
					BaseTileData tileData = Tileset.GetTileData(x, y);
					if (tileData != null && tileData == editorControl.SelectedTileData) {
						return new Point2I(x, y);
					}
				}
			}
			return -Point2I.One;
		}*/


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void TimerUpdate() {
			if (editorControl.PlayAnimations || needsToInvalidate) {
				needsToInvalidate = false;
				Invalidate();
			}
			if (TileListMode && ClientSize.Width != AutoScrollMinSize.Width)
				UpdateSize();
		}

		private void OnPostReset(object sender, EventArgs e) {
			if (HoverTileData != null) {
				if (TileListMode) {
					SelectedTileLocation = hoverPoint;
					SelectedTileset = Tileset;
				}
				else {
					SelectedTileLocation = Tileset.GetTileDataOrigin(hoverPoint);
					SelectedTileset = Tileset;
				}
				SelectedTileData = HoverTileData;
				Invalidate();
			}
			ScrollPosition = Point2I.Zero;
		}

		private void OnMouseDown(object sender, MouseEventArgs e) {
			if (HoverTileData != null) {
				if (!TileListMode) {
					SelectedTileLocation = Tileset.GetTileDataOrigin(hoverPoint);
					SelectedTileset = Tileset;
				}
				SelectedTileData = HoverTileData;
				Invalidate();
			}

			this.Focus();
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			mouse = (ScrollPosition + new Point2I(e.X, e.Y));
			UpdateHoverSprite();
		}
		private void OnMouseLeave(object sender, EventArgs e) {
			mouse = -Point2I.One;
			UpdateHoverSprite();
		}

		private void UpdateSize() {
			Point2I size = new Point2I(ClientSize.Width, 1);
			if (TileListMode) {
				columns = Math.Max(1, (ClientSize.Width - 1) / (GameSettings.TILE_SIZE + 1));
				if (filteredTileData != null)
					size.Y = 1 + ((filteredTileData.Count + columns - 1) / columns) * (GameSettings.TILE_SIZE + 1);
				size.X = ClientSize.Width;
			}
			else {
				size = Tileset.Dimensions * (GameSettings.TILE_SIZE + 1) + Point2I.One;
			}
			AutoScrollMinSize = new Size(size.X, size.Y);
			UpdateHoverSprite();
			needsToInvalidate = true;
		}

		private void UpdateHoverSprite() {
			int column = mouse.X / (GameSettings.TILE_SIZE + 1);
			int row = mouse.Y / (GameSettings.TILE_SIZE + 1);
			Point2I point = new Point2I(column, row);
			Point2I newHoverPoint = -Point2I.One;
			if (mouse >= Point2I.Zero && column < columns && IsValidHoverPoint(ref point, out hoverSize)) {
				newHoverPoint = point;
			}
			if (newHoverPoint != hoverPoint) {
				hoverPoint = newHoverPoint;
				if (HoverChanged != null)
					HoverChanged(this, EventArgs.Empty);
				if (!editorControl.PlayAnimations)
					Invalidate();
			}
		}

		private bool IsValidHoverPoint(ref Point2I point, out Point2I hoverSize) {
			if (TileListMode) {
				int index = (point.Y * columns) + point.X;
				hoverSize = Point2I.One;
				if (filteredTileData != null)
					return index < filteredTileData.Count;
				else
					return false;
			}
			else {
				hoverSize = Point2I.One;
				if (Tileset != null && point < Tileset.Dimensions) {
					Point2I origin = Tileset.GetTileDataOrigin(point);
					if (origin != -Point2I.One) {
						if (!Tileset.UsePreviewSprites)
							hoverSize = Tileset.GetTileDataAtOrigin(origin).Size;
						point = origin;
					}
					return true;
				}
				return false;
			}
		}

		public void UpdateTileset(List<BaseTileData> filteredTileData = null) {
			this.filteredTileData = filteredTileData;
			UpdateSize();
			Invalidate();
		}

		public void UpdateZone() {
			Invalidate();
		}


		//-----------------------------------------------------------------------------
		// Overriden methods
		//-----------------------------------------------------------------------------

		protected override void Draw() {
			if (!editorControl.IsResourcesLoaded)
				return;
			editorControl.UpdateTicks();
			Graphics2D g = new Graphics2D(spriteBatch);
			//g.SetRenderTarget(GameData.RenderTargetGame);
			GameData.PaletteShader.TilePalette = Zone.Palette;
			GameData.PaletteShader.ApplyPalettes();
			TileDataDrawing.RewardManager = editorControl.RewardManager;
			TileDataDrawing.Level = editorControl.Level;
			TileDataDrawing.Room = null;
			TileDataDrawing.Extras = false;
			TileDataDrawing.PlaybackTime = editorControl.Ticks;

			g.Begin(GameSettings.DRAW_MODE_DEFAULT);

			//Point2I selectedTileLocation = GetSelectedTileLocation();

			// Draw the tileset.
			g.Clear(Color.White);
			g.PushTranslation(-ScrollPosition);
			/*for (int y = 0; y < Tileset.Height; y++) {
				for (int x = 0; x < Tileset.Width; x++) {
					BaseTileData tileData = Tileset.GetTileDataAtOrigin(x, y);
					if (tileData != null) {
						int spacing = 1;
						Point2I drawPos = new Point2I(x, y) * (GameSettings.TILE_SIZE + spacing) + spacing;

						TileDataDrawing.DrawTilePreview(g, tileData, drawPos, Zone);
						if (Tileset.UsePreviewSprites)
							TileDataDrawing.DrawTilePreview(g, tileData, drawPos, Zone);
						else
							TileDataDrawing.DrawTile(g, tileData, drawPos, Zone);
					}
				}
			}*/

			Point2I selectionPoint = -Point2I.One;

			if (TileListMode) {
				int startRow = (ScrollPosition.Y + 1) / (GameSettings.TILE_SIZE + 1);
				int startIndex = startRow * columns;
				int endRow = (ScrollPosition.Y + ClientSize.Height + 1 + GameSettings.TILE_SIZE) / (GameSettings.TILE_SIZE + 1);
				int endIndex = (endRow + 1) * columns;
				for (int i = startIndex; i < endIndex && filteredTileData != null && i < filteredTileData.Count; i++) {
					BaseTileData tile = filteredTileData[i];
					int row = i / columns;
					int column = i % columns;
					int x = 1 + column * (GameSettings.TILE_SIZE + 1);
					int y = 1 + row * (GameSettings.TILE_SIZE + 1);

					if (tile == SelectedTileData)
						selectionPoint = new Point2I(column, row);

					try {
						TileDataDrawing.DrawTilePreview(g, tile, new Point2I(x, y), Zone);
					}
					catch (Exception) {

					}
				}
			}
			else {
				for (int indexX = 0; indexX < Tileset.Width; indexX++) {
					for (int indexY = 0; indexY < Tileset.Height; indexY++) {
						BaseTileData tile = Tileset.GetTileDataAtOrigin(indexX, indexY);
						if (tile != null) {
							int x = 1 + indexX * (GameSettings.TILE_SIZE + 1);
							int y = 1 + indexY * (GameSettings.TILE_SIZE + 1);

							try {
								if (Tileset.UsePreviewSprites)
									TileDataDrawing.DrawTilePreview(g, tile, new Point2I(x, y), Zone);
								else
									TileDataDrawing.DrawTile(g, tile, new Point2I(x, y), Zone);
							}
							catch (Exception) {

							}
						}
					}
				}
				if (SelectedTileLocation >= Point2I.Zero && SelectedTileset == Tileset) {
					selectionPoint = SelectedTileLocation;
				}
			}

			if (selectionPoint != -Point2I.One) {
				Point2I selectedSize = SelectedTileData.Size;
				if (Tileset == null || Tileset.UsePreviewSprites)
					selectedSize = Point2I.One;
				Rectangle2I selectRect = new Rectangle2I(
					selectionPoint * (GameSettings.TILE_SIZE + 1),
					selectedSize * GameSettings.TILE_SIZE + 2);
				g.DrawRectangle(selectRect, 1, Color.Black);
				g.DrawRectangle(selectRect.Inflated(1, 1), 1, Color.White);
				g.DrawRectangle(selectRect.Inflated(2, 2), 1, Color.Black);
			}

			if (hoverPoint != -Point2I.One) {
				Rectangle2I selectRect = new Rectangle2I(
					hoverPoint * (GameSettings.TILE_SIZE + 1),
					hoverSize * GameSettings.TILE_SIZE + 2);
				g.DrawRectangle(selectRect, 1, Color.Black);
				g.DrawRectangle(selectRect.Inflated(1, 1), 1, Color.White);
				g.DrawRectangle(selectRect.Inflated(2, 2), 1, Color.Black);
			}

			// Draw the selection box.
			/*if (selectedTileLocation >= Point2I.Zero) {
				Point2I tilePoint = selectedTileLocation * (GameSettings.TILE_SIZE + 1);
				//g.Translate(-this.HorizontalScroll.Value, -this.VerticalScroll.Value);
				g.DrawRectangle(new Rectangle2I(tilePoint, (Point2I) GameSettings.TILE_SIZE + 1), 1, Color.White);
				g.DrawRectangle(new Rectangle2I(tilePoint + 1, (Point2I) GameSettings.TILE_SIZE - 1), 1, Color.Black);
				g.DrawRectangle(new Rectangle2I(tilePoint - 1, (Point2I) GameSettings.TILE_SIZE + 3), 1, Color.Black);
				//g.ResetTranslation();
			}*/

			g.PopTranslation();

			g.End();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorWindow EditorWindow {
			get { return editorWindow; }
			set { editorWindow = value; }
		}

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}

		public Tileset Tileset {
			get { return editorControl.Tileset; }
		}

		public Zone Zone {
			get { return editorControl.Zone ?? GameData.ZONE_DEFAULT; }
		}

		public Tileset SelectedTileset {
			get { return editorControl.SelectedTileset; }
			set { editorControl.SelectedTileset = value; }
		}

		public BaseTileData SelectedTileData {
			get { return editorControl.SelectedTileData; }
			set { editorControl.SelectedTileData = value; }
		}

		public Point2I SelectedTileLocation {
			get { return editorControl.SelectedTilesetLocation; }
			set { editorControl.SelectedTilesetLocation = value; }
		}

		public Point2I ScrollPosition {
			get { return new Point2I(HorizontalScroll.Value, VerticalScroll.Value); }
			set {
				AutoScrollPosition = new System.Drawing.Point(
					GMath.Clamp(value.X, HorizontalScroll.Minimum, HorizontalScroll.Maximum),
					GMath.Clamp(value.Y, VerticalScroll.Minimum, VerticalScroll.Maximum)
				);
			}
		}

		public bool TileListMode {
			get { return Tileset == null; }
		}

		public BaseTileData HoverTileData {
			get {
				if (TileListMode) {
					if (hoverPoint == -Point2I.One || filteredTileData == null)
						return null;
					int index = (hoverPoint.Y * columns) + hoverPoint.X;
					return filteredTileData[index];
				}
				else {
					if (hoverPoint == -Point2I.One)
						return null;
					return Tileset.GetTileData(hoverPoint.X, hoverPoint.Y);
				}
			}
		}
	}
}


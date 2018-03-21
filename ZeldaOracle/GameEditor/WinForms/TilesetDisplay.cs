using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaEditor.Control;
using System.Windows.Threading;
using ZeldaEditor.Util;
using ZeldaOracle.Common.Util;

namespace ZeldaEditor.WinForms {

	public struct TileDisplaySource {

		public IList<BaseTileData> TileList { get; private set; }
		public Tileset Tileset { get; private set; }

		public static implicit operator TileDisplaySource(List<BaseTileData> tileList) {
			return new TileDisplaySource() {
				TileList = tileList,
				Tileset = null,
			};
		}

		public static implicit operator TileDisplaySource(Tileset tileset) {
			return new TileDisplaySource() {
				TileList = null,
				Tileset = tileset,
			};
		}

		public bool IsTileset {
			get { return (Tileset != null); }
		}

		public bool IsList {
			get { return (TileList != null); }
		}
	}

	public class TilesetDisplay : GraphicsDeviceControl {
		
		private EditorWindow editorWindow;
		private EditorControl editorControl;
		private StoppableTimer dispatcherTimer;
		private TileDisplaySource source;
		private Zone zone;
		private int spacing;
		private int columns;
		private BaseTileData selectedTileData;
		private BaseTileData hoverTileData;
		protected Point2I hoverPoint;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			zone = null;
			hoverTileData = null;
			hoverPoint = -Point2I.One;
			selectedTileData = null;
			columns = 1;
			spacing = 1;

			// Setup the event handlers
			MouseMove	+= OnMouseMove;
			MouseDown	+= OnMouseDown;
			MouseLeave	+= OnMouseLeave;
			PostReset	+= OnPostReset;

			ResizeRedraw = true;

			// Start the timer to refresh the panel
			dispatcherTimer = StoppableTimer.StartNew(
				TimeSpan.FromMilliseconds(15),
				DispatcherPriority.Render,
				delegate {
					if (editorControl.IsActive)
						TimerUpdate();
				});
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public event EventHandler HoverChanged;
		public event EventHandler SelectionChanged;


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		//public Point2I GetTileCoord(Point2I point) {
		//	return ((point - Point2I.One) / (GameSettings.TILE_SIZE + spacing));
		//}

		//public Point2I GetTileLocationInPalette(BaseTileData tileData) {
		//	if (source.IsList) {
		//		int index = source.TileList.IndexOf(tileData);
		//		if (index >= 0)
		//			return new Point2I(index % columns, index / columns);
		//	}
		//	else if (source.IsTileset) {
		//		for (int x = 0; x < source.Tileset.Width; x++) {
		//			for (int y = 0; y < source.Tileset.Height; y++) {
		//				BaseTileData checkTileData = source.Tileset.GetTileData(x, y);
		//				if (source.Tileset.GetTileData(x, y) == tileData)
		//					return new Point2I(x, y);
		//			}
		//		}
		//	}
		//	return -Point2I.One;
		//}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void TimerUpdate() {
			if (source.IsList && ClientSize.Width != AutoScrollMinSize.Width) {
				UpdateSize();
			}
			else if (editorControl.PlayAnimations)
				Invalidate();
		}

		private void OnPostReset(object sender, EventArgs e) {
			if (hoverTileData != null) {
				selectedTileData = hoverTileData;
				Invalidate();
			}
			ScrollPosition = Point2I.Zero;
		}

		private void OnMouseDown(object sender, MouseEventArgs e) {
			if (hoverTileData != null)
				SelectedTileData = hoverTileData;
			Focus();
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			UpdateHoverLocation(ScrollPosition + new Point2I(e.X, e.Y));
		}

		private void OnMouseLeave(object sender, EventArgs e) {
			UpdateHoverLocation(null);
		}

		private void UpdateSize() {
			Point2I size = Point2I.One;
			if (source.IsList) {
				// Calculate the number of columns that would fit
				columns = Math.Max(1, (ClientSize.Width - 1) /
					(GameSettings.TILE_SIZE + spacing));
				size.X = ClientSize.Width;
				size.Y = 1 + ((source.TileList.Count + columns - 1) / columns) *
					(GameSettings.TILE_SIZE + spacing);
			}
			else if (source.IsTileset) {
				size = source.Tileset.Dimensions *
					(GameSettings.TILE_SIZE + spacing) + Point2I.One;
			}
			AutoScrollMinSize = new System.Drawing.Size(size.X, size.Y);
			UpdateHoverLocation(ScrollPosition + GdiCasting.ToPoint2I(MousePosition));
			Invalidate();
		}

		private void UpdateHoverLocation(Point2I? mouse) {
			// Get the mouse location and tile data under it
			
			Point2I point = -Point2I.One;
			BaseTileData tileData = null;

			if (mouse.HasValue) {
				point = mouse.Value / (GameSettings.TILE_SIZE + spacing);
				tileData = GetTileDataAtLocation(point);
				if (tileData == null)
					point = -Point2I.One;
			}

			if (hoverPoint != point || hoverTileData != tileData) {
				hoverPoint = point;
				hoverTileData = tileData;
				HoverChanged?.Invoke(this, EventArgs.Empty);
				Invalidate();
			}
		}

		/// <summary>Return the tile data placed at the given tile location in the
		/// tileset display.</summary>
		private BaseTileData GetTileDataAtLocation(Point2I location) {
			if (location < Point2I.Zero) {
				return null;
			}
			else if (source.IsList) {
				int index = (location.Y * columns) + location.X;
				if (index >= 0 && index < source.TileList.Count)
					return source.TileList[index];
				else
					return null;
			}
			else if (source.IsTileset && location < source.Tileset.Dimensions) {
				Point2I origin = source.Tileset.GetTileDataOrigin(location);
				if (origin != -Point2I.One)
					return source.Tileset.GetTileDataAtOrigin(origin);
				else
					return null;
			}
			else
				return null;
		}


		//-----------------------------------------------------------------------------
		// Overriden Methods
		//-----------------------------------------------------------------------------

		protected override void Draw() {
			if (!Resources.IsInitialized || !editorControl.IsInitialized)
				return;
			editorControl.UpdateTicks();

			Graphics2D g = new Graphics2D();
			GameData.PaletteShader.TilePalette = Zone.Palette;
			GameData.PaletteShader.ApplyPalettes();
			TileDataDrawing.RewardManager = editorControl.RewardManager;
			TileDataDrawing.Level = editorControl.Level;
			TileDataDrawing.Room = null;
			TileDataDrawing.Extras = false;
			TileDataDrawing.PlaybackTime = editorControl.Ticks;
			
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.Clear(Color.White);
			g.PushTranslation(-ScrollPosition);

			Point2I selectedPoint = -Point2I.One;
			
			if (source.IsList) {
				// Draw the list of tiles
				int startRow = (ScrollPosition.Y + 1) / (GameSettings.TILE_SIZE + spacing);
				int startIndex = startRow * columns;
				int endRow = (ScrollPosition.Y + ClientSize.Height + 1 +
					GameSettings.TILE_SIZE) / (GameSettings.TILE_SIZE + spacing);
				int endIndex = (endRow + 1) * columns;
				for (int i = startIndex; i < endIndex &&
					i < source.TileList.Count; i++)
				{
					BaseTileData tile = source.TileList[i];
					int row = i / columns;
					int column = i % columns;
					int x = 1 + column * (GameSettings.TILE_SIZE + spacing);
					int y = 1 + row * (GameSettings.TILE_SIZE + spacing);
					
					if (tile == selectedTileData)
						selectedPoint = new Point2I(column, row);

					try {
						TileDataDrawing.DrawTilePreview(
							g, tile, new Point2I(x, y), Zone);
					}
					catch (Exception) {
					}
				}
			}
			else if (source.IsTileset) {
				// Draw the selected tileset
				for (int indexX = 0; indexX < source.Tileset.Width; indexX++) {
					for (int indexY = 0; indexY < source.Tileset.Height; indexY++) {
						BaseTileData tile = source.Tileset.GetTileDataAtOrigin(indexX, indexY);
						if (tile != null) {
							int x = 1 + indexX * (GameSettings.TILE_SIZE + spacing);
							int y = 1 + indexY * (GameSettings.TILE_SIZE + spacing);
							
							if (tile == selectedTileData)
								selectedPoint = new Point2I(indexX, indexY);

							try {
								if (source.Tileset.UsePreviewSprites)
									TileDataDrawing.DrawTilePreview(
										g, tile, new Point2I(x, y), Zone);
								else
									TileDataDrawing.DrawTile(
										g, tile, new Point2I(x, y), Zone);
							}
							catch (Exception) {
							}
						}
					}
				}
			}

			// Draw a box around the selected tile
			if (selectedPoint != -Point2I.One) {
				Point2I selectedSize = Point2I.One;
				if (selectedTileData != null)
					selectedSize = selectedTileData.Size;
				if (source.IsTileset && source.Tileset.UsePreviewSprites)
					selectedSize = Point2I.One;
				Rectangle2I selectRect = new Rectangle2I(
					selectedPoint * (GameSettings.TILE_SIZE + spacing),
					selectedSize * GameSettings.TILE_SIZE + 2);
				g.DrawRectangle(selectRect, 1, Color.Black);
				g.DrawRectangle(selectRect.Inflated(1, 1), 1, Color.White);
				g.DrawRectangle(selectRect.Inflated(2, 2), 1, Color.Black);
			}

			// Draw a translucent box around the tile under the mouse cursor
			if (hoverPoint != -Point2I.One) {
				Point2I size = Point2I.One;
				if (hoverTileData != null)
					size = hoverTileData.Size;
				if (source.IsTileset && source.Tileset.UsePreviewSprites)
					size = Point2I.One;
				Rectangle2I selectRect = new Rectangle2I(
					hoverPoint * (GameSettings.TILE_SIZE + spacing),
					size * GameSettings.TILE_SIZE + 2);
				g.DrawRectangle(selectRect, 1, Color.Black);
				g.DrawRectangle(selectRect.Inflated(1, 1), 1, Color.White);
				g.DrawRectangle(selectRect.Inflated(2, 2), 1, Color.Black);
			}

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

		public Zone Zone {
			get { return (zone ?? GameData.ZONE_DEFAULT); }
			set {
				if (zone != value) {
					zone = value;
					Invalidate();
				}
			}
		}

		public BaseTileData SelectedTileData {
			get { return selectedTileData; }
			set {
				if (selectedTileData != value) {
					selectedTileData = value;
					SelectionChanged?.Invoke(this, EventArgs.Empty);
					Invalidate();
				}
			}
		}

		public BaseTileData HoverTileData {
			get { return hoverTileData; }
		}

		public TileDisplaySource TilesSource {
			get { return source; }
			set {
				source = value;
				UpdateSize();
			}
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

		public int Spacing {
			get { return spacing; }
			set {
				if (spacing != value) {
					spacing = value;
					UpdateSize();
				}
			}
		}
	}
}


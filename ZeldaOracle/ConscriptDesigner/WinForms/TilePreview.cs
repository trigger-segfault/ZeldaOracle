using System;
using System.Collections.Generic;
using Size = System.Drawing.Size;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game;
using System.Windows.Threading;
using System.Diagnostics;
using ZeldaOracle.Game.Tiles;
using ConscriptDesigner.Control;
using ZeldaOracle.Game.Worlds;

namespace ConscriptDesigner.WinForms {
	public class TilePreview : ZeldaGraphicsDeviceControl {

		private List<BaseTileData> tileData;
		private Tileset tileset;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilePreview() {
			this.tileData = new List<BaseTileData>();
			this.tileset = null;
			this.MouseDown += OnMouseDown;
		}

		protected override void Initialize() {
			base.Initialize();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (HoverTileData != null) {
				if (!TileListMode) {
					DesignerControl.SelectedTileLocation = tileset.GetTileDataOrigin(hoverPoint);
					DesignerControl.SelectedTileset = tileset;
				}
				DesignerControl.SelectedTileData = HoverTileData;
				Invalidate();
			}
		}


		//-----------------------------------------------------------------------------
		// Loading/Updating
		//-----------------------------------------------------------------------------

		public void UpdateList(List<BaseTileData> tileData) {
			this.tileData = tileData;
			this.tileset = null;
			UpdateHeight();
		}

		public void UpdateList(Tileset tileset) {
			this.tileset = tileset;
			this.tileData = null;
			UpdateHeight();
		}

		public void UpdateScale() {
			UpdateHeight();
		}

		public void Unload() {
			tileData.Clear();
			tileset = null;
			UpdateSize(Point2I.One);
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		protected override void TimerUpdate() {
			base.TimerUpdate();
			if (TileListMode && UnscaledClientSize.X * DesignerControl.PreviewScale != AutoScrollMinSize.Width)
				UpdateHeight();
		}

		protected override bool IsValidHoverPoint(ref Point2I point, out Point2I hoverSize) {
			hoverSize = Point2I.One;
			if (TileListMode) {
				int index = (point.Y * columns) + point.X;
				return index < tileData.Count;
			}
			else if (tileset != null && point < tileset.Dimensions) {
				Point2I origin = tileset.GetTileDataOrigin(point);
				if (origin != -Point2I.One) {
					if (!tileset.UsePreviewSprites)
						hoverSize = tileset.GetTileDataAtOrigin(origin).Size;
					point = origin;
				}
				return true;
			}
			return false;
		}

		protected override void UpdateHeight() {
			if (TileListMode) {
				columns = Math.Max(1, (UnscaledClientSize.X - 1) / (BaseSpriteSize.X + 1));
				int height = 1 + ((tileData.Count + columns - 1) / columns) * (BaseSpriteSize.Y + 1);

				UpdateSize(new Point2I(UnscaledClientSize.X, height));
			}
			else if (tileset != null) {
				columns = tileset.Width;
				UpdateSize(tileset.Dimensions * (BaseSpriteSize + 1) + 1);
			}
			else {
				columns = 1;
				UpdateSize(Point2I.One);
			}
		}

		protected override void Draw(Graphics2D g, SpriteDrawSettings settings, Zone zone) {
			TileDataDrawing.Extras = false;
			TileDataDrawing.Level = null;
			TileDataDrawing.PlaybackTime = DesignerControl.PlaybackTime;
			TileDataDrawing.RewardManager = DesignerControl.RewardManager;

			Point2I selectionPoint = -Point2I.One;

			if (TileListMode) {
				int startRow = (UnscaledScrollPosition.Y + 1) / (GameSettings.TILE_SIZE + 1);
				int startIndex = startRow * columns;
				int endRow = (UnscaledScrollPosition.Y + ClientSize.Height + 1 + GameSettings.TILE_SIZE) / (GameSettings.TILE_SIZE + 1);
				int endIndex = (endRow + 1) * columns;
				for (int i = startIndex; i < endIndex && i < tileData.Count; i++) {
					BaseTileData tile = tileData[i];
					int row = i / columns;
					int column = i % columns;
					int x = 1 + column * (GameSettings.TILE_SIZE + 1);
					int y = 1 + row * (GameSettings.TILE_SIZE + 1);
					
					if (tile == DesignerControl.SelectedTileData)
						selectionPoint = new Point2I(column, row);

					try {
						TileDataDrawing.DrawTilePreview(g, tile, new Point2I(x, y), zone);
					}
					catch (Exception) {

					}
				}
			}
			else if (tileset != null) {
				for (int indexX = 0; indexX < tileset.Width; indexX++) {
					for (int indexY = 0; indexY < tileset.Height; indexY++) {
						BaseTileData tile = tileset.GetTileDataAtOrigin(indexX, indexY);
						if (tile != null) {
							int x = 1 + indexX * (BaseSpriteSize.X + 1);
							int y = 1 + indexY * (BaseSpriteSize.Y + 1);

							try {
								if (tileset.UsePreviewSprites)
									TileDataDrawing.DrawTilePreview(g, tile, new Point2I(x, y), zone);
								else
									TileDataDrawing.DrawTile(g, tile, new Point2I(x, y), zone);
							}
							catch (Exception) {

							}
						}
					}
				}
				if (DesignerControl.SelectedTileLocation >= Point2I.Zero &&
					DesignerControl.SelectedTileset == tileset)
				{
					selectionPoint = DesignerControl.SelectedTileLocation;
				}
			}

			if (selectionPoint != -Point2I.One) {
				Point2I selectedSize = DesignerControl.SelectedTileData.Size;
				if (tileset == null || tileset.UsePreviewSprites)
					selectedSize = Point2I.One;
				Rectangle2I selectRect = new Rectangle2I(
					selectionPoint * (GameSettings.TILE_SIZE + 1),
					selectedSize * GameSettings.TILE_SIZE + 2);
				g.DrawRectangle(selectRect, 1, Color.Black);
				g.DrawRectangle(selectRect.Inflated(1, 1), 1, Color.White);
				g.DrawRectangle(selectRect.Inflated(2, 2), 1, Color.Black);
			}
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		protected override Point2I BaseSpriteSize {
			get { return new Point2I(GameSettings.TILE_SIZE); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool TileListMode {
			get { return tileset == null; }
		}

		public BaseTileData HoverTileData {
			get {
				if (TileListMode) {
					if (hoverPoint == -Point2I.One || tileData == null)
						return null;
					int index = (hoverPoint.Y * columns) + hoverPoint.X;
					return tileData[index];
				}
				else {
					if (hoverPoint == -Point2I.One)
						return null;
					return tileset.GetTileData(hoverPoint.X, hoverPoint.Y);
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Tiles;
using ConscriptDesigner.Control;
using ZeldaOracle.Game.Worlds;

namespace ConscriptDesigner.WinForms {
	public class TileDataPreview : ZeldaGraphicsDeviceControl {

		private List<BaseTileData> tileData;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileDataPreview() {
			this.tileData = new List<BaseTileData>();
		}

		protected override void Initialize() {
			base.Initialize();
		}


		//-----------------------------------------------------------------------------
		// Loading/Updating
		//-----------------------------------------------------------------------------

		public void UpdateList(List<BaseTileData> tileData) {
			this.tileData = tileData;
			UpdateHeight();
		}

		public void UpdateScale() {
			UpdateHeight();
		}

		public void Unload() {
			tileData.Clear();
			UpdateSize(Point2I.One);
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		protected override void TimerUpdate() {
			base.TimerUpdate();
			if (UnscaledClientSize.X * DesignerControl.PreviewScale != AutoScrollMinSize.Width)
				UpdateHeight();
		}

		protected override bool IsValidHoverPoint(ref Point2I point, out Point2I hoverSize) {
			int index = (point.Y * columns) + point.X;
			hoverSize = Point2I.One;
			return index < tileData.Count;
		}

		protected override void UpdateHeight() {
			columns = Math.Max(1, (UnscaledClientSize.X - 1) / (BaseSpriteSize.X + 1));
			int height = 1 + ((tileData.Count + columns - 1) / columns) * (BaseSpriteSize.Y + 1);

			UpdateSize(new Point2I(UnscaledClientSize.X, height));
		}

		protected override void Draw(Graphics2D g, SpriteSettings settings, Zone zone) {
			TileDataDrawing.Extras = false;
			TileDataDrawing.Level = null;
			TileDataDrawing.PlaybackTime = DesignerControl.PlaybackTime;
			TileDataDrawing.RewardManager = DesignerControl.RewardManager;
			
			int startRow = (UnscaledScrollPosition.Y + 1) / (GameSettings.TILE_SIZE + 1);
			int startIndex = startRow * columns;
			int endRow = (UnscaledScrollPosition.Y + ClientHeight + 1 + GameSettings.TILE_SIZE) / (GameSettings.TILE_SIZE + 1);
			int endIndex = (endRow + 1) * columns;
			for (int i = startIndex; i < endIndex && i < tileData.Count; i++) {
				BaseTileData tile = tileData[i];
				int row = i / columns;
				int column = i % columns;
				int x = 1 + column * (GameSettings.TILE_SIZE + 1);
				int y = 1 + row * (GameSettings.TILE_SIZE + 1);

				try {
					TileDataDrawing.DrawTilePreview(g, tile, new Point2I(x, y), zone);
				}
				catch (Exception) {

				}
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
		
		public BaseTileData HoverTileData {
			get {
				if (hoverPoint == -Point2I.One || tileData == null)
					return null;
				int index = (hoverPoint.Y * columns) + hoverPoint.X;
				return tileData[index];
			}
		}
	}
}

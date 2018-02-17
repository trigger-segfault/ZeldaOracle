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
	public class TilesetPreview : ZeldaGraphicsDeviceControl {

		private Tileset tileset;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilesetPreview() {
			this.tileset = null;
		}

		protected override void Initialize() {
			base.Initialize();
		}


		//-----------------------------------------------------------------------------
		// Loading/Updating
		//-----------------------------------------------------------------------------

		public void UpdateList(Tileset tileset) {
			this.tileset = tileset;
			UpdateHeight();
		}

		public void UpdateScale() {
			UpdateHeight();
		}

		public void Unload() {
			tileset = null;
			UpdateSize(Point2I.One);
		}

		
		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		protected override void TimerUpdate() {
			base.TimerUpdate();
		}

		protected override void UpdateHeight() {
			if (tileset != null) {
				columns = tileset.Width;
				UpdateSize(tileset.Dimensions * (BaseSpriteSize + 1) + 1);
			}
			else {
				columns = 1;
				UpdateSize(Point2I.One);
			}
		}

		protected override bool IsValidHoverPoint(ref Point2I point, out Point2I hoverSize) {
			hoverSize = Point2I.One;
			if (tileset != null && point < tileset.Dimensions) {
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

		protected override void Draw(Graphics2D g, SpriteSettings settings, Zone zone) {
			if (tileset == null)
				return;

			TileDataDrawing.Extras = false;
			TileDataDrawing.Level = null;
			TileDataDrawing.PlaybackTime = DesignerControl.PlaybackTime;
			TileDataDrawing.RewardManager = DesignerControl.RewardManager;

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
				if (hoverPoint == -Point2I.One)
					return null;
				return tileset.GetTileData(hoverPoint.X, hoverPoint.Y);
			}
		}
	}
}

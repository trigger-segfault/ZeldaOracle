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
using ZeldaOracle.Common.Graphics.Sprites;
using System.Windows.Threading;
using Size = System.Drawing.Size;
using ZeldaWpf.WinForms;

namespace ZeldaEditor.WinForms {

	public class TilePreview : GraphicsDeviceControl {
		
		private BaseTileDataInstance tile;
		private Color background;
		private bool initialized;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TilePreview() {
			initialized = false;
			tile = null;
			background = Color.White;
		}

		protected override void Initialize() {
			initialized = true;
			ResizeRedraw = true;
			ClientSize = new Size(16, 16);
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		protected override void Draw() {
			if (!Resources.IsInitialized) return;

			Graphics2D g = new Graphics2D();
			g.Clear(background);
			if (tile == null)
				return;

			GameData.SHADER_PALETTE.TilePalette = tile.Room.Zone.Palette;
			GameData.SHADER_PALETTE.ApplyParameters();
			TileDataDrawing.Level = tile.Room.Level;
			TileDataDrawing.Room = tile.Room;
			TileDataDrawing.Extras = false;
			TileDataDrawing.PlaybackTime = 0f;

			g.Begin(GameSettings.DRAW_MODE_PALLETE);

			TileDataDrawing.DrawTilePreview(g, tile, Point2I.Zero, tile.Room.Zone);

			g.End();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets or sets the background color of the preview.</summary>
		public Color Background {
			get { return background; }
			set {
				if (value != background) {
					background = value;
					if (initialized)
						Invalidate();
				}
			}
		}

		/// <summary>Gets or sets the tile being drawn.</summary>
		public BaseTileDataInstance Tile {
			get { return tile; }
			set {
				if (value != tile) {
					tile = value;
					if (initialized)
						Invalidate();
				}
			}
		}
	}
}


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

namespace ZeldaEditor.WinForms {

	public class TilePreview : GraphicsDeviceControl {
		
		private EditorControl editorControl;

		private BaseTileDataInstance tile;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			this.ResizeRedraw = true;
			tile = null;

			ClientSize = new Size(16, 16);
		}
		

		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------
		
		public void UpdateTile(BaseTileDataInstance tile) {
			if (this.tile != tile) {
				this.tile = tile;
				Invalidate();
			}
		}


		//-----------------------------------------------------------------------------
		// Overriden methods
		//-----------------------------------------------------------------------------

		protected override void Draw() {
			if (!Resources.IsInitialized) return;
			if (!editorControl.IsInitialized)
				return;
			Graphics2D g = new Graphics2D();
			g.Clear(Color.White);
			if (tile == null)
				return;

			GameData.SHADER_PALETTE.TilePalette = tile.Room.Zone.Palette;
			GameData.SHADER_PALETTE.ApplyParameters();
			TileDataDrawing.RewardManager = editorControl.RewardManager;
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

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}
	}
}


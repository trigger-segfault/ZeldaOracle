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

namespace ZeldaEditor.WinForms {

	public class SpritePreview : GraphicsDeviceControl {

		private static SpriteBatch spriteBatch;

		private EditorControl editorControl;

		private ISprite sprite;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public SpritePreview(ISprite sprite) {
			this.sprite = sprite;
		}

		protected override void Initialize() {
			spriteBatch = new SpriteBatch(GraphicsDevice);

			this.ResizeRedraw = true;

			ClientSize = new Size(16, 16);
		}


		//-----------------------------------------------------------------------------
		// Overriden methods
		//-----------------------------------------------------------------------------

		protected override void Draw() {
			Graphics2D g = new Graphics2D(spriteBatch);
			g.Clear(Color.White);
			if (sprite == null)
				return;

			GameData.PaletteShader.TilePalette = GameData.PAL_TILES_DEFAULT;
			GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
			GameData.PaletteShader.ApplyPalettes();

			g.Begin(GameSettings.DRAW_MODE_DEFAULT);

			g.DrawSprite(sprite, Point2I.Zero);

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


using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game;
using ZeldaWpf.WinForms;
using ZeldaEditor.Control;

namespace ZeldaEditor.WinForms {

	public class SpritePreview : GraphicsDeviceControl {

		private EditorControl editorControl;

		private ISprite sprite;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public SpritePreview(ISprite sprite) {
			this.sprite = sprite;
		}

		protected override void Initialize() {

			this.ResizeRedraw = true;

			ClientSize = new Point2I(16, 16);
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
			if (sprite == null)
				return;

			GameData.SHADER_PALETTE.TilePalette = GameData.PAL_TILES_DEFAULT;
			GameData.SHADER_PALETTE.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
			GameData.SHADER_PALETTE.ApplyParameters();

			g.Begin(GameSettings.DRAW_MODE_PALLETE);

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


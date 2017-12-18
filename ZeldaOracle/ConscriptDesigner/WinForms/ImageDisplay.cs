using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Content;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;

namespace ConscriptDesigner.WinForms {
	public class ImageDisplay : GraphicsDeviceControl {

		//private ContentManager contentManager;
		private SpriteBatch spriteBatch;
		private Texture2D texture;
		private ContentFile file;

		public ImageDisplay(ContentFile file) {
			this.file = file;

		}

		protected override void Initialize() {
			//contentManager = new ContentManager(Services, "Content");
			spriteBatch = new SpriteBatch(GraphicsDevice);
			TextureLoader loader = new TextureLoader(GraphicsDevice);
			texture = loader.FromFile(file.FilePath);// contentManager.Load<Texture2D>(Path.ChangeExtension(file.Path, ""));
		}

		protected override void Draw() {
			Graphics2D g = new Graphics2D(spriteBatch);
			g.Clear(Color.White);
			g.Begin(GameSettings.DRAW_MODE_BASIC);
			g.Translate(-HorizontalScroll.Value, -VerticalScroll.Value);
			g.DrawImage(texture, Vector2F.Zero);
			g.End();
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			texture.Dispose();
			//contentManager.Dispose();
		}
	}
}

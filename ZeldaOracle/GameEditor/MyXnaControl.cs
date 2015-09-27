using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsGraphicsDevice;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
/*using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;*/

namespace ZeldaEditor {
	public class MyXnaControl : GraphicsDeviceControl {

		static bool initialized = false;

		static ContentManager content;
		static Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;
		static Microsoft.Xna.Framework.Graphics.Texture2D texture;


		protected override void Initialize() {
			if (!DesignMode && !initialized) {
				content = new ContentManager(Services, "Content");

				spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

				Resources.Initialize(content, GraphicsDevice);
				GameData.Initialize();
				initialized = true;
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				content.Unload();
			}

			base.Dispose(disposing);
		}


		protected override void Draw() {

			GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
			Graphics2D g = new Graphics2D(spriteBatch);

			//g.SetRenderTarget(GameData.RenderTargetGame);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.Clear(Color.Black);
			g.DrawImage(Resources.GetImage("UI/menu_weapons_a"), Point2I.Zero);
			//spriteBatch.Draw(texture, Microsoft.Xna.Framework.Vector2.Zero, Microsoft.Xna.Framework.Color.White);
			//spriteBatch.End();
			g.End();
		}
	}
}

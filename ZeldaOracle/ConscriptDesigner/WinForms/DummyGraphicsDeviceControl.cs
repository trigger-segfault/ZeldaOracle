using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Control;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ConscriptDesigner.WinForms {
	public class DummyGraphicsDeviceControl : GraphicsDeviceControl {


		protected override void Initialize() {
			SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
			DesignerControl.SetGraphics(
				spriteBatch,
				GraphicsDevice,
				new ContentManager(Services, "Content"));
		}
	}
}

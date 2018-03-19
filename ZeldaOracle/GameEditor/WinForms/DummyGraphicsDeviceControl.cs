using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaEditor.Control;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;

namespace ZeldaEditor.WinForms {
	public class DummyGraphicsDeviceControl : GraphicsDeviceControl {

		protected override void Initialize() {
			Resources.Initialize(GraphicsDevice, Services);
		}
	}
}

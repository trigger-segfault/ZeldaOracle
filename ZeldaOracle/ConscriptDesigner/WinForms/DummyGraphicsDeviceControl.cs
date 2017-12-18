using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Control;
using Microsoft.Xna.Framework.Content;

namespace ConscriptDesigner.WinForms {
	public class DummyGraphicsDeviceControl : GraphicsDeviceControl {


		protected override void Initialize() {
			DesignerControl.GraphicsDevice	= GraphicsDevice;
			DesignerControl.ContentManager	= new ContentManager(Services, "Content");
		}
	}
}

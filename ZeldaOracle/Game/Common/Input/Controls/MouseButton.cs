using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using XnaMouse	= Microsoft.Xna.Framework.Input.Mouse;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Input;
using Mouse		= ZeldaOracle.Common.Input.Mouse;

namespace ZeldaOracle.Common.Input.Controls {
	public class MouseButton : ControlHandler {

		public MouseButtons ButtonCode;
		public bool DoubleClick;

		public MouseButton(MouseButtons buttonCode, bool doubleClick = false) {
			this.ButtonCode		= buttonCode;
			this.DoubleClick	= doubleClick;
		}

		public override bool Pressed() {
			return Mouse.IsButtonPressed(ButtonCode);
		}
		public override bool Released() {
			return Mouse.IsButtonReleased(ButtonCode);
		}
		public override bool Down() {
			return Mouse.IsButtonDown(ButtonCode);
		}
		public override bool Up() {
			return Mouse.IsButtonUp(ButtonCode);
		}

		public override string Name {
			get { return Mouse.GetButtonName(ButtonCode) + " Double Click"; }
		}
	}
}

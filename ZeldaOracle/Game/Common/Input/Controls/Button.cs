using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using XnaGamePad	= Microsoft.Xna.Framework.Input.GamePad;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Input;
using GamePad		= ZeldaOracle.Common.Input.GamePad;

namespace ZeldaOracle.Common.Input.Controls {
public class Button : Control {

	public Buttons ButtonCode;
	public int Player;

	public Button(Buttons buttonCode, int player = 0) {
		this.ButtonCode		= buttonCode;
		this.Player			= player;
	}

	public override bool Pressed() {
		return GamePad.IsButtonPressed(ButtonCode, Player);
	}
	public override bool Released() {
		return GamePad.IsButtonReleased(ButtonCode, Player);
	}
	public override bool Down() {
		return GamePad.IsButtonDown(ButtonCode, Player);
	}
	public override bool Up() {
		return GamePad.IsButtonUp(ButtonCode, Player);
	}

	public override string Name {
		get { return GamePad.GetButtonName(ButtonCode); }
	}
}
}

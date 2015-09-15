using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using XnaKeyboard	= Microsoft.Xna.Framework.Input.Keyboard;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Input.Controls;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;

namespace ZeldaOracle.Common.Debug {
/** <summary>
 * The base controller for game debugging.
 * </summary> */
public class DebugControllerBase {

	//=========== MEMBERS ============
	#region Members

	/** <summary> The debug menu manager. </summary> */
	protected DebugMenu menu;
	/** <summary> True if the game is paused. </summary> */
	protected bool gamePaused;
	/** <summary> The key for opening the debug menu. </summary> */
	private ControlHandler menuKey;
	/** <summary> The mouse button for opening the debug menu. </summary> */
	private ControlHandler menuMouseButton;
	/** <summary> The gamepad button for opening the debug menu. </summary> */
	private ControlHandler menuButton;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the main debug controller. </summary> */
	public DebugControllerBase() {
		this.menu		= new DebugMenu();
		this.gamePaused	= false;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets or sets the key for opening the debug menu. </summary> */
	public ControlHandler DebugMenuKey {
		get { return menuKey; }
		set { menuKey = value; }
	}
	/** <summary> Gets or sets the mouse button for opening the debug menu. </summary> */
	public ControlHandler DebugMenuMouseButton {
		get { return menuMouseButton; }
		set { menuMouseButton = value; }
	}
	/** <summary> Gets or sets the gamepad button for opening the debug menu. </summary> */
	public ControlHandler DebugMenuButton {
		get { return menuButton; }
		set { menuButton = value; }
	}
	/** <summary> Gets or sets the font for the debug menu. </summary> */
	public RealFont DebugMenuFont {
		get { return menu.debugMenuFont; }
		set { menu.debugMenuFont = value; }
	}
	/** <summary> Gets or sets the bold font for the debug menu. </summary> */
	public RealFont DebugMenuFontBold {
		get { return menu.debugMenuFontBold; }
		set { menu.debugMenuFontBold = value; }
	}
	/** <summary> Gets or sets the sprite sheet for the debug menu. </summary> */
	/*public SpriteAtlas DebugMenuSprites {
		get { return menu.debugMenuSprites; }
		set { menu.debugMenuSprites = value; }
	}*/
	/** <summary> Returns true if the game is paused. </summary> */
	public bool IsGamePaused {
		get { return gamePaused; }
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the debug controller. </summary> */
	public virtual void Update() {

		if (menuKey.Pressed() || menuMouseButton.Pressed() || menuButton.Pressed()) {
			if (!menu.IsOpen) {
				menu.Open();
			}
			else {
				menu.Close();
			}
		}
		else if (menu.IsOpen) {
			menu.Update();
		}

		menu.CheckHotkeys();
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw debug information. </summary> */
	public virtual void Draw(Graphics2D g) {
		if (menu.IsOpen) {
			menu.Draw(g);
		}
	}

	#endregion
}
} // end namespace

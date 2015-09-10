using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Keys			= Microsoft.Xna.Framework.Input.Keys;

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using Color			= ZeldaOracle.Common.Graphics.Color;
using GamePad		= ZeldaOracle.Common.Input.GamePad;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;
using Mouse			= ZeldaOracle.Common.Input.Mouse;
using Buttons		= ZeldaOracle.Common.Input.Buttons;
using MouseButtons	= ZeldaOracle.Common.Input.MouseButtons;

namespace GameFramework.MyGame.Main {
/** <summary>
 * The class that contains updatable objects in a menu.
 * </summary> */
public class Menu : Room {
	
	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Management
	/** <summary> True if the menu has been closed. </summary> */
	protected bool closed;
	/** <summary> True if the game should be paused while this menu is open. </summary> */
	protected bool pauseGame;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default menu. </summary> */
	public Menu(string id = "")
		: base(1, 1, id) {

		// Management
		this.closed			= false;
		this.pauseGame		= true;
	}
	/** <summary> Initializes the menu. </summary> */
	public override void Initialize(GameManager game) {
		base.Initialize(game);

		// NOTE: Override this function and call base first
	}
	/** <summary> Uninitializes the menu. </summary> */
	public override void Uninitialize() {
		base.Uninitialize();

		// NOTE: Override this function and call base last
	}
	/** <summary> Called to load game manager content. </summary> */
	public override void LoadContent(ContentManager content) {
		base.LoadContent(content);

		// NOTE: Override this function and call base first
	}
	/** <summary> Called to unload game manager content. </summary> */
	public override void UnloadContent(ContentManager content) {
		base.UnloadContent(content);

		// NOTE: Override this function and call base last
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Information

	/** <summary> Returns true if the menu has been closed. </summary> */
	public bool IsClosed {
		get { return closed; }
	}
	/** <summary> Returns true if the game should be paused while this menu is open. </summary> */
	public bool PauseGame {
		get { return pauseGame; }
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the menu. </summary> */
	public override void Update() {
		base.Update();

		// NOTE: Override this function and call base
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the room and its objects. </summary> */
	public override void Draw(Graphics2D g) {
		base.Draw(g);

		// NOTE: Override this function and call base
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Called when the menu becomes active. </summary> */
	public override void EnterRoom() {
		base.EnterRoom();

		// NOTE: Override this function and call base
	}
	/** <summary> Called when the menu is no longer active. </summary> */
	public override void LeaveRoom() {
		base.LeaveRoom();

		// NOTE: Override this function and call base
	}

	#endregion
}
} // End namespace

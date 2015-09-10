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
using ZeldaOracle.Game.Worlds;

namespace GameFramework.MyGame.Main {
/** <summary>
 * The interface for all objects in rooms.
 * </summary> */
public interface IGameObject {

	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Initializes the object and sets up containment variables. </summary> */
	void Initialize(Room room);
	/** <summary> Uninitializes the object and removes all containment variables. </summary> */
	void Uninitialize();

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the class managing the XNA framework. </summary> */
	//GameBase GameBase { get; }
	/** <summary> Gets the class managing the game. </summary> */
	//GameManager Game { get; }
	/** <summary> Gets the room that contains the object. </summary> */
	Room Room { get; }

	#endregion
	//--------------------------------
	#region Information

	/** <summary> Gets or sets the string identifier for the object. </summary> */
	string ID { get; set; }
	/** <summary> Returns true if the object has been destroyed. </summary> */
	bool IsDestroyed { get; }
	/** <summary> Returns true if the object has been initialized. </summary> */
	bool IsInitialized { get; }
	/** <summary> Gets the position of the object. </summary> */
	Vector2F ObjectPosition { get; }

	#endregion
	//--------------------------------
	#region Visual

	/** <summary> Gets or sets the draw depth of the object. </summary> */
	double Depth { get; set; }
	/** <summary> Gets or sets if the object is visible. </summary> */
	bool IsVisible { get; set; }
	/** <summary> Gets or sets if the object is enabled. </summary> */
	bool IsEnabled { get; set; }
	/** <summary> Gets or sets if the object is updatable. </summary> */
	bool IsUpdatable { get; set; }

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the object. </summary> */
	void Preupdate();
	/** <summary> Called every step to update the object. </summary> */
	void Update();
	/** <summary> Called every step to update the object. </summary> */
	void Postupdate();

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the object. </summary> */
	void Draw(Graphics2D g);

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Marks the object for removal from the room. </summary> */
	void Destroy();
	/** <summary> Called when the object enters the room. </summary> */
	void EnterRoom();
	/** <summary> Called when the object leaves the room. </summary> */
	void LeaveRoom();

	#endregion
}
} // End namespace

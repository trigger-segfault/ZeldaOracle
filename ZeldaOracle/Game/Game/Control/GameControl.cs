using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Maps;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Debug;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Drops;
using ZeldaOracle.Game.Items.Equipment;
using ZeldaOracle.Game.Items.Essences;
using ZeldaOracle.Game.Items.KeyItems;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {

	// The main control for the current game session.
	public class GameControl : ZeldaAPI.Game {

		private GameManager		gameManager;
		private RoomControl		roomControl;
		private World			world;
		private Player			player;
		private HUD				hud;
		private Inventory		inventory;
		private RewardManager	rewardManager;
		private DropManager		dropManager;
		private RoomStateStack	roomStateStack;
		private bool			isAdvancedGame;
		private int				roomTicks; // The total number of ticks elapsed since the game was started (used for animation).
		private bool			updateRoom;
		private bool			animateRoom;
		private ScriptRunner	scriptRunner;
		private Room			lastRoomOnMap;

		// Menus
		private MenuWeapons			menuWeapons;
		private MenuSecondaryItems	menuSecondaryItems;
		private MenuEssences		menuEssences;
		private ScreenDungeonMap	mapDungeon;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public GameControl(GameManager gameManager) {
			this.gameManager		= gameManager;
			this.roomStateStack		= null;
			this.roomControl		= null;
			this.world				= null;
			this.player				= null;
			this.hud				= null;
			this.inventory			= null;
			this.rewardManager		= null;
			this.dropManager		= null;
			this.isAdvancedGame		= false;
			this.updateRoom			= true;
			this.animateRoom		= true;
			this.menuWeapons		= null;
			this.menuSecondaryItems	= null;
			this.menuEssences		= null;
			this.scriptRunner		= null;
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		public void LoadWorld(string fileName, bool recompile) {
			WorldFile worldFile = new WorldFile();
			World world = worldFile.Load(fileName, false);
			LoadWorld(world, recompile);
		}

		public void LoadWorld(World world, bool recompile) {
			this.world = world;

			if (recompile) {
				world.ScriptManager.CompileAndWriteAssembly(null);
			}
			scriptRunner.OnLoadWorld(world);
		}

		// Start a new game.
		public void StartGame() {
			roomTicks = 0;

			// Setup the player beforehand so certain classes such as the HUD can reference it
			player = new Player();

			inventory						= new Inventory(this);
			menuWeapons						= new MenuWeapons(gameManager);
			menuSecondaryItems				= new MenuSecondaryItems(gameManager);
			menuEssences					= new MenuEssences(gameManager);
			menuWeapons.PreviousMenu		= menuEssences;
			menuWeapons.NextMenu			= menuSecondaryItems;
			menuSecondaryItems.PreviousMenu	= menuWeapons;
			menuSecondaryItems.NextMenu		= menuEssences;
			menuEssences.PreviousMenu		= menuSecondaryItems;
			menuEssences.NextMenu			= menuWeapons;

			mapDungeon = new ScreenDungeonMap(gameManager);

			GameData.LoadInventory(inventory, true);

			inventory.ObtainAmmo("ammo_ember_seeds");
			inventory.ObtainAmmo("ammo_scent_seeds");
			inventory.ObtainAmmo("ammo_pegasus_seeds");
			inventory.ObtainAmmo("ammo_gale_seeds");
			inventory.ObtainAmmo("ammo_mystery_seeds");

			hud = new HUD(this);
			hud.DynamicHealth = player.Health;

			rewardManager = new RewardManager(this);
			GameData.LoadRewards(rewardManager);
			dropManager = new DropManager(this);
			GameData.LoadDrops(dropManager, rewardManager);

			// Create the script runner.
			scriptRunner = new ScriptRunner(this);

			// Create the room control.
			roomControl = new RoomControl();
			gameManager.PushGameState(roomControl);
			
			// Load the world.
			//WorldFile worldFile = new WorldFile();
			//world = worldFile.Load("Content/Worlds/temp_world.zwd");

			// Begin the room state.
			if (gameManager.LaunchParameters.Length > 0) {

				int startLevel = 0;
				Point2I startRoom = Point2I.Zero;
				Point2I startPlayer = Point2I.Zero;
				bool test = false;
				bool recompile = true;

				for (int i = 1; i < gameManager.LaunchParameters.Length; i++) {
					if (gameManager.LaunchParameters[i] == "-test") {
						test = true;
						startLevel = Int32.Parse(gameManager.LaunchParameters[i+1]);
						startRoom.X = Int32.Parse(gameManager.LaunchParameters[i+2]);
						startRoom.Y = Int32.Parse(gameManager.LaunchParameters[i+3]);
						startPlayer.X = Int32.Parse(gameManager.LaunchParameters[i+4]);
						startPlayer.Y = Int32.Parse(gameManager.LaunchParameters[i+5]);
					}
					else if (gameManager.LaunchParameters[i] == "-no-compile") {
						recompile = false;
					}
				}
				
				LoadWorld(gameManager.LaunchParameters[0], recompile);

				if (test) {
					player.SetPositionByCenter(startPlayer * GameSettings.TILE_SIZE + new Point2I(8, 8));
					player.MarkRespawn();
					roomControl.BeginRoom(world.GetLevelAt(startLevel).Rooms[startRoom.X, startRoom.Y]);
				}
				else {
					player.SetPositionByCenter(world.StartTileLocation * GameSettings.TILE_SIZE + new Point2I(8, 8));
					player.MarkRespawn();
					roomControl.BeginRoom(world.StartRoom);
				}
			}
			else {
				LoadWorld(GameDebug.CreateTestWorld(), false);
				player.SetPositionByCenter(world.StartTileLocation * GameSettings.TILE_SIZE + new Point2I(8, 8));
				player.MarkRespawn();
				roomControl.BeginRoom(world.StartRoom);
			}
			roomStateStack = new RoomStateStack(new RoomStateNormal());
			roomStateStack.Begin(this);
			
			if (!roomControl.Room.IsHiddenFromMap)
				lastRoomOnMap = roomControl.Room;

			AudioSystem.MasterVolume = 0.04f; // The way David likes it.

			FireEvent(world, "start_game", this);
		}
		

		//-----------------------------------------------------------------------------
		// Scripts
		//-----------------------------------------------------------------------------
		
		public void FireEvent(IEventObject caller, string eventName, params object[] parameters) {
			Event evnt = caller.Events.GetEvent(eventName);
			if (evnt != null)
				ExecuteScript(evnt.InternalScriptID, parameters);
		}

		// Execute a script with the given name.
		public void ExecuteScript(string scriptID, params object[] parameters) {
			if (!string.IsNullOrEmpty(scriptID)) {
				Script script = world.GetScript(scriptID);
				
				if (script != null) {
					ExecuteScript(script, parameters);
				}
				else {
					Console.WriteLine("Error trying to execute non-existent script '" + scriptID + "'");
				}
			}
		}

		// Execute a script.
		public void ExecuteScript(Script script, params object[] parameters) {
			if (script != null) {
				Console.WriteLine("Executing script " + script.ID);
				
				// Only internal scripts take in parameters.
				if (script.IsHidden)
					scriptRunner.RunScript(script.ID, parameters);
				else
					scriptRunner.RunScript(script.ID, new object[] {});
			}
		}


		//-----------------------------------------------------------------------------
		// Text Messages
		//-----------------------------------------------------------------------------

		public void DisplayMessage(Message message) {
			PushRoomState(new RoomStateTextReader(message));
		}

		public void DisplayMessage(string text) {
			DisplayMessage(new Message(text));
		}
		
		public void DisplayMessage(Message message, Action completeAction) {
			PushRoomState(new RoomStateQueue(
				new RoomStateTextReader(message),
				new RoomStateAction(completeAction)));
		}

		public void DisplayMessage(string text, Action completeAction) {
			DisplayMessage(new Message(text), completeAction);
		}
		

		//-----------------------------------------------------------------------------
		// Menu
		//-----------------------------------------------------------------------------

		public void OpenMenu(Menu currentMenu, Menu menu) {
			gameManager.PopGameState();
			gameManager.QueueGameStates(
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeOut, currentMenu),
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeIn, menu),
				menu
			);
		}

		public void OpenMenu(Menu menu) {
			AudioSystem.PlaySound(GameData.SOUND_MENU_OPEN);
			gameManager.QueueGameStates(
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeOut, roomControl),
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeIn, menu),
				menu
			);
			menuWeapons.OnOpen();
			menuSecondaryItems.OnOpen();
			menuEssences.OnOpen();
		}

		public void CloseMenu(Menu menu) {
			AudioSystem.PlaySound(GameData.SOUND_MENU_CLOSE);
			gameManager.PopGameState();
			gameManager.QueueGameStates(
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeOut, menu),
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeIn, roomControl),
				roomControl
			);
			menuWeapons.OnClose();
			menuSecondaryItems.OnClose();
			menuEssences.OnClose();
		}

		public void OpenMapScreen() {
			if (lastRoomOnMap != null && lastRoomOnMap.Dungeon != null) {
				ScreenDungeonMap mapScreen = mapDungeon;
				AudioSystem.PlaySound(GameData.SOUND_MENU_OPEN);
				gameManager.QueueGameStates(
					new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeOut, roomControl),
					new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeIn, mapScreen),
					mapScreen
				);
				mapScreen.OnOpen();
			}
		}

		public void CloseMapScreen() {
			ScreenDungeonMap mapScreen = gameManager.CurrentGameState as ScreenDungeonMap;
			AudioSystem.PlaySound(GameData.SOUND_MENU_CLOSE);
			gameManager.PopGameState();
			gameManager.QueueGameStates(
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeOut, mapScreen),
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeIn, roomControl),
				roomControl
			);
			mapScreen.OnClose();
		}


		//-----------------------------------------------------------------------------
		// Room state management
		//-----------------------------------------------------------------------------

		public void UpdateRoomState() {
			roomStateStack.Update();
		}

		public void DrawRoomState(Graphics2D g) {
			roomStateStack.Draw(g);
		}

		// Push a new room-state onto the stack and begin it.
		public void PushRoomState(RoomState state) {
			roomStateStack.Push(state);
		}

		// Push a queue of room states.
		public void QueueRoomStates(params RoomState[] states) {
			PushRoomState(new RoomStateQueue(states));
		}

		// End the top-most room state in the stack.
		public void PopRoomState() {
			roomStateStack.Pop();
		}

		// End the given number of states in the stack from the top down.
		public void PopRoomStates(int amount) {
			roomStateStack.Pop(amount);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the game manager.
		public GameManager GameManager {
			get { return gameManager; }
		}

		// Gets the current room control.
		public RoomControl RoomControl {
			get { return roomControl; }
			set {
				Level oldLevel = roomControl.Level;
				roomControl = value;

				if (!roomControl.Room.IsHiddenFromMap)
					lastRoomOnMap = roomControl.Room;
				
				// Leave the old room.
				foreach (Room room in oldLevel.GetRooms())
					room.OnRoomLeave();

				// Respawn all monsters in the previous level.
				if (roomControl.Level != oldLevel) {
					foreach (Room room in oldLevel.GetRooms())
						room.RespawnMonsters();
				}
			}
		}

		public Room LastRoomOnMap {
			get { return lastRoomOnMap; }
		}

		// Gets the world.
		public World World {
			get { return world; }
		}

		// Gets the player.
		public Player Player {
			get { return player; }
		}

		// Gets the top HUD for the game.
		public HUD HUD {
			get { return hud; }
		}

		// Gets the player's inventory.
		public Inventory Inventory {
			get { return inventory; }
		}

		// Returns true if this is an advanced game.
		public bool IsAdvancedGame {
			get { return isAdvancedGame; }
			set { isAdvancedGame = value; }
		}

		// The player weapons menu.
		public MenuWeapons MenuWeapons {
			get { return menuWeapons; }
		}

		// The player key items menu.
		public MenuSecondaryItems MenuSecondaryItems {
			get { return menuSecondaryItems; }
		}

		// The player essences menu.
		public MenuEssences MenuEssences {
			get { return menuEssences; }
		}

		public int RoomTicks {
			get { return roomTicks; }
			set { roomTicks = value; }
		}

		public RewardManager RewardManager {
			get { return rewardManager; }
		}

		public DropManager DropManager {
			get { return dropManager; }
		}

		public bool UpdateRoom {
			get { return updateRoom; }
			set { updateRoom = value; }
		}

		public bool AnimateRoom {
			get { return animateRoom; }
			set { animateRoom = value; }
		}

		public RoomState CurrentRoomState {
			get { return roomStateStack.CurrentRoomState; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.API;
using ZeldaOracle.Game.Control.Maps;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Debugging;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Drops;
using ZeldaOracle.Game.Items.KeyItems;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {

	// The main control for the current game session.
	public class GameControl : ZeldaAPI.Game, IVariableObject {

		private GameManager		gameManager;
		private RoomControl		roomControl;
		private AreaControl		areaControl;
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
		private int				nextRoomNumber;
		private int				nextMonsterID;

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
			Logs.Initialization.LogNotice("Loading world file");
			World world = worldFile.Load(fileName, false);
			LoadWorld(world, recompile);
		}

		public void LoadWorld(World world, bool recompile) {
			this.world = world;

			if (recompile) {
				//throw new NotImplementedException();
				//ScriptCompiler compiler = new ScriptCompiler();
				//compiler.Compile(
				//world.ScriptManager.CompileAndWriteAssembly(null);
			}
			scriptRunner.OnLoadWorld(world);
			world.AssignMonsterIDs();
		}

		// Start a new game.
		public void StartGame() {
			roomTicks = 0;
			nextRoomNumber = 0;
			nextMonsterID = int.MaxValue;

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

			inventory.Initialize();
			inventory.SetMaxLevel("wallet");

			hud = new HUD(this);
			hud.DynamicHealth = player.Health;

			rewardManager = new RewardManager(this);
			rewardManager.Initialize();

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
				bool devMode = false;
				string worldPath;

				// Parse the command line arguments.
				for (int i = 0; i < gameManager.LaunchParameters.Length; i++) {
					if (gameManager.LaunchParameters[i] == "-dev") {
						devMode = true;
					}
					else if (gameManager.LaunchParameters[i] == "-test") {
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

				if (devMode) {
					GameDebug.LoadDevSettings();
					worldPath = GameDebug.DevSettings.StartLocation.WorldFile;
				}
				else {
					worldPath = gameManager.LaunchParameters[0];
				}

				// Load the world file.
				LoadWorld(worldPath, recompile);

				// DEBUG: Until enter name screen exists
				Vars.Set("player", "Link");

				// Begin the starting room.
				if (test) {
					player.SetPositionByCenter(startPlayer * GameSettings.TILE_SIZE + new Point2I(8, 8));
					player.MarkRespawn();
					roomControl.BeginRoom(world.GetLevelAt(startLevel).Rooms[startRoom.X, startRoom.Y]);
				}
				else if (devMode && GameDebug.DevSettings.StartLocation.Level != "default") {
					Level level = world.GetLevel(GameDebug.DevSettings.StartLocation.Level);
					player.SetPositionByCenter(
						GameDebug.DevSettings.StartLocation.Location *
						GameSettings.TILE_SIZE + new Point2I(8, 8));
					player.MarkRespawn();
					roomControl.BeginRoom(level.GetRoomAt(
						GameDebug.DevSettings.StartLocation.Room));
				}
				else {
					player.SetPositionByCenter(world.StartTileLocation * GameSettings.TILE_SIZE + new Point2I(8, 8));
					player.MarkRespawn();
					roomControl.BeginRoom(world.StartRoom);
				}
			}


			GameData.PaletteShader.TilePalette = roomControl.Zone.Palette;
			GameData.PaletteShader.TileRatio = 0f;
			roomStateStack = new RoomStateStack(new RoomStateNormal());
			roomStateStack.Begin(this);

			if (!roomControl.Room.IsHiddenFromMap)
				lastRoomOnMap = roomControl.Room;

			roomControl.Player.ForceSideScrollingLadder();
			//roomControl.Player.RequestSpawnNaturalState();
			roomControl.Player.OnEnterRoom();

			FireEvent(world, "start_game", this);
		}


		//-----------------------------------------------------------------------------
		// Scripts
		//-----------------------------------------------------------------------------

		public void FireEvent(IEventObject caller, string eventName,
			params object[] parameters)
		{
			if (caller is ITriggerObject) {
				FireEvent((ITriggerObject) caller, eventName);
			}
			else {
				Logs.Scripts.LogError("Outdated FireEvent on " + caller.GetType().Name);
			}
		}

		public void FireEvent(ITriggerObject caller, string eventName) {
			Event evnt = caller.Events.GetEvent(eventName);
			List<Trigger> triggers = caller.Triggers.GetTriggersByEvent(evnt).ToList();
			foreach (Trigger trigger in triggers) {
				Logs.Scripts.LogNotice("Running trigger {0}", trigger.Name);
				if (trigger.FireOnce)
					trigger.IsEnabled = false;
				if (trigger.Script != null) {
					ScriptRunner.RunScript(trigger.Script, new object[] { caller });
				}
			}
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
				// Only internal scripts take in parameters
				if (script.IsHidden)
					scriptRunner.RunScript(script.ID, parameters);
				else
					scriptRunner.RunScript(script.ID, new object[] {});
			}
		}


		//-----------------------------------------------------------------------------
		// Text Messages
		//-----------------------------------------------------------------------------

		public void DisplayMessage(Message message, TextReaderArgs args = null) {
			PushRoomState(new RoomStateTextReader(message, args,
				 inventory.PiecesOfHeart));
		}

		public void DisplayMessage(string text, TextReaderArgs args = null) {
			DisplayMessage(new Message(text), args);
		}
		
		public void DisplayMessage(Message message, TextReaderArgs args,
			Action completeAction)
		{
			PushRoomState(new RoomStateQueue(
				new RoomStateTextReader(message, args, inventory.PiecesOfHeart),
				new RoomStateAction(completeAction)));
		}

		public void DisplayMessage(string text, TextReaderArgs args,
			Action completeAction)
		{
			DisplayMessage(new Message(text), args, completeAction);
		}

		public void DisplayMessage(Message message, TextReaderArgs args,
			params RoomState[] completeStates)
		{
			var queue = new RoomStateQueue(new RoomStateTextReader(message, args,
				inventory.PiecesOfHeart));
			queue.AddRange(completeStates);
			PushRoomState(queue);
		}

		public void DisplayMessage(string text, TextReaderArgs args,
			params RoomState[] completeStates)
		{
			DisplayMessage(new Message(text), args, completeStates);
		}


		//-----------------------------------------------------------------------------
		// Menu
		//-----------------------------------------------------------------------------

		public void OpenMenu(Menu currentMenu, Menu menu) {
			gameManager.PopGameState();
			gameManager.QueueGameStates(
				new TransitionFade(Color.GBCWhite, 20, FadeType.FadeOut, currentMenu),
				new TransitionFade(Color.GBCWhite, 20, FadeType.FadeIn, menu),
				menu
			);
		}

		public void OpenMenu(Menu menu) {
			AudioSystem.PlaySound(GameData.SOUND_MENU_OPEN);
			gameManager.QueueGameStates(
				new TransitionFade(Color.GBCWhite, 20, FadeType.FadeOut, roomControl),
				new TransitionFade(Color.GBCWhite, 20, FadeType.FadeIn, menu),
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
				new TransitionFade(Color.GBCWhite, 20, FadeType.FadeOut, menu),
				new TransitionFade(Color.GBCWhite, 20, FadeType.FadeIn, roomControl),
				roomControl
			);
			menuWeapons.OnClose();
			menuSecondaryItems.OnClose();
			menuEssences.OnClose();
		}

		public void OpenMapScreen() {
			if (lastRoomOnMap != null && lastRoomOnMap.Area != null) {
				MapScreen mapScreen = null;
				if (AreaControl.Area.MapType == MapType.Dungeon) {
					mapScreen = mapDungeon;
				}
				else if (AreaControl.Area.MapType == MapType.Overworld) {
					//mapScreen = mapOverworld
				}
				if (mapScreen != null) {
					AudioSystem.PlaySound(GameData.SOUND_MENU_OPEN);
					gameManager.QueueGameStates(
						new TransitionFade(Color.GBCWhite, 20, FadeType.FadeOut, roomControl),
						new TransitionFade(Color.GBCWhite, 20, FadeType.FadeIn, mapScreen),
						mapScreen
					);
					mapScreen.OnOpen();
				}
			}
		}

		public void CloseMapScreen() {
			ScreenDungeonMap mapScreen = gameManager.CurrentGameState as ScreenDungeonMap;
			AudioSystem.PlaySound(GameData.SOUND_MENU_CLOSE);
			gameManager.PopGameState();
			gameManager.QueueGameStates(
				new TransitionFade(Color.GBCWhite, 20, FadeType.FadeOut, mapScreen),
				new TransitionFade(Color.GBCWhite, 20, FadeType.FadeIn, roomControl),
				roomControl
			);
			mapScreen.OnClose();
		}


		//-----------------------------------------------------------------------------
		// Area Management
		//-----------------------------------------------------------------------------
		
		/// <summary>Call this when a room begins. If the supplied area does not match
		/// the current area then area-reset will take effect.</summary>
		/*public void BeginArea(Area area) {
			if (areaControl != null && areaControl.Area != area) {
				if (areaControl != null)
					areaControl.EndArea();
				areaControl = new AreaControl(this, area);
				areaControl.BeginArea();
			}
		}*/

		/// <summary>Returns the existing area control if the areas match or creates
		/// a new one.</summary>
		public AreaControl GetAreaControl(Area area) {
			if (areaControl == null || areaControl.Area != area)
				return new AreaControl(this, area);
			return areaControl;
		}


		//-----------------------------------------------------------------------------
		// Room State Management
		//-----------------------------------------------------------------------------

		public int NextRoomNumber() {
			return nextRoomNumber++;
		}

		public int NextMonsterID() {
			return nextMonsterID--;
		}

		public void UpdateRoomState() {
			roomStateStack.Update();
		}
		
		public void UpdateScripts() {
			scriptRunner.UpdateScriptExecution();
		}
		
		public void OnLeaveRoom(RoomControl roomControl) {
			scriptRunner.TerminateRoomScripts(roomControl);
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
				if (roomControl == value)
					return;

				// Leave the previous room
				roomControl.Room.OnLeaveRoom();

				// Leave the previous area
				if (value.Level != roomControl.Level) {
					foreach (Room room in roomControl.Level.GetRooms())
						room.OnLeaveArea();
				}

				roomControl = value;

				if (!roomControl.Room.IsHiddenFromMap)
					lastRoomOnMap = roomControl.Room;
			}
		}

		/// <summary>Gets the current area control. This should be assigned to
		/// during BeginRoom</summary>
		public AreaControl AreaControl {
			get { return areaControl; }
			set {
				if (areaControl == value)
					return;

				// Leave the previous area
				if (areaControl != null)
					areaControl.EndArea();
				
				// Begin the new area
				areaControl = value;
				areaControl.BeginArea();
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

		public Variables Vars {
			get { return world.Vars; }
		}

		public ScriptRunner ScriptRunner {
			get { return scriptRunner; }
		}
	}
}

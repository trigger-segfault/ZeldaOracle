using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Game.Debug;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Equipment;
using ZeldaOracle.Game.Items.Essences;
using ZeldaOracle.Game.Items.KeyItems;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {

	// The main control for the current game session.
	public class GameControl {

		private GameManager		gameManager;
		private RoomControl		roomControl;
		private World			world;
		private Player			player;
		private HUD				hud;
		private Inventory		inventory;
		private RewardManager	rewardManager;
		private RoomStateStack	roomStateStack;
		private bool			isAdvancedGame;
		private int				roomTicks; // The total number of ticks elapsed since the game was started (used for animation).
		private bool			updateRoom;
		private bool			animateRoom;

		// Menus
		private MenuWeapons			menuWeapons;
		private MenuSecondaryItems	menuSecondaryItems;
		private MenuEssences		menuEssences;


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
			this.isAdvancedGame		= false;
			this.updateRoom			= true;
			this.animateRoom		= true;
			this.menuWeapons		= null;
			this.menuSecondaryItems	= null;
			this.menuEssences		= null;
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		// Start a new game.
		public void StartGame() {
			roomTicks = 0;

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

			GameData.LoadInventory(inventory, true);
			
			inventory.ObtainAmmo("ammo_scent_seeds");
			inventory.ObtainAmmo("ammo_pegasus_seeds");
			inventory.ObtainAmmo("ammo_gale_seeds");
			inventory.ObtainAmmo("ammo_mystery_seeds");

			hud = new HUD(this);
			hud.DynamicHealth = player.Health;

			rewardManager = new RewardManager(this);

			GameData.LoadRewards(rewardManager);

			// Create the room control.
			roomControl = new RoomControl();
			gameManager.PushGameState(roomControl);

			// Create the test world.
			
			// Load the world.
			//WorldFile worldFile = new WorldFile();
			//world = worldFile.Load("Content/Worlds/temp_world.zwd");

			// Begin the room state.
			if (gameManager.LaunchParameters.Length > 0) {
				WorldFile worldFile = new WorldFile();
				world = worldFile.Load(gameManager.LaunchParameters[0]);
				if (gameManager.LaunchParameters.Length > 1 && gameManager.LaunchParameters[1] == "-test") {
					int startLevel = Int32.Parse(gameManager.LaunchParameters[2]);
					int startRoomX = Int32.Parse(gameManager.LaunchParameters[3]);
					int startRoomY = Int32.Parse(gameManager.LaunchParameters[4]);
					int startPlayerX = Int32.Parse(gameManager.LaunchParameters[5]);
					int startPlayerY = Int32.Parse(gameManager.LaunchParameters[6]);

					player.Position = new Point2I(startPlayerX, startPlayerY) * GameSettings.TILE_SIZE + new Point2I(8, 16);
					player.MarkRespawn();
					roomControl.BeginRoom(world.Levels[startLevel].Rooms[startRoomX, startRoomY]);
				}
				else {
					player.Position = world.StartTileLocation * GameSettings.TILE_SIZE + new Point2I(8, 16);
					player.MarkRespawn();
					roomControl.BeginRoom(world.StartRoom);
				}
			}
			else {
				//WorldFile worldFile = new WorldFile();
				//world = worldFile.Load("temp_world.zwd");
				world = GameDebug.CreateTestWorld();
				player.Position = world.StartTileLocation * GameSettings.TILE_SIZE + new Point2I(8, 16);
				player.MarkRespawn();
				roomControl.BeginRoom(world.StartRoom);
			}
			roomStateStack = new RoomStateStack(new RoomStateNormal());
			roomStateStack.Begin(this);

			AudioSystem.MasterVolume = 0.06f;
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
			PushRoomState(new RoomStateQueue(new RoomStateTextReader(message), new RoomStateAction(completeAction)));
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
			AudioSystem.PlaySound("UI/menu_open");
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
			AudioSystem.PlaySound("UI/menu_close");
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
			set { roomControl = value; }
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

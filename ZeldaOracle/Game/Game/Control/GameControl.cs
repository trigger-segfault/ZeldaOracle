using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Equipment;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {

	// The main control for the current game session.
	public class GameControl {

		private GameManager gameManager;
		private RoomControl roomControl;
		private World world;
		private Player player;
		private HUD hud;
		private Inventory inventory;
		private bool advancedGame;

		private MenuInventory menuInventory;
		private MenuKeyItems menuKeyItems;
		private MenuEssences menuEssences;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public GameControl(GameManager gameManager) {
			this.gameManager	= gameManager;
			this.roomControl	= null;
			this.world			= null;
			this.player			= null;
			this.hud			= null;
			this.inventory		= null;
			this.advancedGame	= false;

			this.menuInventory	= null;
			this.menuKeyItems	= null;
			this.menuEssences	= null;
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		public void StartGame() {
			inventory = new Inventory(this);
			inventory.AddItem(new ItemFeather());
			inventory.AddItem(new ItemBow());
			inventory.AddItem(new ItemWallet(2));
			inventory.EquipUsableItem((UsableItem)inventory.GetItem("item_bow"), 0);
			inventory.EquipUsableItem((UsableItem)inventory.GetItem("item_feather"), 1);

			hud = new HUD(this);

			// TODO: Load world here.
			roomControl		= new RoomControl();
			gameManager.PushGameState(roomControl);

			roomControl.BeginTestWorld();
			player = roomControl.Player;
			menuInventory	= new MenuInventory(gameManager);
			menuKeyItems	= new MenuKeyItems(gameManager);
			menuEssences	= new MenuEssences(gameManager);
			menuInventory.PreviousMenu = menuEssences;
			menuInventory.NextMenu = menuKeyItems;
			menuKeyItems.PreviousMenu = menuInventory;
			menuKeyItems.NextMenu = menuEssences;
			menuEssences.PreviousMenu = menuKeyItems;
			menuEssences.NextMenu = menuInventory;

			AudioSystem.PlaySong("overworld", true);
		}

		public void DisplayMessage(Message message) {
			gameManager.PushGameState(new StateTextReader(message));
		}

		public void DisplayMessage(string text) {
			gameManager.PushGameState(new StateTextReader(new Message(text)));
		}

		public void OpenMenu(Menu menu) {
			AudioSystem.PlaySound("UI/menu_open");
			gameManager.QueueGameStates(
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeOut, roomControl),
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeIn, menu),
				menu
			);
		}
		public void CloseMenu(Menu menu) {
			AudioSystem.PlaySound("UI/menu_close");
			gameManager.PopGameState();
			gameManager.QueueGameStates(
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeOut, menu),
				new TransitionFade(new Color(248, 248, 248), 20, FadeType.FadeIn, roomControl),
				roomControl
			);
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
			get { return advancedGame; }
			set { advancedGame = value; }
		}

		// The player inventory menu.
		public Menu MenuInventory {
			get { return menuInventory; }
		}
	}
}

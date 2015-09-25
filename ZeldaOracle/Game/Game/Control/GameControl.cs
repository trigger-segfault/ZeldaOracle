using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control.Menus;
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

		private GameManager gameManager;
		private RoomControl roomControl;
		private World world;
		private Player player;
		private HUD hud;
		private Inventory inventory;
		private RewardManager rewardManager;
		private bool advancedGame;
		private int roomTicks; // The total number of ticks elapsed (used for animation.
		private RoomStateStack roomStateStack;
		private bool updateRoom;
		private bool animateRoom;

		// Menus
		private MenuWeapons menuWeapons;
		private MenuSecondaryItems menuSecondaryItems;
		private MenuEssences menuEssences;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public GameControl(GameManager gameManager) {
			this.gameManager	= gameManager;
			this.roomStateStack	= null;
			this.roomControl	= null;
			this.world			= null;
			this.player			= null;
			this.hud			= null;
			this.inventory		= null;
			this.rewardManager	= null;
			this.advancedGame	= false;
			this.updateRoom		= true;
			this.animateRoom	= true;

			this.menuWeapons	= null;
			this.menuSecondaryItems	= null;
			this.menuEssences	= null;
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		public void StartGame() {

			// Setup the player beforehand so certain classes such as the HUD can reference it
			player = new Player();

			menuWeapons	= new MenuWeapons(gameManager);
			menuSecondaryItems	= new MenuSecondaryItems(gameManager);
			menuEssences	= new MenuEssences(gameManager);
			menuWeapons.PreviousMenu = menuEssences;
			menuWeapons.NextMenu = menuSecondaryItems;
			menuSecondaryItems.PreviousMenu = menuWeapons;
			menuSecondaryItems.NextMenu = menuEssences;
			menuEssences.PreviousMenu = menuSecondaryItems;
			menuEssences.NextMenu = menuWeapons;

			roomTicks = 0;

			inventory = new Inventory(this);
			inventory.AddItem(new ItemWallet(2), true);
			inventory.AddItem(new ItemBracelet(), true);
			inventory.AddItem(new ItemFeather(), true);
			inventory.AddItem(new ItemBow(), true);
			inventory.AddItem(new ItemEssence1(), true);
			inventory.AddItem(new ItemEssence2(), true);
			inventory.AddItem(new ItemEssence3(), true);
			inventory.AddItem(new ItemEssence4(), true);
			inventory.AddItem(new ItemEssence5(), true);
			inventory.AddItem(new ItemEssence6(), true);
			inventory.AddItem(new ItemEssence7(), true);
			inventory.AddItem(new ItemEssence8(), true);
			inventory.AddItem(new ItemFlippers(), true);
			inventory.AddItem(new ItemMagicPotion(), true);
			inventory.AddItem(new ItemEssenceSeed(), true);
			inventory.AddItem(new ItemBombs(), true);
			inventory.AddItem(new ItemOcarina(), true);
			inventory.AddItem(new ItemBigSword(), true);
			inventory.AddItem(new ItemMembersCard(), true);
			inventory.AddItem(new ItemSword(), true);
			inventory.AddItem(new ItemShield(), true);
			inventory.AddItem(new ItemBoomerang(), true);
			inventory.AddItem(new ItemSeedSatchel(), true);
			inventory.AddItem(new ItemSeedShooter(), true);
			inventory.AddItem(new ItemSlingshot(), true);

			inventory.ObtainAmmo(inventory.GetAmmo("ammo_scent_seeds"));
			inventory.ObtainAmmo(inventory.GetAmmo("ammo_pegasus_seeds"));
			inventory.ObtainAmmo(inventory.GetAmmo("ammo_gale_seeds"));
			inventory.ObtainAmmo(inventory.GetAmmo("ammo_mystery_seeds"));

			hud = new HUD(this);
			hud.DynamicHealth = player.Health;

			rewardManager = new RewardManager(this);

			rewardManager.AddReward(new RewardRupee("rupees_1", 1,
				"You got<n><red>1 Rupee<red>!<n>That's terrible.",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 5, 3, -4, -9)));

			rewardManager.AddReward(new RewardRupee("rupees_5", 5,
				"You got<n><red>5 Rupees<red>!<n>That's nice.",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 6, 3, -4, -9)));

			rewardManager.AddReward(new RewardRupee("rupees_20", 20,
				"You got<n><red>20 Rupees<red>!<n>That's nice.",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 7, 3, -4, -9)));

			rewardManager.AddReward(new RewardRupee("rupees_30", 30,
				"You got<n><red>30 Rupees<red>!<n>That's nice.",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 7, 3, -4, -9)));

			rewardManager.AddReward(new RewardRupee("rupees_50", 50,
				"You got<n><red>50 Rupees<red>!<n>That's nice.",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 7, 3, -4, -9)));

			rewardManager.AddReward(new RewardRupee("rupees_100", 100,
				"You got<n><red>100 Rupees<red>!<n>That's nice.",
				new Sprite(GameData.SHEET_ITEMS_LARGE, 0, 3, -8, -9)));

			rewardManager.AddReward(new RewardRupee("rupees_200", 200,
				"You got<n><red>200 Rupees<red>!<n>That's nice.",
				new Sprite(GameData.SHEET_ITEMS_LARGE, 1, 3, -8, -9)));

			rewardManager.AddReward(new RewardItem("item_flippers_1", "item_flippers", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Zora's Flippers<red>! You can now go for a swim! Press A to swim, B to dive!",
				new Sprite(GameData.SHEET_ITEMS_LARGE, 6, 1, -8, -8)));

			rewardManager.AddReward(new RewardItem("item_sword_1", "item_sword", Item.Level1, RewardHoldTypes.OneHand,
				"You got a Hero's <red>Wooden Sword<red>! Hold A or B to charge it up, then release it for a spin attack!",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 0, 0, -4, -8)));

			rewardManager.AddReward(new RewardRecoveryHeart("hearts_1", 1,
				"You got a<n><red>Recovery Heart<red>!",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 13, 3, -4, -6)));

			rewardManager.AddReward(new RewardAmmo("ammo_ember_seeds_5", "ammo_ember_seeds", 5,
				"You got<n><red>5 Ember Seeds<red>!",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 0, 3, -4, -1)));

			rewardManager.AddReward(new RewardAmmo("ammo_scent_seeds_5", "ammo_scent_seeds", 5,
				"You got<n><red>5 Scent Seeds<red>!",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 1, 3, -4, -1)));

			rewardManager.AddReward(new RewardAmmo("ammo_pegasus_seeds_5", "ammo_pegasus_seeds", 5,
				"You got<n><red>5 Pegasus Seeds<red>!",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 2, 3, -4, -1)));

			rewardManager.AddReward(new RewardAmmo("ammo_gale_seeds_5", "ammo_gale_seeds", 5,
				"You got<n><red>5 Gale Seeds<red>!",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 3, 3, -4, -1)));

			rewardManager.AddReward(new RewardAmmo("ammo_mystery_seeds_5", "ammo_mystery_seeds", 5,
				"You got<n><red>5 Mystery Seeds<red>!",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 4, 3, -4, -1)));

			rewardManager.AddReward(new RewardAmmo("ammo_bombs_5", "ammo_bombs", 5,
				"You got<n><red>5 Bombs<red>!",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 13, 0, -4, -8)));


			rewardManager.AddReward(new RewardAmmo("ammo_arrows_5", "ammo_arrows", 5,
				"You got<n><red>5 Arrows<red>!",
				new Sprite(GameData.SHEET_ITEMS_SMALL, 15, 1, -4, -10)));


			// TODO: Load world here.
			roomControl		= new RoomControl();
			gameManager.PushGameState(roomControl);

			roomControl.BeginTestWorld(player);

			roomStateStack = new RoomStateStack(new RoomStateNormal());
			roomStateStack.Begin(this);


			AudioSystem.MasterVolume = 0.01f;

		}

		public void DisplayMessage(Message message) {
			PushRoomState(new RoomStateTextReader(message));
		}

		public void DisplayMessage(string text) {
			PushRoomState(new RoomStateTextReader(new Message(text)));
		}

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
			get { return advancedGame; }
			set { advancedGame = value; }
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

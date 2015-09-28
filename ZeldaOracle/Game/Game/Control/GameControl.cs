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
			
			inventory.AddItems(true,
				new ItemWallet(),
				new ItemSword(),
				new ItemBracelet(),
				new ItemFeather(),
				new ItemBow(),
				new ItemEssence1(),
				new ItemEssence2(),
				new ItemEssence3(),
				new ItemEssence4(),
				new ItemEssence5(),
				new ItemEssence6(),
				new ItemEssence7(),
				new ItemEssence8(),
				new ItemFlippers(),
				new ItemMagicPotion(),
				new ItemEssenceSeed(),
				new ItemBombs(),
				new ItemOcarina(),
				new ItemBigSword(),
				new ItemMembersCard(),
				new ItemSword(),
				new ItemShield(),
				new ItemBoomerang(),
				new ItemSeedSatchel(),
				new ItemSeedShooter(),
				new ItemSlingshot());
			
			inventory.ObtainAmmo("ammo_scent_seeds");
			//inventory.ObtainAmmo("ammo_pegasus_seeds");
			//inventory.ObtainAmmo("ammo_gale_seeds");
			inventory.ObtainAmmo("ammo_mystery_seeds");

			hud = new HUD(this);
			hud.DynamicHealth = player.Health;

			rewardManager = new RewardManager(this);

			rewardManager.AddReward(new RewardRupee("rupees_1", 1,
				"You got <red>1 Rupee<red>!<n>That's terrible.",
				GameData.SPR_REWARD_RUPEE_GREEN));

			rewardManager.AddReward(new RewardRupee("rupees_5", 5,
				"You got<n><red>5 Rupees<red>!",
				GameData.SPR_REWARD_RUPEE_RED));

			rewardManager.AddReward(new RewardRupee("rupees_20", 20,
				"You got<n><red>20 Rupees<red>!<n>That's not bad.",
				GameData.SPR_REWARD_RUPEE_BLUE));

			rewardManager.AddReward(new RewardRupee("rupees_30", 30,
				"You got<n><red>30 Rupees<red>!<n>That's nice.",
				GameData.SPR_REWARD_RUPEE_BLUE));

			rewardManager.AddReward(new RewardRupee("rupees_50", 50,
				"You got<n><red>50 Rupees<red>!<n>How lucky!",
				GameData.SPR_REWARD_RUPEE_BLUE));

			rewardManager.AddReward(new RewardRupee("rupees_100", 100,
				"You got <red>100<n>Rupees<red>! I bet<n>you're thrilled!",
				GameData.SPR_REWARD_RUPEE_BIG_BLUE));

			rewardManager.AddReward(new RewardRupee("rupees_150", 150,
				"You got <red>150<n>Rupees<red>!<n>Way to go!!!",
				GameData.SPR_REWARD_RUPEE_BIG_RED));

			rewardManager.AddReward(new RewardRupee("rupees_200", 200,
				"You got <red>200<n>Rupees<red>! That's<n>pure bliss!",
				GameData.SPR_REWARD_RUPEE_BIG_RED));

			rewardManager.AddReward(new RewardItem("item_flippers_1", "item_flippers", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Zora's Flippers<red>! You can now go for a swim! Press A to swim, B to dive!",
				GameData.SPR_ITEM_ICON_FLIPPERS_1));

			rewardManager.AddReward(new RewardItem("item_sword_1", "item_sword", Item.Level1, RewardHoldTypes.OneHand,
				"You got a Hero's <red>Wooden Sword<red>! Hold A or B to charge it up, then release it for a spin attack!",
				GameData.SPR_ITEM_ICON_SWORD_1));

			rewardManager.AddReward(new RewardRecoveryHeart("hearts_1", 1,
				"You recovered<n>only one <red>heart<red>!",
				GameData.SPR_REWARD_HEART));

			rewardManager.AddReward(new RewardRecoveryHeart("hearts_3", 3,
				"You got three<n><red>hearts<red>!",
				GameData.SPR_REWARD_HEARTS_3));

			rewardManager.AddReward(new RewardHeartPiece());

			rewardManager.AddReward(new RewardHeartContainer());

			rewardManager.AddReward(new RewardAmmo("ammo_ember_seeds_5", "ammo_ember_seeds", 5,
				"You got<n><red>5 Ember Seeds<red>!",
				GameData.SPR_REWARD_SEED_EMBER));

			rewardManager.AddReward(new RewardAmmo("ammo_scent_seeds_5", "ammo_scent_seeds", 5,
				"You got<n><red>5 Scent Seeds<red>!",
				GameData.SPR_REWARD_SEED_SCENT));

			rewardManager.AddReward(new RewardAmmo("ammo_pegasus_seeds_5", "ammo_pegasus_seeds", 5,
				"You got<n><red>5 Pegasus Seeds<red>!",
				GameData.SPR_REWARD_SEED_PEGASUS));

			rewardManager.AddReward(new RewardAmmo("ammo_gale_seeds_5", "ammo_gale_seeds", 5,
				"You got<n><red>5 Gale Seeds<red>!",
				GameData.SPR_REWARD_SEED_GALE));

			rewardManager.AddReward(new RewardAmmo("ammo_mystery_seeds_5", "ammo_mystery_seeds", 5,
				"You got<n><red>5 Mystery Seeds<red>!",
				GameData.SPR_REWARD_SEED_MYSTERY));

			rewardManager.AddReward(new RewardAmmo("ammo_bombs_5", "ammo_bombs", 5,
				"You got<n><red>5 Bombs<red>!",
				GameData.SPR_REWARD_SEED_EMBER));

			rewardManager.AddReward(new RewardAmmo("ammo_arrows_5", "ammo_arrows", 5,
				"You got<n><red>5 Arrows<red>!",
				GameData.SPR_REWARD_SEED_EMBER));

			// Create the room control.
			roomControl = new RoomControl();
			gameManager.PushGameState(roomControl);

			// Create the test world.
			world = GameDebug.CreateTestWorld();
			
			// Load the world.
			//WorldFile worldFile = new WorldFile();
			//world = worldFile.Load("Content/Worlds/temp_world.zwd");

			// Begin the room state.
			player.Position = world.StartTileLocation * GameSettings.TILE_SIZE;
			roomControl.BeginRoom(world.StartRoom);
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
			PushRoomState(new RoomStateTextReader(new Message(text)));
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

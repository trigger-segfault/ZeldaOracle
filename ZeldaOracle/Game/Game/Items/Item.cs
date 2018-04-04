using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game.Items {
	public abstract class Item : ISlotItem {

		private Inventory	inventory;
		private ItemData	itemData;
		private string		id;

		private string[]	name;
		private string[]	description;
		private string[]	message;
		private ISprite[]	sprite;
		private int			maxLevel;
		private RewardHoldTypes holdType;

		private int[]		price;

		private Ammo[]		ammo;
		private int[]		maxAmmo;
		private bool		levelUpAmmo;
		
		// TODO: Store these as properties for save format
		private int			level;
		private bool		isObtained;
		private bool		isLost;

		
		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		public const int Level1 = 0;
		public const int Level2 = 1;
		public const int Level3 = 2;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the base item.</summary>
		protected Item() {
			inventory		= null;
			itemData		= null;
			id				= "";

			name			= new string[0];
			description		= new string[0];
			message			= new string[0];
			sprite			= new ISprite[0];
			level			= Item.Level1;
			maxLevel		= Item.Level1;
			holdType		= RewardHoldTypes.TwoHands;

			price			= new int[0];

			ammo			= null;
			maxAmmo			= null;
			levelUpAmmo		= false;

			isObtained		= false;
			isLost			= false;
		}

		/// <summary>Constructs the base item with the specified ID.</summary>
		protected Item(string id) : this() {
			this.id			= id;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the name of the item at the specified level.</summary>
		public string GetName(int level) {
			if (level < name.Length)
				return name[level];
			return name.LastOrDefault() ?? "";
		}

		/// <summary>Gets the description of the item at the specified level.</summary>
		public string GetDescription(int level) {
			if (level < description.Length)
				return description[level];
			return description.LastOrDefault() ?? "";
		}

		/// <summary>Gets the reward message of the item at the specified level.</summary>
		public string GetMessage(int level) {
			if (level < message.Length)
				return message[level];
			return message.LastOrDefault() ?? "";
		}

		/// <summary>Gets the sprite of the item at the specified level.</summary>
		public ISprite GetSprite(int level) {
			if (level < sprite.Length)
				return sprite[level];
			return sprite.LastOrDefault() ?? new EmptySprite();
		}

		/// <summary>Gets the price of the item at the specified level.</summary>
		public int GetDefaultPrice(int level) {
			if (level < price.Length)
				return price[level];
			return price.LastOrDefault();
		}

		/// <summary>Gets the collection of this item's levels.</summary>
		public IEnumerable<int> GetLevels() {
			for (int i = 0; i <= maxLevel; i++) {
				yield return i;
			}
		}

		/// <summary>Gets the max ammo of the item at the specified level.</summary>
		public int GetMaxAmmo(int level) {
			if (maxAmmo == null)
				return 0;
			if (level < maxAmmo.Length)
				return maxAmmo[level];
			return maxAmmo.LastOrDefault();
		}


		//-----------------------------------------------------------------------------
		// Protected Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the leveled names of the item.</summary>
		public void SetName(params string[] names) {
			name = names;
		}

		/// <summary>Sets the leveled descriptions of the item.</summary>
		public void SetDescription(params string[] descriptions) {
			description = descriptions;
		}

		/// <summary>Sets the leveled reward messages of the item.</summary>
		public void SetMessage(params string[] messages) {
			message = messages;
		}

		/// <summary>Sets the leveled sprites of the item.</summary>
		public void SetSprite(params ISprite[] sprites) {
			sprite = sprites;
		}

		/// <summary>Sets the default leveled prices of the item.</summary>
		public void SetDefaultPrice(params int[] prices) {
			price = prices;
		}

		/// <summary>Sets the ammo types used by this weapon.</summary>
		public virtual void SetAmmo(params string[] ammoIDs) {
			ammo = new Ammo[ammoIDs.Length];
			for (int i = 0; i < ammo.Length; i++) {
				Ammo ammoRes = Resources.Get<Ammo>(ammoIDs[i]);
				if (ammoRes == null)
					throw new ArgumentException("Ammo with name '" + ammoIDs[i] +
						"' does not exist!");
				ammo[i] = ammoRes;
			}
		}

		/// <summary>Sets the ammo types used by this weapon.</summary>
		public virtual void SetAmmo(params Ammo[] ammos) {
			ammo = new Ammo[ammos.Length];
			for (int i = 0; i < ammo.Length; i++) {
				ammo[i] = ammos[i];
			}
		}

		/// <summary>Sets the max ammo allowed at each level.</summary>
		public void SetMaxAmmo(params int[] maxAmmos) {
			maxAmmo = maxAmmos;
		}


		//-----------------------------------------------------------------------------
		// Ammo
		//-----------------------------------------------------------------------------

		/// <summary>Gets the ammo at the specified index in the list.</summary>
		public Ammo GetAmmoAt(int index) {
			return ammo[index];
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the item from the item data.</summary>
		public static Item CreateItem(ItemData data) {
			Item item = ReflectionHelper.Construct<Item>(data.Type);
			
			// Load item data
			item.itemData		= data;
			item.id				= data.ResourceName;
			item.sprite			= data.Sprites;
			item.name			= data.Names;
			item.description	= data.Descriptions;
			item.message		= data.Messages;
			item.price			= data.Prices;
			item.maxAmmo		= data.MaxAmmos;
			item.maxLevel		= data.MaxLevel;
			item.holdType		= data.HoldType;
			item.levelUpAmmo	= data.LevelUpAmmo;

			data.Properties.Print();

			return item;
		}

		/// <summary>Initializes the item after it's added to the inventory list.</summary>
		public void Initialize(Inventory inventory) {
			this.inventory = inventory;


			// Load and setup ammo
			string[] ammoID = itemData.Ammos;
			if (ammoID.Any()) {
				ammo = new Ammo[ammoID.Length];
				for (int i = 0; i < ammo.Length; i++) {
					ammo[i] = inventory.GetAmmo(ammoID[i]);
					if (IsAmmoContainer) {
						ammo[i].MaxAmount = GetMaxAmmo(level);
						ammo[i].Container = this;
					}
				}
			}
			/*if (ammo != null) {
				foreach (Ammo ammo in this.ammo) {
					if (IsAmmoContainer)
						ammo.Container = this;
				}
			}*/

			// Initialize child classes
			OnInitialize();
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the item after it's added to the inventory list.</summary>
		protected virtual void OnInitialize() { }

		/// <summary>Called when the item's level is changed.</summary>
		protected virtual void OnLevelUp() { }

		/// <summary>Called when the item has been obtained.</summary>
		protected virtual void OnObtained() { }

		/// <summary>Called when the item has been unobtained.</summary>
		protected virtual void OnUnobtained() { }

		/// <summary>Called when the item has been lost.</summary>
		protected virtual void OnLost() { }

		/// <summary>Called when the lost item has been reobtained.</summary>
		protected virtual void OnReobtained() { }

		/// <summary>Draws the item inside the inventory.</summary>
		public virtual void DrawSlot(Graphics2D g, Point2I position) {
			DrawSprite(g, position);
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		protected virtual void DrawSprite(Graphics2D g, Point2I position) {
			g.DrawSprite(Sprite, position);
		}

		protected virtual void DrawLevel(Graphics2D g, Point2I position) {
			g.DrawSprite(GameData.SPR_HUD_LEVEL, position + new Point2I(8, 8));
			g.DrawString(GameData.FONT_SMALL, DisplayLevel.ToString(), position + new Point2I(16, 8), EntityColors.Black);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void AmmoLevelUp() {
			if (IsAmmoContainer) {
				foreach (Ammo ammo in this.ammo) {
					ammo.MaxAmount = GetMaxAmmo(Level);
					if (ammo.IsObtained && IncreaseAmmoOnLevelUp)
						ammo.Amount = ammo.MaxAmount;
				}
			}
		}

		private void AmmoObtained() {
			if (IsAmmoContainer) {
				if (isObtained) {
					inventory.ObtainAmmo(ammo[0]);
					ammo[0].MaxAmount = GetMaxAmmo(level);
					if (IncreaseAmmoOnLevelUp)
						ammo[0].Amount = ammo[0].MaxAmount;
				}
				else {
					foreach (Ammo ammo in this.ammo) {
						ammo.IsObtained = false;
					}
				}
			}
		}

		private void AmmoLost() {
			if (IsAmmoContainer) {
				foreach (Ammo ammo in this.ammo) {
					if (ammo.IsObtained)
						ammo.IsObtained = isLost;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the item data used to construct this item.</summary>
		public ItemData ItemData {
			get { return itemData; }
		}
		
		/// <summary>Get or sets the inventory containing this item.</summary>
		public Inventory Inventory {
			get { return inventory; }
			set { inventory = value; }
		}

		/// <summary>Gets the current game control.</summary>
		public GameControl GameControl {
			get { return inventory.GameControl; }
		}

		/// <summary>Gets the current room control.</summary>
		public RoomControl RoomControl {
			get { return inventory.GameControl.RoomControl; }
		}

		/// <summary>Gets the player.</summary>
		public Player Player {
			get { return inventory.GameControl.Player; }
		}

		/// <summary>Gets the id of the item.</summary>
		public string ID {
			get { return id; }
		}

		/// <summary>Gets the name of the item.</summary>
		public string Name {
			get { return GetName(level); }
			//set { SetName(value); }
		}

		/// <summary>Gets the description of the item.</summary>
		public string Description {
			get { return GetDescription(level); }
			//set { SetDescription(value); }
		}

		/// <summary>Gets the reward message of the item.</summary>
		public string Message {
			get { return GetMessage(level); }
			//set { SetMessage(value); }
		}

		/// <summary>Gets the sprite of the item.</summary>
		public ISprite Sprite {
			get { return GetSprite(level); }
			//set { SetSprite(value); }
		}

		/// <summary>Gets the hold type for the item during the reward state.</summary>
		public RewardHoldTypes HoldType {
			get { return holdType; }
			set { holdType = value; }
		}

		/// <summary>Gets the level of the item.</summary>
		public int Level {
			get { return level; }
			set {
				value = GMath.Clamp(value, 0, maxLevel);
				if (level != value) {
					level = value;
					AmmoLevelUp();
					OnLevelUp();
				}
			}
		}

		/// <summary>Gets the dispaly level for the item and not the level index.</summary>
		public int DisplayLevel {
			get { return level + 1; }
		}

		/// <summary>Gets the highest item level.</summary>
		public int MaxLevel {
			get { return maxLevel; }
			set { maxLevel = GMath.Max(Item.Level1, value); }
		}

		/// <summary>Gets if the item has multiple levels.</summary>
		public bool IsLeveled {
			get { return MaxLevel > Item.Level1; }
		}

		/// <summary>Gets if the item has been obtained.</summary>
		public bool IsObtained {
			get { return isObtained; }
			set {
				if (isObtained != value) {
					isObtained = value;
					AmmoObtained();
					if (isObtained)
						OnObtained();
					else
						OnUnobtained();
				}
			}
		}

		/// <summary>Gets if the item has been lost.</summary>
		public bool IsLost {
			get { return isLost; }
			set {
				if (isLost != value) {
					isLost = value;
					AmmoLost();
					if (isLost)
						OnLost();
					else
						OnReobtained();
				}
			}
		}

		/// <summary>Gets if the item has been obtained and is not lost.</summary>
		public bool IsAvailable {
			get { return isObtained && !isLost; }
		}

		/// <summary>Returns the number of ammo types this item uses.</summary>
		public int AmmoCount {
			get {
				if (ammo != null)
					return ammo.Length;
				return 0;
			}
		}

		/// <summary>Gets if the item uses ammo to function.</summary>
		public bool UsesAmmo {
			get { return ammo != null; }
		}

		/// <summary>Gets if this item is the one containing the ammo it uses.</summary>
		public bool IsAmmoContainer {
			get { return ammo != null && maxAmmo != null; }
		}

		/// <summary>Gets if ammo should be increased to the new capacity on level up.</summary>
		public bool IncreaseAmmoOnLevelUp {
			get { return levelUpAmmo; }
			set { levelUpAmmo = value; }
		}
	}
}

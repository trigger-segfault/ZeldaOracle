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

namespace ZeldaOracle.Game.Items {
	public abstract class Item : ISlotItem {

		protected Inventory inventory;
		protected Player player;

		protected string		id;
		protected string[]		name;
		protected string[]		description;
		protected int			level;
		protected int			maxLevel;

		protected int			currentAmmo;
		protected Ammo[]		ammo;
		protected bool			isObtained;
		protected bool			isStolen;

		protected Sprite[]		sprite;
		protected Sprite[]		spriteLight;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		public const int Level1 = 0;
		public const int Level2 = 1;
		public const int Level3 = 2;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		protected Item() {
			this.inventory		= null;
			this.id				= "";
			this.name			= new string[] { "" };
			this.description	= new string[] { "" };
			this.level			= Item.Level1;
			this.maxLevel		= Item.Level1;
			this.currentAmmo	= -1;
			this.ammo			= null;
			this.isObtained		= false;
			this.isStolen		= false;
			this.sprite			= null;
			this.spriteLight	= null;
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Called when the item is added to the inventory list.
		public virtual void OnAdded(Inventory inventory) {
			this.inventory = inventory;
		}

		// Called when the item's level is changed.
		public virtual void OnLevelUp() { }

		// Called when the item has been obtained.
		public virtual void OnObtained() { }

		// Called when the item has been unobtained.
		public virtual void OnUnobtained() { }

		// Called when the item has been stolen.
		public virtual void OnStolen() { }

		// Called when the stolen item has been returned.
		public virtual void OnReturned() { }

		// Draws the item inside the inventory.
		public virtual void DrawSlot(Graphics2D g, Point2I position, bool light) {
			DrawSprite(g, position, light);
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		protected void DrawSprite(Graphics2D g, Point2I position, bool light) {
			g.DrawSprite(light ? spriteLight[level] : sprite[level], position);
		}

		protected void DrawAmmo(Graphics2D g, Point2I position, bool light) {
			g.DrawString(GameData.FONT_SMALL, ammo[currentAmmo].Amount.ToString("00"), position + new Point2I(8, 8), light ? new Color(16, 16, 16) : Color.Black);
		}

		protected void DrawLevel(Graphics2D g, Point2I position, bool light) {
			SpriteSheet sheetMenuSmall = (light ? GameData.SHEET_MENU_SMALL_LIGHT : GameData.SHEET_MENU_SMALL);
			Sprite levelSprite = new Sprite(sheetMenuSmall, 2, 1);
			g.DrawSprite(levelSprite, position + new Point2I(8, 8));
			g.DrawString(GameData.FONT_SMALL, (level + 1).ToString(), position + new Point2I(16, 8), light ? new Color(16, 16, 16) : Color.Black);
		}


		//-----------------------------------------------------------------------------
		// Ammo
		//-----------------------------------------------------------------------------

		public Ammo GetAmmoAt(int index) {
			return ammo[index];
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Gets the player.
		public Player Player {
			get { return player; }
			set { player = value; }
		}

		// Gets the current room control.
		public RoomControl RoomControl {
			get { return player.RoomControl; }
		}

		// Gets the id of the item.
		public string ID {
			get { return id; }
		}

		// Gets the name of the item.
		public virtual string Name {
			get { return name[level]; }
		}

		// Gets the description of the item.
		public virtual string Description {
			get { return description[level]; }
		}

		// Gets the level of the item.
		public int Level {
			get { return level; }
			set {
				if (level != value) {
					level = GMath.Clamp(value, 0, maxLevel);
					OnLevelUp();
				}
			}
		}

		// Gets the highest item level.
		public int MaxLevel {
			get { return maxLevel; }
		}

		// Gets if the item has been obtained.
		public bool IsObtained {
			get { return isObtained; }
			set {
				if (isObtained != value) {
					isObtained = value;
					if (isObtained)
						OnObtained();
					else
						OnUnobtained();
				}
			}
		}

		// Gets if the item has been stolen.
		public bool IsStolen {
			get { return isStolen; }
			set {
				if (isStolen != value) {
					isStolen = value;
					if (isStolen)
						OnStolen();
					else
						OnReturned();
				}
			}
		}

		public int NumAmmos {
			get {
				if (ammo != null)
					return ammo.Length;
				return 0;
			}
		}

		public int CurrentAmmo {
			get { return currentAmmo; }
			set { currentAmmo = GMath.Clamp(value, 0, ammo.Length - 1); }
		}
	}
}

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

namespace ZeldaOracle.Game.Items {
	public abstract class Item : ISlotItem {

		protected Inventory		inventory;

		protected string		id;
		protected string[]		name;
		protected string[]		description;
		protected int			level;
		protected int			maxLevel;

		protected bool			isObtained;
		protected bool			isStolen;

		protected ISprite[]		sprite;


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
			this.isObtained		= false;
			this.isStolen		= false;
			this.sprite			= null;
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
		public virtual void DrawSlot(Graphics2D g, Point2I position) {
			DrawSprite(g, position);
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		protected virtual void DrawSprite(Graphics2D g, Point2I position) {
			g.DrawSprite(sprite[level], position);
		}

		protected virtual void DrawLevel(Graphics2D g, Point2I position) {
			g.DrawSprite(GameData.SPR_HUD_LEVEL, position + new Point2I(8, 8));
			g.DrawString(GameData.FONT_SMALL, (level + 1).ToString(), position + new Point2I(16, 8), EntityColors.Black);
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Gets the player.
		public Player Player {
			get { return inventory.GameControl.Player; }
		}

		// Gets the current room control.
		public RoomControl RoomControl {
			get { return inventory.GameControl.RoomControl; }
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
	}
}

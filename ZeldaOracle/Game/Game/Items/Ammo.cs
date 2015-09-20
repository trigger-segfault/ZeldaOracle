using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control.Menus;

namespace ZeldaOracle.Game.Items {
	public class Ammo : ISlotItem {

		protected string id;
		protected string name;
		protected string description;
		private int amount;
		private int maxAmount;
		private bool isObtained;
		protected bool isStolen;
		protected Sprite sprite;
		protected Sprite spriteLight;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Ammo(string id, string name, string description, Sprite sprite, Sprite spriteLight, int amount, int maxAmount) {
			this.id				= id;
			this.name			= name;
			this.description	= description;
			this.amount			= amount;
			this.maxAmount		= maxAmount;
			this.sprite			= sprite;
			this.spriteLight	= spriteLight;
			this.isObtained		= false;
			this.isStolen		= false;
		}

		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Draws the item inside the inventory.
		public virtual void DrawSlot(Graphics2D g, Point2I position, bool light) {
			g.DrawSprite(light ? spriteLight : sprite, position);
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Gets the id of the ammo.
		public string ID {
			get { return id; }
		}

		// Gets the name of the ammo.
		public string Name {
			get { return name; }
		}

		// Gets the description of the ammo.
		public string Description {
			get { return description; }
		}

		// Gets or sets the current amount of the ammo.
		public int Amount {
			get { return amount; }
			set {
				if (isObtained && !isStolen)
					amount = GMath.Clamp(value, 0, maxAmount);
			}
		}

		// Gets or sets the max amount of the ammo.
		public int MaxAmount {
			get { return maxAmount; }
			set {
				maxAmount = GMath.Max(value, 0);
				if (amount > maxAmount)
					amount = maxAmount;
			}
		}

		// Gets or sets if the ammo has been obtained.
		public bool IsObtained {
			get { return isObtained; }
			set { isObtained = value; }
		}

		// Gets or sets if the ammo has been stolen.
		public bool IsStolen {
			get { return isStolen; }
			set { isStolen = value; }
		}

		// Gets if the ammo is out.
		public bool IsEmpty {
			get { return amount == 0; }
		}

		public Sprite Sprite {
			get { return sprite; }
		}

		public Sprite SpriteLight {
			get { return spriteLight; }
		}
	}
}

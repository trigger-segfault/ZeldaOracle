using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items {
	public class ItemEquipment : Item {

		protected bool isEquipped;
		protected Sprite[] spriteEquipped;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEquipment() {
			isEquipped = false;
			spriteEquipped = null;
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		// Equips the item.
		public void Equip() {
			if (!isEquipped) {
				isEquipped = true;
				OnEquip();
			}
		}

		// Unequips the item.
		public void Unequip() {
			if (isEquipped) {
				isEquipped = false;
				OnEquip();
			}
		}

		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Called when the item is equipped.
		public virtual void OnEquip() { }

		// Called when the item is unequipped.
		public virtual void OnUnequip() { }
		
		// Draws the item inside the inventory.
		protected override void DrawSprite(Graphics2D g, Point2I position, int lightOrDark) {
			Sprite spr = sprite[level];
			if (spriteEquipped != null)
				spr = spriteEquipped[level];
			g.DrawSprite(isEquipped ? spr : sprite[level], lightOrDark, position);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets if the item is equipped or not.
		public bool IsEquipped {
			get { return isEquipped; }
			set { isEquipped = value; }
		}

	}
}

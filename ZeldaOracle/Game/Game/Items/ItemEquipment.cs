using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items {
	public class ItemEquipment : Item {

		protected bool isEquipped;
		protected ISprite[] spriteEquipped;


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
				OnUnequip();
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
			ISprite spr = sprite[level];
			if (inventory.IsWeaponEquipped(this) && spriteEquipped != null)
				spr = spriteEquipped[level];
			g.DrawSprite(spr, lightOrDark, position);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Items {
	public class ItemEquipment : Item {

		protected bool equipped;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEquipment() : base() {
			this.equipped = false;
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		// Equips the item.
		public void Equip() {
			if (!equipped) {
				equipped = true;
				OnEquip();
			}
		}

		// Unequips the item.
		public void Unequip() {
			if (equipped) {
				equipped = false;
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets if the item is equipped or not.
		public bool IsEquipped {
			get { return equipped; }
			set { equipped = value; }
		}

	}
}

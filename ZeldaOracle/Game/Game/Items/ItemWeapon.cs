using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items {

	public abstract class ItemWeapon : ItemEquipment {

		// The flags describing the item.
		protected ItemFlags flags;

		private int equipSlot;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemWeapon() : base() {
			this.flags		= ItemFlags.None;
			this.equipSlot	= 0;
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Called when the item is switched to.
		public virtual void OnStart() { }

		// Called when the item is put away.
		public virtual void OnEnd() { }

		// Immediately interrupt this item (ex: if the player falls in a hole).
		public virtual void Interrupt() { }

		// Called when the items button is down (A or B).
		public virtual void OnButtonDown() {}
		
		// Called when the items button is pressed (A or B).
		public virtual void OnButtonPress() {}
		
		// Update the item.
		public virtual void Update() { }

		// Draws under link's sprite.
		public virtual void DrawUnder() { }

		// Draws over link's sprite.
		public virtual void DrawOver() { }
		
		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		// Gets if the item has the specified flags.
		public bool HasFlag(ItemFlags flags) {
			return this.flags.HasFlag(flags);
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Gets or sets the item flags.
		public ItemFlags Flags {
			get { return flags; }
			set { flags = value; }
		}

		public int CurrentEquipSlot {
			get { return equipSlot; }
			set { equipSlot = value; }
		}
	}
}

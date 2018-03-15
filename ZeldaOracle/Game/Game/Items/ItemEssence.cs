using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Items {
	public class ItemEssence : Item {

		protected int slot;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence(string id) : base(id) {
			this.slot = 0;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int EssenceSlot {
			get { return slot; }
			set { slot = value; }
		}
	}
}

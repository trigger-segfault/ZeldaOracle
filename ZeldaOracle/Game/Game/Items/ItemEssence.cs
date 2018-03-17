using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Items {
	public class ItemEssence : Item {

		private int slot;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence(string id) : base(id) {
			slot = 0;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the item after it's added to the inventory list.</summary>
		protected override void OnInitialize() {
			//slot = ItemData.Properties.Get("slot", 0);
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

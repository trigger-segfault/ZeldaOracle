using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Items {
	public class ItemSecondary : Item {

		private Point2I slot;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSecondary(string id) : base(id) {
			slot = Point2I.Zero;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the item after it's added to the inventory list.</summary>
		protected override void OnInitialize() {
			//slot = ItemData.Properties.Get("slot", Point2I.Zero);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Point2I SecondarySlot {
			get { return slot; }
			set { slot = value; }
		}
	}
}

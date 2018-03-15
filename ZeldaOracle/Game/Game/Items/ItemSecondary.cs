using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Items {
	public class ItemSecondary : Item {

		protected Point2I slot;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSecondary(string id) : base(id) {
			this.slot = Point2I.Zero;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Point2I SecondarySlot {
			get { return slot; }
		}
	}
}

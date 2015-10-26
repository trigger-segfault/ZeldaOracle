using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Items.Drops {

	public class DropChance {

		private int odds;
		private IDropCreator drop;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DropChance(int odds, IDropCreator drop) {
			this.odds = odds;
			this.drop = drop;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Odds {
			get { return odds; }
			set { odds = value; }
		}
		
		public IDropCreator Drop {
			get { return drop; }
			set { drop = value; }
		}
	}
}

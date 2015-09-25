using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Worlds {
	public class Zone {

		private string id;
		private string name;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Zone(string id, string name) {
			this.id = id;
			this.name = name;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string ID {
			get { return id; }
			set { id = value; }
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
	}
}

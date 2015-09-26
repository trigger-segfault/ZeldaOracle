using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Worlds {
	public class Zone {

		private string	id;
		private string	name;
		private int		imageVariantID;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Zone(string id, string name, int imageVariantID) {
			this.id		= id;
			this.name	= name;
			this.imageVariantID = imageVariantID;
		}

		
		//-----------------------------------------------------------------------------
		// Stuff
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
		
		public int ImageVariantID {
			get { return imageVariantID; }
			set { imageVariantID = value; }
		}
	}
}

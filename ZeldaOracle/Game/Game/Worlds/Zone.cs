using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Worlds {
	public class Zone : IIDObject {

		private string		id;
		private string		name;
		private int			imageVariantID;
		private TileData	defaultTileData;
		private bool		isSideScrolling;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Zone(string id, string name, int imageVariantID, TileData defaultTileData) {
			this.id					= id;
			this.name				= name;
			this.imageVariantID		= imageVariantID;
			this.defaultTileData	= defaultTileData;
			this.isSideScrolling	= false;
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
		
		public TileData DefaultTileData {
			get { return defaultTileData; }
			set { defaultTileData = value; }
		}
		
		public bool IsSideScrolling {
			get { return isSideScrolling; }
			set { isSideScrolling = value; }
		}
	}
}

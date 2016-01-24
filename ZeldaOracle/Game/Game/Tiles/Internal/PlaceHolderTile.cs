using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Tiles.Internal {

	// A tile that holds the place of another tile.
	// Example: A tile that spawns with a poof effect will first create a
	// PlaceHolderTile tile that won't allow the actual tile to be spawned
	// more than once.
	public class PlaceHolderTile : Tile {
		
		protected TileDataInstance tile; // The tile we're holding the place for.


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlaceHolderTile(TileDataInstance tile) {
			this.tile = tile;
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		public void SpawnTile() {
			Tile tileObj = Tile.CreateTile(tile);
			RoomControl.RemoveTile(this);
			RoomControl.PlaceTile(tileObj, Location, Layer);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
		}

		public override void Update() {
			base.Update();
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		public override TileDataInstance TileDataOwner {
			get { return tile; }
		}
	}
}

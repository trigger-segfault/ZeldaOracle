using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities {
	public class TileDummy : Entity {

		private Tile tile;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TileDummy(Tile tile) {
			this.tile = tile;
			Position = tile.Position;
			centerOffset = tile.Size * GameSettings.TILE_SIZE / 2;
			Physics.CollisionBox = new Rectangle2F(tile.Size * GameSettings.TILE_SIZE);
			Physics.SoftCollisionBox = Physics.CollisionBox;
		}

	}
}

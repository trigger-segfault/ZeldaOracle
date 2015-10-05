using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileLantern : Tile {

		private bool isLit;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileLantern() {

		}


		//-----------------------------------------------------------------------------
		// Interaction
		//-----------------------------------------------------------------------------

		public void Light() {
			if (!isLit) {
				isLit = true;
				CustomSprite = GameData.ANIM_TILE_LANTERN;
			}
		}

		public void PutOut() {
			if (isLit) {
				isLit = false;
				CustomSprite = GameData.SPR_TILE_LANTERN_UNLIT;
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnSeedHit(Seed seed) {
			if (seed.Type == SeedType.Ember && !isLit) {
				Light();
				seed.Destroy();
			}
		}

		public override void Initialize() {
			isLit = Properties.GetBoolean("lit", false);

			if (isLit) {
				CustomSprite = GameData.ANIM_TILE_LANTERN;
			}
		}
	}
}

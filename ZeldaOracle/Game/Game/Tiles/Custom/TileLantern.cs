using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileLantern : Tile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileLantern() {

		}


		//-----------------------------------------------------------------------------
		// Interaction
		//-----------------------------------------------------------------------------

		public void Light() {
			if (!Properties.GetBoolean("lit", false)) {
				BaseProperties.Set("lit", true);
				Properties.Set("lit", true);
			}
		}

		public void PutOut() {
			if (Properties.GetBoolean("lit", false)) {
				BaseProperties.Set("lit", false);
				Properties.Set("lit", false);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnSeedHit(Seed seed) {
			if (seed.Type == SeedType.Ember && !Properties.GetBoolean("lit", false)) {
				Light();
				seed.Destroy();
			}
		}

		public override void Initialize() {
			
		}
	}
}

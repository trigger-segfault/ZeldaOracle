using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileLantern : Tile, ZeldaAPI.Lantern {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileLantern() {

		}


		//-----------------------------------------------------------------------------
		// Interaction
		//-----------------------------------------------------------------------------
		
		public void Light() {
			Light(Properties.GetBoolean("stay_lit", false));
		}
		
		public void PutOut() {
			PutOut(Properties.GetBoolean("stay_lit", false));
		}

		public void Light(bool stayLit) {
			if (!IsLit) {
				IsLit = true;
				CustomSprite = GameData.ANIM_TILE_LANTERN;
				GameControl.ExecuteScript(Properties.GetString("event_light", ""), this);
			}
			
			if (stayLit)
				Properties.SetBase("lit", true);
		}

		public void PutOut(bool stayLit) {
			if (IsLit) {
				IsLit = false;
				CustomSprite = GameData.SPR_TILE_LANTERN_UNLIT;
				GameControl.ExecuteScript(Properties.GetString("event_put_out", ""), this);
			}

			if (stayLit)
				Properties.SetBase("lit", false);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnSeedHit(SeedType seedType, SeedEntity seed) {
			if (seedType == SeedType.Ember && !Properties.GetBoolean("lit", false)) {
				Light();
				seed.Destroy();
			}
		}

		public override void OnInitialize() {
			if (IsLit)
				CustomSprite = GameData.ANIM_TILE_LANTERN;
			else
				CustomSprite = GameData.SPR_TILE_LANTERN_UNLIT;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsLit {
			get { return Properties.GetBoolean("lit"); }
			set { Properties.Set("lit", value); }
		}
	}
}

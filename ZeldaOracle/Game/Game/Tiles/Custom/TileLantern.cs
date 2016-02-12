using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;

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
			Light(false);
		}
		
		public void PutOut() {
			PutOut(false);
		}

		public void Light(bool stayLit) {
			if (!IsLit) {
				IsLit = true;
				//Graphics.PlaySprite(GameData.ANIM_TILE_LANTERN);
				Graphics.PlaySprite(SpriteList[0]);
				Properties.Set("lit", true);
				GameControl.ExecuteScript(Properties.GetString("event_light", ""), this);
			}
		}

		public void PutOut(bool stayLit) {
			if (IsLit) {
				IsLit = false;
				//Graphics.PlaySprite(GameData.SPR_TILE_LANTERN_UNLIT);
				Graphics.PlaySprite(SpriteList[1]);
				Properties.Set("lit", false);
				GameControl.ExecuteScript(Properties.GetString("event_put_out", ""), this);
			}
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
			Graphics.PlaySprite(SpriteList[IsLit ? 0 : 1]);
			/*if (IsLit)
				Graphics.PlaySprite(GameData.ANIM_TILE_LANTERN);
			else
				Graphics.PlaySprite(GameData.SPR_TILE_LANTERN_UNLIT);*/
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

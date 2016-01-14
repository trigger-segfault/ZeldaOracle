using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Tiles {

	public class TileLever : Tile, ZeldaAPI.Lever {

		private bool isFacingLeft;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileLever() {

		}
		

		//-----------------------------------------------------------------------------
		// Lever Methods
		//-----------------------------------------------------------------------------

		public void Toggle() {
			isFacingLeft = !isFacingLeft;
			if (isFacingLeft)
				CustomSprite = GameData.SPR_TILE_LEVER_LEFT;
			else
				CustomSprite = GameData.SPR_TILE_LEVER_RIGHT;
			GameControl.FireEvent(this, "event_toggle", this);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnSwordHit(ItemWeapon swordItem) {
			Toggle();
		}

		public override void OnBoomerang() {
			Toggle();
		}

		public override void OnBombExplode() {
			Toggle();
		}

		public override void OnSeedHit(SeedType type, SeedEntity seed) {
			seed.DestroyWithVisualEffect(type, seed.Position);
			Toggle();
		}

		// TODO: on hit swtich hook.
		// TODO: On hit projectiles: arrows, magic rod fire

		public override void OnInitialize() {
			isFacingLeft = true;
			if (isFacingLeft)
				CustomSprite = GameData.SPR_TILE_LEVER_LEFT;
			else
				CustomSprite = GameData.SPR_TILE_LEVER_RIGHT;
		}

		public override void Update() {
			base.Update();

		}
	}
}

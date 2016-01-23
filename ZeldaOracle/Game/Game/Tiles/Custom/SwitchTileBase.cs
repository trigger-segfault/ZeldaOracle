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

	public class SwitchTileBase : Tile {

		private bool switchState;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SwitchTileBase() {
		}
		

		//-----------------------------------------------------------------------------
		// Switch Methods
		//-----------------------------------------------------------------------------

		public virtual void OnToggle(bool switchState) {}
		
		public bool Toggle() {
			bool switchOnce = Properties.Get("switch_once", false);
			bool hasSwitched = Properties.Get("has_switched", false);
			
			if (!switchOnce || !hasSwitched) {
				switchState = !switchState;

				OnToggle(switchState);
				GameControl.FireEvent(this, "event_toggle", this);

				if (Properties.Get("remember_state", false)) {
					Properties.SetBase("switch_state", switchState);
					Properties.SetBase("has_switched", true);
				}
				else {
					Properties.Set("switch_state", switchState);
					Properties.Set("has_switched", true);
				}
				return true;
			}
			return false;
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

		public override bool OnDig(int direction) {
			Toggle();
			return false;
		}

		public override void OnHitByProjectile(Projectile projectile) {
			if (projectile is SeedEntity) {
				if (Toggle())
					(projectile as SeedEntity).DestroyWithAbsorbedEffect(false);
			}
			if (projectile is SwitchHookProjectile) {
				if (Toggle())
					(projectile as SwitchHookProjectile).Intercept();
			}
			else if (projectile is MagicRodFire) {
				if (Toggle())
					(projectile as MagicRodFire).InterceptAndAbsorb();
			}
			else if (projectile is Arrow) {
				if (Toggle())
					(projectile as Arrow).Intercept();
			}
			else if (projectile is SwordBeam) {
				if (Toggle())
					(projectile as SwordBeam).Intercept();
			}
		}

		public override void OnInitialize() {
			switchState = Properties.Get("switch_state", false);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool SwitchState {
			get { return switchState; }
		}
	}
}

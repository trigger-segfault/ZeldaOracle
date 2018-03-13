using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;
using ZeldaOracle.Game.Items;

namespace ZeldaOracle.Game.Tiles {

	public class SwitchTileBase : Tile {

		private bool switchState;
		private int toggleDelayTimer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SwitchTileBase() {
		}
		

		//-----------------------------------------------------------------------------
		// Switch Methods
		//-----------------------------------------------------------------------------

		public virtual void OnToggle(bool switchState) {}
		
		public virtual void SetSwitchState(bool switchState) {
			this.switchState = switchState;
		}

		public void Flip() {
			Toggle();
		}

		public bool Toggle() {
			if (toggleDelayTimer > 0)
				return false;
			toggleDelayTimer = 8;

			bool switchOnce = Properties.Get("switch_once", false);
			bool hasSwitched = Properties.Get("has_switched", false);
			
			if (!switchOnce || !hasSwitched) {
				switchState = !switchState;

				SetSwitchState(switchState);
				OnToggle(switchState);
				GameControl.FireEvent(this, "toggle", this);
				
				Properties.Set("switch_state", switchState);
				Properties.Set("has_switched", true);
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

		public override void OnBombExplode() {
			Toggle();
		}

		public override bool OnDig(Direction direction) {
			Toggle();
			return false;
		}

		public override void OnHitByThrownObject(CarriedTile thrownObject) {
			Toggle();
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
			else if (projectile is PlayerBoomerang) {
				if (Toggle())
					(projectile as PlayerBoomerang).Intercept();
			}
		}

		public override void OnInitialize() {
			switchState = Properties.Get("switch_state", false);
			SetSwitchState(switchState);
			toggleDelayTimer = 0;
		}

		public override void Update() {
			if (toggleDelayTimer > 0)
				toggleDelayTimer--;
			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool SwitchState {
			get { return switchState; }
		}
	}
}

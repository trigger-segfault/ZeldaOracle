using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSwitchHookState : PlayerState {

		private SwitchHookProjectile hookProjectile;
		private bool isSwitching;
		private int direction;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwitchHookState() {
			StateParameters.ProhibitJumping			= true;
			StateParameters.ProhibitMovementControl	= true;
		}


		//-----------------------------------------------------------------------------
		// Switching
		//-----------------------------------------------------------------------------

		public void BeginSwitch(object hookedObject) {
			isSwitching = true;
			player.SwitchHookSwitchState.SetupSwitch(
				hookedObject, hookProjectile, direction);
			End();
			player.BeginControlState(player.SwitchHookSwitchState);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			isSwitching = false;
			direction = player.Direction;

			if (player.RoomControl.IsUnderwater)
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_MERMAID_THROW);
			else
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
		}
		
		public override void OnEnd(PlayerState newState) {
			// Destroy the hook projectile
			if (!isSwitching && hookProjectile != null && !hookProjectile.IsDestroyed)
				hookProjectile.Destroy();
		}

		public override void OnHurt(DamageInfo damage) {
			// Getting hurt will cancel the switch hook state, and destroy the hook
			// projectile
			End();
		}

		public override void Update() {
			// NOTE: the switch hook projectile will tell this state when to switch

			if (hookProjectile == null || hookProjectile.IsDestroyed)
				End();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SwitchHookProjectile Hook {
			get { return hookProjectile; }
			set { hookProjectile = value; }
		}
	}
}

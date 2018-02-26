using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSideScrollSwimEnvironmentState : PlayerEnvironmentState {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSideScrollSwimEnvironmentState() {
			StateParameters.DisableGravity				= true;
			StateParameters.EnableGroundOverride		= true;
			StateParameters.AlwaysFaceLeftOrRight		= true;
			StateParameters.ProhibitJumping				= true;
			StateParameters.ProhibitPushing				= true;
			StateParameters.DisableAnimationPauseWhenNotMoving = true;

			PlayerAnimations.Default		= GameData.ANIM_PLAYER_MERMAID_SWIM;
			PlayerAnimations.Aim			= GameData.ANIM_PLAYER_MERMAID_AIM;
			PlayerAnimations.Throw			= GameData.ANIM_PLAYER_MERMAID_THROW;
			PlayerAnimations.Swing			= GameData.ANIM_PLAYER_MERMAID_SWING;
			//PlayerAnimations.SwingBig		= GameData.ANIM_PLAYER_MERMAID_SWING_BIG; // TODO
			PlayerAnimations.SwingNoLunge	= GameData.ANIM_PLAYER_MERMAID_SWING; // TODO
			PlayerAnimations.Spin			= GameData.ANIM_PLAYER_MERMAID_SPIN;
			PlayerAnimations.Stab			= GameData.ANIM_PLAYER_MERMAID_STAB;
			
			MotionSettings.MovementSpeed		= 0.5f;
			MotionSettings.IsSlippery			= true;
			MotionSettings.Acceleration			= 0.08f;
			MotionSettings.Deceleration			= 0.05f;
			MotionSettings.MinSpeed				= 0.05f;
			MotionSettings.DirectionSnapCount	= 32;
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void CreateSplashEffect() {
			Effect splash = new Effect(GameData.ANIM_EFFECT_WATER_SPLASH,
				DepthLayer.EffectSplash, true);
			splash.Position = player.Center + new Vector2F(0, 4);
			player.RoomControl.SpawnEntity(splash);
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
		}

		/// <summary>Returns true if the player has the capability to swim in the
		/// liquid he is currently in.</summary>
		private bool CanSwimInCurrentLiquid() {
			if (player.Physics.IsInLava && !player.RoomControl.IsSideScrolling)
				return player.SwimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInLava);
			else if (player.Physics.IsInOcean)
				return player.SwimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInOcean);
			else if (player.Physics.IsInWater)
				return player.SwimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInWater);
			else
				return false;
		}

		/// <summary>Make the player die by drowning.</summary>
		private void Drown() {
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DROWN);

			if (player.Physics.IsInLava)
				player.Graphics.IsHurting = true;

			player.RespawnDeath();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.InterruptWeapons();
			//CreateSplashEffect(); // Now called in Entity.OnFallInSideScrollWater()
			if (!CanSwimInCurrentLiquid()) {
				Drown();
				// TODO: Cancel the hurt animation if the player was knocked in.
				//player.InvincibleTimer = 0;
				//player.Graphics.IsHurting = false;
			}
		}
		
		public override void OnEnd(PlayerState newState) {
			// Jump out of the water, and create a splash effect
			if (newState != player.SideScrollLadderState) {
				player.Physics.ZVelocity = 1.5f;
				CreateSplashEffect();
			}
		}
	}
}

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSprintState : PlayerState {

		private int timer;
		private int duration;
		private float movementSpeedScale;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSprintState(int duration, float movementSpeedScale) {
			this.duration = duration;
			this.movementSpeedScale = movementSpeedScale;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			timer = 0;
			
			StateParameters.DisableAnimationPauseWhenNotMoving = true;

			if (!player.IsSwimming)
				StateParameters.MovementSpeedScale = movementSpeedScale;
			else
				StateParameters.MovementSpeedScale = 1.0f;
		}

		public override void Update() {
			timer++;

			if (!player.IsSwimming) {
				StateParameters.MovementSpeedScale = movementSpeedScale;

				// Spawn the dust particles
				if (player.IsOnGround &&
					timer % GameSettings.PLAYER_SPRINT_EFFECT_INTERVAL == 0)
				{
					AudioSystem.PlaySound(GameData.SOUND_PLAYER_LAND);
					Effect dustParticle = new Effect(GameData.ANIM_EFFECT_SPRINT_PUFF,
						DepthLayer.EffectSprintDustParticle, true);
					player.RoomControl.SpawnEntity(dustParticle, player.Position);
				}
			}
			else {
				StateParameters.MovementSpeedScale = 1.0f;
			}

			// Check if the time is up
			if (timer >= duration)
				End();
		}
	}
}

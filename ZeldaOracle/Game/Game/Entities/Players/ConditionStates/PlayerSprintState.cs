using System.Collections.Generic;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;

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
			
			if (!player.IsSwimming && !player.IsInAir)
				StateParameters.MovementSpeedScale = movementSpeedScale;
			else
				StateParameters.MovementSpeedScale = 1.0f;
		}

		public override void Update() {
			timer++;

			if (!player.IsSwimming && !player.IsInAir)
				StateParameters.MovementSpeedScale = movementSpeedScale;
			else
				StateParameters.MovementSpeedScale = 1.0f;
			
			if (!player.IsSwimming) {
				// Spawn the dust particles
				if (timer % GameSettings.PLAYER_SPRINT_EFFECT_INTERVAL == 0 &&
					player.IsOnGround && !player.IsSwimming)
				{
					AudioSystem.PlaySound(GameData.SOUND_PLAYER_LAND);
					Effect dustParticle = new Effect(GameData.ANIM_EFFECT_SPRINT_PUFF,
						DepthLayer.EffectSprintDustParticle, true);
					player.RoomControl.SpawnEntity(dustParticle, player.Position);
				}
			}

			if (timer >= duration) {
				End();
			}
		}
	}
}

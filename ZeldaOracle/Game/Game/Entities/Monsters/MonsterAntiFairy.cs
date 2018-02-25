using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterAntiFairy : Monster {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public MonsterAntiFairy() {
			// General
			ContactDamage	= 2;
			Color			= MonsterColor.Gold;
			isDamageable	= false;
			isKnockbackable	= false;
			isGaleable		= false;
			isBurnable		= false;
			isStunnable		= false;

			// Physics
			Physics.Gravity					= 0.0f;
			Physics.ReboundRoomEdge			= true;
			Physics.ReboundSolid			= true;
			physics.DisableSurfaceContact	= true;

			// Projectile interations
			SetReaction(InteractionType.Boomerang, SenderReactions.Intercept, Reactions.SoftKill);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			Graphics.PlayAnimation(GameData.ANIM_MONSTER_ANTI_FAIRY);

			// Start moving in a random diagonal angle
			int angle = ((GRandom.NextInt(4) * 2) + 1) % 8;
			Physics.Velocity = Angles.ToVector(angle) * 
				GameSettings.MONSTER_ANTI_FAIRY_MOVE_SPEED;
		}

		public override void CreateDeathEffect() {
			// Create vanish effect
			Effect explosion = new Effect(
				GameData.ANIM_EFFECT_BLOCK_POOF,
				DepthLayer.EffectMonsterExplosion);
			AudioSystem.PlaySound(GameData.SOUND_APPEAR_VANISH);
			RoomControl.SpawnEntity(explosion, Center);
		}

		public override void UpdateAI() {
			// Just in case we stopped moving, start moving again
			if (Physics.Velocity.Length <= 0.1f) {
				int angle = ((GRandom.NextInt(4) * 2) + 1) % 8;
				Physics.Velocity = Angles.ToVector(angle) * 
					GameSettings.MONSTER_ANTI_FAIRY_MOVE_SPEED;
			}
		}
	}
}

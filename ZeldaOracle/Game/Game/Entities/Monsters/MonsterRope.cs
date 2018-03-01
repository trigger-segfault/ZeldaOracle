using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	public class MonsterRope : BasicMonster {
		public MonsterRope() {
			// General.
			MaxHealth		= 1;
			ContactDamage	= 1;
			Color			= MonsterColor.Green;

			// Movement.
			moveSpeed					= 0.375f;
			changeDirectionsOnCollide	= true;
			movesInAir					= false;
			stopTime.Set(0, 0);
			moveTime.Set(50, 80);
			
			// Physics.
			Physics.Bounces				= true; // This is here for when the monster is digged up or dropped from the ceiling.
			
			// Graphics.
			animationMove				= GameData.ANIM_MONSTER_ROPE;
			isAnimationHorizontal		= true;
			syncAnimationWithDirection	= true;
			
			// Charging.
			chargeType			= ChargeType.ChargeUntilCollision;
			chargeSpeed			= 1.25f;
			chargeAcceleration	= 1.25f;
			chargeMinAlignment	= 5;

			// Interactions.
			Interactions.SetReaction(InteractionType.SwitchHook, SenderReactions.Intercept, Reactions.Damage);
		}
	}
}

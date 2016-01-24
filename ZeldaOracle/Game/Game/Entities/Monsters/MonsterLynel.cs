using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.MagicProjectiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterLynel : BasicMonster {

		public MonsterLynel() {
			// General.
			MaxHealth		= 4;
			ContactDamage	= 4;
			color			= MonsterColor.Red;

			// Movement.
			moveSpeed					= 0.5f;
			changeDirectionsOnCollide	= true;
			stopTime.Set(0, 0);
			moveTime.Set(40, 80);
			
			// Charging.
			/*chargeType			= ChargeType.ChargeForDuration;
			chargeSpeed			= 1.25f;
			chargeAcceleration	= 0.1f;
			chargeMinAlignment	= 5;
			chargeDuration.Set(100, 160);*/
			
			// Projectiles.
			projectileType		= typeof(SpearProjectile);
			shootType			= ShootType.OnStop;
			aimType				= AimType.FacePlayer;
			shootSpeed			= 3.0f;
			projectileShootOdds	= 10;
			shootPauseDuration	= 15;

			// Physics.
			Physics.Bounces			= true; // This is here for when the monster is digged up or dropped from the ceiling.
						
			// Graphics.
			animationMove				= GameData.ANIM_MONSTER_LYNEL;
			syncAnimationWithDirection	= true;
			
			// Interactions.
			SetReaction(InteractionType.Sword,			Reactions.DamageByLevel(1, 2, 2));
			SetReaction(InteractionType.BiggoronSword,	Reactions.Damage2);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.ClingEffect);
			SetReaction(InteractionType.GaleSeed,		Reactions.None);
		}
	}
}

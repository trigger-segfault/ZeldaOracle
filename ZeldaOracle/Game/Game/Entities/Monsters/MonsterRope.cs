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
			color			= MonsterColor.Green;

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
			
			// Interactions.
			SetReaction(InteractionType.SwitchHook, SenderReactions.Intercept, Reactions.Damage);
		}
	}

	public class MonsterMoblin : BasicMonster {
		public MonsterMoblin() {
			// General.
			MaxHealth		= 2;
			ContactDamage	= 2;
			color			= MonsterColor.Red;
			animationMove	= GameData.ANIM_MONSTER_MOBLIN;
			
			// Movement.
			moveSpeed					= 0.5f;
			changeDirectionsOnCollide	= true;
			syncAnimationWithDirection	= true;
			stopTime.Set(0, 15);
			moveTime.Set(50, 100);

			// Projectiles.
			projectileType		= typeof(MonsterArrow);
			shootType			= ShootType.OnStop;
			aimType				= AimType.FacePlayer;
			shootSpeed			= 2.0f;
			projectileShootOdds	= 5; // 1 in 5 chance to shoot between movements.
			shootPauseDuration	= 5;
		}

		public override void Initialize() {
			base.Initialize();

			if (color == MonsterColor.Red) {
				MaxHealth = 2;
			}
			else if (color == MonsterColor.Blue) {
				MaxHealth = 3;
			}
		}
	}

	public class MonsterGibdo : BasicMonster {
		public MonsterGibdo() {
			MaxHealth		= 4;
			ContactDamage	= 2;
			color			= MonsterColor.Red;
			animationMove	= GameData.ANIM_MONSTER_GIBDO;
			IsKnockbackable	= false;
			moveSpeed		= 0.375f;
			syncAnimationWithDirection = false;
			stopTime.Set(0, 0);
			moveTime.Set(50, 90);
		}
	}

	public class MonsterSandCrab : BasicMonster {
		public MonsterSandCrab() {
			MaxHealth		= 4;
			ContactDamage	= 2;
			color			= MonsterColor.Orange;
			animationMove	= GameData.ANIM_MONSTER_SAND_CRAB;
			moveSpeed		= 0.375f;
			syncAnimationWithDirection = false;
			stopTime.Set(0, 0);
			moveTime.Set(40, 80);
		}

		public override void UpdateAI() {
			if (isMoving)
				speed = (Directions.IsHorizontal(direction) ? 1.25f : 0.25f);
			base.UpdateAI();
		}
	}

	public class MonsterBeetle : BasicMonster {
		public MonsterBeetle() {
			// General.
			MaxHealth		= 1;
			ContactDamage	= 1;
			color			= MonsterColor.Green;
			
			// Movement.
			moveSpeed					= 0.5f;
			//numMoveAngles				= 8;
			//isMovementDirectionBased	= true;
			changeDirectionsOnCollide	= true;
			movesInAir					= false;
			stopTime.Set(0, 0);
			moveTime.Set(50, 80);
								
			// Physics.
			Physics.Bounces			= true; // This is here for when the monster is digged up or dropped from the ceiling.
			Physics.ReboundRoomEdge	= true;
			Physics.ReboundSolid	= true;
			
			// Graphics.
			animationMove				= GameData.ANIM_MONSTER_BEETLE;
			Graphics.DrawOffset			= new Point2I(-8, -14) + new Point2I(0, 3);
			centerOffset				= new Point2I(0, -6) + new Point2I(0, 3);
			syncAnimationWithDirection	= true;

			// Interactions.
			SetReaction(InteractionType.SwitchHook, SenderReactions.Intercept, Reactions.Damage);
		}
	}
	
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
			SetReaction(InteractionType.Sword, Reactions.DamageByLevel(1, 2, 2));
			SetReaction(InteractionType.BiggoronSword, Reactions.Damage2);
			SetReaction(InteractionType.SwitchHook, SenderReactions.Intercept,
				Reactions.ContactEffect(new Effect(GameData.ANIM_EFFECT_CLING, DepthLayer.EffectCling)));
			SetReaction(InteractionType.GaleSeed, Reactions.None);
		}
	}
}

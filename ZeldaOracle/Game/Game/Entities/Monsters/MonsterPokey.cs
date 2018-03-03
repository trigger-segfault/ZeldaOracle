using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterPokey : BasicMonster {
		
		private int bodyHeight;
		private List<AnimationPlayer> bodyAnimationPlayers;
		private Animation[] bodyAnimations;
		private Animation[] headAnimations;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterPokey() {
			// General
			MaxHealth		= 3;
			ContactDamage	= 2;
			Color			= MonsterColor.Green;
			isBurnable		= false;
			isKnockbackable	= false;

			// Movement
			moveSpeed					= 0.25f;
			numMoveAngles				= 8;
			changeDirectionsOnCollide	= false;
			movesInAir					= false;
			facePlayerOdds				= 0;
			facePlayerOdds				= 0;
			stopTime.Set(0, 0);
			moveTime.Set(50, 80);
			
			// Physics
			Physics.Bounces			= true; // This is here for when the monster is digged up or dropped from the ceiling.
			Physics.ReboundRoomEdge	= true;
			Physics.ReboundSolid	= true;
			
			// Graphics
			animationMove				= GameData.ANIM_MONSTER_POKEY_BODY;
			syncAnimationWithDirection	= false;
			//animationMove				= GameData.ANIM_MONSTER_BEAMOS;
			//syncAnimationWithDirection	= true;

			bodyAnimations = new Animation[] {
				GameData.ANIM_MONSTER_POKEY_BODY,
				GameData.ANIM_MONSTER_POKEY_BODY_WIGGLE_SLOW,
				GameData.ANIM_MONSTER_POKEY_BODY_WIGGLE_FAST,
			};
			headAnimations = new Animation[] {
				GameData.ANIM_MONSTER_POKEY_HEAD,
				GameData.ANIM_MONSTER_POKEY_HEAD_WIGGLE_SLOW,
				GameData.ANIM_MONSTER_POKEY_HEAD_WIGGLE_FAST,
			};

			// Weapon interactions
			//Interactions.SetReaction(InteractionType.Sword, DamageBody);
			//Interactions.SetReaction(InteractionType.SwordSpin, DamageBody);
			//Interactions.SetReaction(InteractionType.BiggoronSword, DamageBody);
			////// Projectile interactions
			//Interactions.SetReaction(InteractionType.Arrow, SenderReactions.Intercept, DamageBody);
			//Interactions.SetReaction(InteractionType.SwordBeam, SenderReactions.Intercept, DamageBody);
			//Interactions.SetReaction(InteractionType.SwitchHook, SenderReactions.Intercept, DamageBody);
			//Interactions.SetReaction(InteractionType.BombExplosion, DamageBody);
			
			// Weapon interactions
			Interactions.SetReaction(InteractionType.Sword,			MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwordSpin,		MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.Kill);
			//// Projectile interactions
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.BombExplosion,	MonsterReactions.Kill);
		}


		//-----------------------------------------------------------------------------
		// Reactions
		//-----------------------------------------------------------------------------

		//public override void OnHurt(DamageInfo damage) {
		//	base.OnHurt(damage);

		//	if (bodyHeight == 1) {
		//		Kill();
		//	}
		//	else {
		//		CreateDeathEffect();
		//		zPosition += GameSettings.MONSTER_POKEY_BODY_SEPARATION;
		//		bodyHeight--;
		//		//Hurt(new DamageInfo(0));
		//	}
		//}

		public override void Die() {
			if (bodyHeight == 1) {
				base.Die();
			}
			else {
				CreateDeathEffect();
				zPosition += GameSettings.MONSTER_POKEY_BODY_SEPARATION;
				bodyHeight--;
				
				for (int i = 0; i < bodyHeight; i++) {
					if (i < bodyHeight - 1)
						bodyAnimationPlayers[i].Play(bodyAnimations[i]);
					else
						bodyAnimationPlayers[i].Play(headAnimations[i]);
				}
				if (bodyHeight == 1) {
					Graphics.PlayAnimation(headAnimations[0]);
					animationMove = headAnimations[0];
				}
				Health = MaxHealth;

				DamageInfo damage = new DamageInfo(0) {
					InvincibleDuration = 30,
				};
				//Hurt(damage);
			}
		}
		/*
		private void DamageBody(Entity sender, EventArgs args) {
			if (IsInvincible || IsBeingKnockedBack)
				return;

			if (bodyHeight == 1) {
				Kill();
			}
			else {
				CreateDeathEffect();
				zPosition += GameSettings.MONSTER_POKEY_BODY_SEPARATION;
				bodyHeight--;
				
				for (int i = 0; i < bodyHeight; i++) {
					if (i < bodyHeight - 1)
						bodyAnimationPlayers[i].Play(bodyAnimations[i]);
					else
						bodyAnimationPlayers[i].Play(headAnimations[i]);
				}
				if (bodyHeight == 1) {
					Graphics.PlayAnimation(headAnimations[0]);
					animationMove = headAnimations[0];
				}
				Health = MaxHealth;

				DamageInfo damage = new DamageInfo(0) {
					InvincibleDuration = 30,
				};
				Hurt(damage);
			}
		}*/


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			bodyHeight = 3;

			bodyAnimationPlayers = new List<AnimationPlayer>();
			bodyAnimationPlayers.Add(new AnimationPlayer());
			bodyAnimationPlayers.Add(new AnimationPlayer());
			bodyAnimationPlayers.Add(new AnimationPlayer());
			bodyAnimationPlayers[0].Play(bodyAnimations[0]);
			bodyAnimationPlayers[1].Play(bodyAnimations[1]);
			bodyAnimationPlayers[2].Play(headAnimations[2]);
		}

		public override void UpdateAI() {
			base.UpdateAI();

			foreach (AnimationPlayer anim in bodyAnimationPlayers)
				anim.Update();
			//Graphics.DrawOffset = new Point2I(0, -(bodyHeight - 1) * 12);
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);
			SpriteSettings drawSettings = new SpriteSettings() {
				Colors = Graphics.ModifiedColorDefinitions
			};
			for (int i = 1; i < bodyHeight; i++) {
				
				Vector2F drawPosition = position;
				drawPosition.Y -= zPosition;
				drawPosition.Y -=  i * GameSettings.MONSTER_POKEY_BODY_SEPARATION;
				g.DrawSprite(bodyAnimationPlayers[i].SpriteOrSubStrip, new SpriteSettings(
					Graphics.ModifiedColorDefinitions, bodyAnimationPlayers[i].PlaybackTime),
					drawPosition + Graphics.DrawOffset, DepthLayer.InAirCollectibles, position);
			}
		}
	}
}

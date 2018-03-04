using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters.Tools;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterSpikedBeetle : BasicMonster {

		private bool isFlipped;
		private int flipTimer;
		private Vector2F flipKnockbackVector;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterSpikedBeetle() {
			// General.
			MaxHealth		= 1;
			ContactDamage	= 2;
			Color			= MonsterColor.Blue;
			animationMove	= GameData.ANIM_MONSTER_SPIKED_BEETLE;
			
			// Movement.
			moveSpeed					= 0.375f;
			changeDirectionsOnCollide	= true;
			syncAnimationWithDirection	= false;
			stopTime.Set(0, 0);
			moveTime.Set(40, 80);
			
			// Charging.
			chargeType			= ChargeType.ChargeUntilCollision;
			chargeSpeed			= 1.5f;
			chargeAcceleration	= 0.06f;
			chargeMinAlignment	= 5;
			
			InitReactionsNotFlipped();
		}


		//-----------------------------------------------------------------------------
		// Flipping Methods
		//-----------------------------------------------------------------------------

		private void FlipOver(Vector2F contactPosition) {
			Physics.ZVelocity = 2.0f;
			flipTimer = 0;
			isFlipped = true;

			flipKnockbackVector = Center - contactPosition;

			animationMove = GameData.ANIM_MONSTER_SPIKED_BEETLE_FLIPPED;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_SPIKED_BEETLE_FLIPPED);
			InitReactionsFlipped();

			Knockback(bumpKnockbackDuration, 1.1f, contactPosition);
		}
		
		private void FlipBack() {
			Knockback(bumpKnockbackDuration, 0.75f, Center + flipKnockbackVector);

			Physics.ZVelocity = 2.0f;
			isFlipped = false;
			animationMove = GameData.ANIM_MONSTER_SPIKED_BEETLE;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_SPIKED_BEETLE);
			InitReactionsNotFlipped();
		}

		private void InitReactionsNotFlipped() {
			isDamageable	= false;
			isBurnable		= false;
			isGaleable		= false;
			IsKnockbackable	= false;
			Physics.Bounces	= false;

			SetDefaultReactions();
			
			// Weapon Reactions
			Reactions[InteractionType.Sword]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordStrafe]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordSpin]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.BiggoronSword]
				.SetBegin(MonsterReactions.ClingEffect);
			Reactions[InteractionType.Shield]
				.SetBegin(OnShieldHit);
			Reactions[InteractionType.Shovel]
				.SetBegin(OnShieldHit);
			// Projectile Reactions
			Reactions[InteractionType.Arrow]
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.SwordBeam]
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.Boomerang]
				.Set(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwitchHook]
				.Set(MonsterReactions.ParryWithClingEffect);
		}

		private void InitReactionsFlipped() {
			isDamageable	= false; // Becomes damageable upon landing.
			isBurnable		= true;
			isGaleable		= true;
			IsKnockbackable	= true;
			Physics.Bounces	= true;

			SetDefaultReactions();
			Reactions[InteractionType.SwitchHook]
				.Set(SenderReactions.Intercept)
				.Add(MonsterReactions.Kill);
		}
		
		private void OnShieldHit(Entity sender, EventArgs args) {
			Player player = (sender as Player);
			FlipOver(player.Center);
			player.Bump(Center);
			AudioSystem.PlaySound(GameData.SOUND_BOMB_BOUNCE);
		}
		
		private void OnShovelHit(Entity sender, EventArgs args) {
			Player player = (sender as Player);
			FlipOver(player.Center);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			isFlipped = false;
		}

		public override void OnLand() {
			base.OnLand();
			if (isFlipped && IsOnGround) {
				Physics.Velocity = Vector2F.Zero;
				isDamageable = true;
			}
		}

		public override void UpdateAI() {
			if (isFlipped) {
				if (IsOnGround) {
					flipTimer++;
					if (flipTimer > 60) {
						FlipBack();
					}
				}
			}
			else  {
				base.UpdateAI();
			}
		}
	}
}

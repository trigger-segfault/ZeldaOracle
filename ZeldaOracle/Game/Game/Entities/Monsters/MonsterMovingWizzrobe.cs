using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Monsters.States;
using ZeldaOracle.Game.Entities.Projectiles.MagicProjectiles;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterMovingWizzrobe : BasicMonster {

		private enum WizzrobeState {
			Phased,
			Paused,
			Solid,
		}

		private float timer;
		private float shootTimer;
		private WizzrobeState wizzrobeState;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MonsterMovingWizzrobe() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 1;
			color			= MonsterColor.Blue;
			
			// Movement
			moveSpeed = 0.5f;
			isFlying = true;
			facePlayerOdds = 2;
			stopTime.Set(0, 0);
			moveTime.Set(50, 90);

			projectileType = typeof(MagicProjectile);
			shootSpeed = GameSettings.MONSTER_WIZZROBE_SHOOT_SPEED;

			// Graphics
			syncAnimationWithDirection = true;
			animationMove = GameData.ANIM_MONSTER_WIZZROBE;

			// Physics
			Physics.CollideWithWorld = false;
			
			// Projectile interactions
			SetReaction(InteractionType.Boomerang, SenderReactions.Intercept);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			MaxHealth		= 2;
			ContactDamage	= 2;
			moveSpeed		= 0.5f;
			
			Health = MaxHealth;

			wizzrobeState = WizzrobeState.Solid;
			timer = 120;
		}

		public override void UpdateAI() {

			bool isMeetingSolid = Physics.IsPlaceMeetingSolid(position);

			if (wizzrobeState == WizzrobeState.Phased) {
				Graphics.IsFlickering = true;
				IsPassable = true;

				timer--;
				if (timer <= 0 && !isMeetingSolid) {
					timer = 0;
					wizzrobeState = WizzrobeState.Paused;
					numMoveAngles = 4;
					facePlayerOdds = 1;
					StartMoving();
					Physics.Velocity = Vector2F.Zero;
					return;
				}

				base.UpdateAI();
			}
			else if (wizzrobeState == WizzrobeState.Paused) {
				Graphics.IsFlickering = true;
				IsPassable = false;
				Physics.Velocity = Vector2F.Zero;

				timer++;
				if (timer >= GameSettings.MONSTER_WIZZROBE_BLUE_MOVE_DELAY) {
					timer = 100;
					wizzrobeState = WizzrobeState.Solid;
					shootTimer = 0;
				}

				base.UpdateAI();
			}
			else if (wizzrobeState == WizzrobeState.Solid) {
				Graphics.IsFlickering = false;
				IsPassable = false;

				shootTimer++;
				if (shootTimer > 30) {
					Shoot();
					shootTimer = 0;
				}

				if (isMeetingSolid) {
					timer = 30;
					wizzrobeState = WizzrobeState.Phased;
					facePlayerOdds = 0;
					int prevMoveAngle = MoveAngle;
					StartMoving();
					numMoveAngles = 16;
					MoveAngle = (prevMoveAngle * 4) + GRandom.NextInt(-2, 2);
				}

				base.UpdateAI();
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerMagicBoomerangState : PlayerState {

		private const int SHOOT_WAIT_TIME = 12;

		// The boomerang item.
		private ItemBoomerang weapon;
		// The boomerang entity.
		private Boomerang boomerang;

		private float boomerangMotionDirection;



		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerMagicBoomerangState() {
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.Movement.CanJump			= false;
			player.Movement.MoveCondition	= PlayerMoveCondition.NoControl;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
			boomerangMotionDirection = boomerang.Angle * GMath.QuarterAngle * 0.5f;
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.CanJump			= true;
			player.Movement.MoveCondition	= PlayerMoveCondition.FreeMovement;
		}

		public override void Update() {
			base.Update();
			
			// Check if we should stop controlling the boomerang.
			if (!weapon.IsEquipped || !weapon.IsButtonDown() || boomerang.IsDestroyed || boomerang.IsReturning) {
				Player.BeginBusyState(10);
			}
			else {
				// TODO: update move controls.
				
				// Poll movement keys.
				bool isArrowDown = false;
				for (int dir = 0; dir < 4 && !isArrowDown; dir++) {
					if (Controls.Arrows[dir].IsDown())
						isArrowDown = true;
				}

				if (isArrowDown) {
					int useAngle = player.UseAngle;
					float currentAngle	= boomerangMotionDirection;
					float goalAngle		= player.UseAngle * GMath.QuarterAngle * 0.5f;
					float distCW	= GMath.GetAngleDistance(currentAngle, goalAngle, WindingOrder.Clockwise);
					float distCCW	= GMath.GetAngleDistance(currentAngle, goalAngle, WindingOrder.CounterClockwise);

					if (distCW != 0.0f || distCCW != 0.0f) {
						int sign = (distCW <= distCCW ? -1 : 1);
						boomerangMotionDirection += sign * 7f;
						boomerangMotionDirection = GMath.Plusdir(boomerangMotionDirection);
					}

					Vector2F velocity = Vector2F.CreatePolar(boomerang.Speed, boomerangMotionDirection);
					velocity.Y = -velocity.Y;
					boomerang.Physics.Velocity = velocity;
				}

				// Determine direction 

				// Apply motion.
				//Vector2F keyMotion = ((Vector2F) keyMove).Normalized;

				//boomerang.Physics.Velocity += keyMotion;
				//int length = boomerang.Physics.Velocity.Length;

			}
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemBoomerang Weapon {
			get { return weapon; }
			set { weapon = value; }
		}

		public Boomerang BoomerangEntity {
			get { return boomerang; }
			set { boomerang = value; }
		}
	}
}

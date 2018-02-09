﻿using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerMagicBoomerangState : PlayerState {

		private const int SHOOT_WAIT_TIME = 12;

		// The boomerang item
		private ItemBoomerang weapon;
		// The boomerang entity
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
			StateParameters.ProhibitJumping = true;
			StateParameters.ProhibitMovementControlInAir = true;
			StateParameters.ProhibitMovementControlOnGround = true;

			boomerangMotionDirection = boomerang.Angle * GMath.QuarterAngle * 0.5f;
			
			if (player.IsInMinecart)
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_MINECART_THROW);
			else
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
		}

		public override void OnExitMinecart() {
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
		}
		
		public override void Update() {
			base.Update();
			
			// Check if we should stop controlling the boomerang
			if (!weapon.IsEquipped || !weapon.IsButtonDown() ||
				boomerang.IsDestroyed || boomerang.IsReturning)
			{
				stateMachine.BeginState(new PlayerBusyState(
					10, player.Graphics.Animation));
			}
			else {
				// Poll movement keys
				bool isArrowDown = false;
				for (int dir = 0; dir < Directions.Count && !isArrowDown; dir++) {
					if (Controls.Arrows[dir].IsDown())
						isArrowDown = true;
				}

				// Update boomerang motion
				if (isArrowDown) {
					int useAngle = player.UseAngle;
					float currentAngle = boomerangMotionDirection;
					float goalAngle = player.UseAngle * GMath.QuarterAngle * 0.5f;
					float distCW = GMath.GetAngleDistance(
						currentAngle, goalAngle, WindingOrder.Clockwise);
					float distCCW = GMath.GetAngleDistance(
						currentAngle, goalAngle, WindingOrder.CounterClockwise);

					if (distCW != 0.0f || distCCW != 0.0f) {
						int sign = (distCW <= distCCW ? -1 : 1);
						boomerangMotionDirection += sign * 7f;
						boomerangMotionDirection = GMath.Plusdir(boomerangMotionDirection);
					}

					Vector2F velocity = Vector2F.CreatePolar(boomerang.Speed, boomerangMotionDirection);
					velocity.Y = -velocity.Y;
					boomerang.Physics.Velocity = velocity;
				}
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
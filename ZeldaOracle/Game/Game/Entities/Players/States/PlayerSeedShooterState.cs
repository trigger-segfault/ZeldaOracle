﻿using System;
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
	public class PlayerSeedShooterState : PlayerState {

		private const int SHOOT_WAIT_TIME = 12;

		// The angle that the player is aiming in.
		private int angle;
		// The direction the player should return to after shooting.
		private int returnDirection;
		// The seed shooter item.
		private ItemSeedShooter weapon;

		private bool isShooting;

		private int shootTimer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSeedShooterState() {
		}
		

		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void Shoot() {
			Vector2F[] projectilePositions = new Vector2F[] {
				new Vector2F(16, 7),
				new Vector2F(15, -2),
				new Vector2F(0, -12),
				new Vector2F(-4, -6),
				new Vector2F(-9, 7),
				new Vector2F(-4, 12),
				new Vector2F(7, 15),
				new Vector2F(15, 11)
			};

			// Spawn the seed projectile.
			SeedType seedType = weapon.CurrentSeedType;
			Seed seed = new Seed(seedType, true);
			Vector2F pos = Player.Position - new Vector2F(8, 16) + projectilePositions[angle] + new Vector2F(4, 4 + 7);
			seed.Physics.Velocity = Angles.ToVector(angle) * 3.0f;
			Player.RoomControl.SpawnEntity(seed, pos, Player.ZPosition + 5);

			isShooting = true;
			shootTimer = 0;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			isShooting		= false;
			shootTimer		= 0;
			angle			= player.UseAngle;
			returnDirection	= player.UseDirection;

			player.SyncAnimationWithDirection = false;
			player.Movement.MoveCondition = PlayerMoveCondition.OnlyInAir;

			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_AIM);
			player.Graphics.SubStripIndex = angle;
			player.toolAnimation.Play(GameData.ANIM_SEED_SHOOTER);
			player.toolAnimation.SubStripIndex = angle;
		}
		
		public override void OnEnd(PlayerState newState) {
			player.toolAnimation.Animation		= null;
			player.SyncAnimationWithDirection	= true;
			player.Movement.MoveCondition		= PlayerMoveCondition.FreeMovement;
		}

		public override void Update() {
			base.Update();
			
			if (isShooting) {
				shootTimer++;
				if (shootTimer >= SHOOT_WAIT_TIME) {
					player.Direction = returnDirection;
					player.BeginNormalState();
				}
			}
			else {
				// Update aiming controls.
				for (int dir = 0; dir < 4; dir++) {
					if (Controls.Arrows[dir].IsPressed()) {
						int goalAngle	= Directions.ToAngle(dir);
						int distCW		= Angles.GetAngleDistance(angle, goalAngle, WindingOrder.Clockwise);
						int distCCW		= Angles.GetAngleDistance(angle, goalAngle, WindingOrder.CounterClockwise);

						if (distCW != 0) {
							if (distCCW <= distCW)
								angle = GMath.Wrap(angle + 1, Angles.AngleCount); // Turn Counter-Clockwise
							else
								angle = GMath.Wrap(angle - 1, Angles.AngleCount); // Turn Clockwise
						}

						if (angle % 2 == 0)
							returnDirection = angle / 2;
					}
				}

				// Make player and seed-shooter face the aim angle.
				player.Graphics.SubStripIndex = angle;
				player.toolAnimation.SubStripIndex = angle;

				// Shoot when the button is released.
				if (!weapon.IsEquipped || !weapon.IsButtonDown()) {
					Shoot();
				}
			}
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemSeedShooter Weapon {
			get { return weapon; }
			set { weapon = value; }
		}
	}
}
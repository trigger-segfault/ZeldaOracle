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
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class BasicMonsterOld : Monster {
		
		public enum ShootType {
			None,			// Don't shoot anything.
			OnStop,			// Shoot only when stopping after moving.
			WhileMoving,	// Shoot randomly while moving.
		}

		public enum AimType {
			Forward,				// Aim in the current forward direction.
			FacePlayer,				// Face toward the player and shoot in that direction.
			FaceRandom,				// Face toward a random direction to shoot in.
			SeekPlayer,				// Shoot toward the player.
			SeekPlayerByDirection,	// Shoot toward the player in the nearest direction.
			SeekPlayerByAngle,		// Shoot toward the player in the nearest angle.
		}

		public enum ChargeType {
			None,				// Don't charge
			Charge,
			ChargeUntilCollision,	// Charge, stopping upon colliding.
			ChargeForDuration,		// Charge for a certain amount of time.

		}
		
		// Movement.
		protected float		moveSpeed;
		protected bool		changeDirectionsOnCollide;
		protected RangeI	stopTime;
		protected RangeI	moveTime;
		protected bool		movesInAir;
		protected bool		isMovementDirectionBased;
		protected int		numMoveAngles;

		// Charging.
		protected ChargeType chargeType;
		protected float		chargeSpeed;
		protected float		chargeAcceleration;
		protected int		chargeMinAlignment;
		protected RangeI	chargeDuration;

		// Chasing.
		protected float		chaseSpeed;
		protected int		chasePauseDuration;

		// Graphics.
		// Animation: horizontal? vertical? scale with speed? only play when moving?
		protected Animation	animationMove;
		protected bool		scaleAnimationSpeed;
		protected bool		playAnimationOnlyWhenMoving;
		protected bool		isAnimationHorizontal;

		// Projectile.
		protected ShootType	shootType;
		protected AimType	aimType;
		protected Type		projectileType;
		protected float		shootSpeed;
		protected int		projectileShootOdds;
		protected int		shootPauseDuration;
				
		// States
		protected bool		isPaused;
		protected int		pauseTimer;
		protected bool		isChasingPlayer;
		protected bool		isMoving;
		protected int		moveTimer;
		protected float		speed;
		protected int		moveAngleIndex;
		protected bool		isCharging;
		protected bool		isShooting;


		// Movement states:
		//  - moving
		//  - stopped
		//  - paused
		//  - chasing
		//  - shooting??

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public BasicMonsterOld() {
			Color			= MonsterColor.Red;
			MaxHealth		= 1;
			ContactDamage	= 1;

			scaleAnimationSpeed			= false;
			playAnimationOnlyWhenMoving	= true;
			isAnimationHorizontal		= false;

			moveSpeed					= 0.5f;
			numMoveAngles				= 4;
			isMovementDirectionBased	= true;
			changeDirectionsOnCollide	= true;
			syncAnimationWithDirection	= true;
			movesInAir					= false;
			stopTime.Set(30, 60);
			moveTime.Set(30, 50);

			chargeType			= ChargeType.None;
			chargeDuration		= RangeI.Zero;

			shootType			= ShootType.None;
			aimType				= AimType.Forward;
			projectileType		= null;
			shootSpeed			= 2.0f;
			projectileShootOdds	= 5;
			shootPauseDuration	= 30;
		}
		
		protected void ChangeDirection() {
			//direction = (direction + 1 + GRandom.NextInt(3)) % 4;
			
			moveAngleIndex = GRandom.NextInt(numMoveAngles);
			if (numMoveAngles == 4)
				direction = moveAngleIndex;
		}
		
		protected void FaceRandomDirection() {
			//direction = GRandom.NextInt(Directions.Count);

			moveAngleIndex = (moveAngleIndex + 1 + GRandom.NextInt(numMoveAngles - 1)) % numMoveAngles;
			if (numMoveAngles == 4)
				direction = moveAngleIndex;
		}

		protected void StartMoving() {
			isMoving = true;
			speed = moveSpeed;
			moveTimer = GRandom.NextInt(moveTime.Min, moveTime.Max);
			Graphics.PlayAnimation(animationMove);
			
			float dir = (moveAngleIndex / (float) numMoveAngles) * GMath.FullAngle;
			Physics.Velocity = Vector2F.CreatePolar(moveSpeed, dir) * new Vector2F(1.0f, -1.0f);
		}

		protected void StopMoving() {
			// Stop moving.
			isMoving = false;
			moveTimer = GRandom.NextInt(stopTime.Min, stopTime.Max);
			Physics.Velocity = Vector2F.Zero;
			Graphics.StopAnimation();

			// Shoot.
			if (shootType == ShootType.OnStop && projectileType != null && GRandom.NextInt(projectileShootOdds) == 0) {
				StartShooting();
			}
		}

		protected void StartShooting() {
			isShooting = true;
			pauseTimer = shootPauseDuration;
			
			if (aimType == AimType.FacePlayer)
				FacePlayer();
			else if (aimType == AimType.FaceRandom)
				FaceRandomDirection();
		}

		protected void Shoot() {
			Projectile projectile = (Projectile) projectileType.GetConstructor(Type.EmptyTypes).Invoke(null);
			Vector2F vectorToPlayer = (RoomControl.Player.Center - Center).Normalized * shootSpeed;
			
			if (aimType == AimType.SeekPlayer) {
				ShootProjectile(projectile, vectorToPlayer.Normalized * shootSpeed);
			}
			else if (aimType == AimType.SeekPlayerByDirection) {
				int shootDirection = Directions.NearestFromVector(vectorToPlayer);
				ShootFromDirection(projectile, shootDirection, shootSpeed);
			}
			else if (aimType == AimType.SeekPlayerByAngle) {
				int shootAngle = Angles.NearestFromVector(vectorToPlayer);
				ShootFromAngle(projectile, shootAngle, shootSpeed);
			}
			else {
				ShootFromDirection(projectile, direction, shootSpeed);
			}

			//Pause(shootPauseDuration);
		}

		protected void Pause(int duration) {
			pauseTimer = duration;
			isPaused = true;
		}

		protected void StartCharging(int chargeDirection) {
			direction		= chargeDirection;
			moveAngleIndex	= chargeDirection;
			isCharging		= true;

			if (chargeType == ChargeType.ChargeForDuration) {
				moveTimer = GRandom.NextInt(chargeDuration.Min, chargeDuration.Max);
			}
		}

		protected void UpdateChargingState() {
			speed = Math.Min(chargeSpeed, speed + chargeAcceleration);
			
			float dir = (moveAngleIndex / (float) numMoveAngles) * GMath.FullAngle;
			Physics.Velocity = Vector2F.CreatePolar(speed, dir) * new Vector2F(1.0f, -1.0f);

			if (chargeType == ChargeType.ChargeForDuration) {
				if (moveTimer <= 0) {
					isCharging = false;
					return;
				}
			}
			else {
				if (Physics.IsColliding) {
					isCharging = false;
					return;
				}
			}

			moveTimer--;
		}

		protected void UpdateMovingState() {
			float dir = (moveAngleIndex / (float) numMoveAngles) * GMath.FullAngle;
			Physics.Velocity = Vector2F.CreatePolar(speed, dir) * new Vector2F(1.0f, -1.0f);
			
			// Stop moving after a duration.
			if (moveTimer <= 0) {
				StopMoving();
				return;
			}

			// Change direction on collisions.
			if (changeDirectionsOnCollide && physics.IsColliding)
			{
				ChangeDirection();
			}
			
			// Shoot.
			if (shootType == ShootType.WhileMoving && projectileType != null && GRandom.NextInt(projectileShootOdds) == 0) {
				isMoving = false;
				StartShooting();
				//Shoot();
			}

			moveTimer--;
		}
		
		protected void UpdateStoppedState() {
			Physics.Velocity = Vector2F.Zero;
			
			// Start moving again after a duration.
			if (moveTimer <= 0) {
				ChangeDirection();
				StartMoving();
			}

			moveTimer--;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void UpdateSubStripIndex() {
			if (syncAnimationWithDirection) {
				if (isAnimationHorizontal) {
					if (Directions.IsHorizontal(direction))
						Graphics.SubStripIndex = direction / 2;
				}
				else {
					Graphics.SubStripIndex = direction;
				}
			}
		}

		public override void Initialize() {
			base.Initialize();

			Graphics.PlayAnimation(animationMove);
			Graphics.StopAnimation();
			
			isMoving		= false;
			moveTimer		= GRandom.NextInt(stopTime.Min, stopTime.Max);
			isChasingPlayer	= false;
			isPaused		= false;
			pauseTimer		= 0;
			isShooting		= false;

			FaceRandomDirection();
		}

		public override void Update() {
			base.Update();

			if (IsOnGround || movesInAir) {
				// Update movement states.
				if (isPaused) {
					if (pauseTimer <= 0) {
						isPaused = false;
					}
					pauseTimer--;
				}
				else if (isShooting) {
					if (pauseTimer <= 0) {
						Shoot();
						isShooting = false;
					}
					pauseTimer--;
				}
				else if (isCharging) {
					UpdateChargingState();
				}
				else if (isChasingPlayer) {
					FacePlayer();
					Physics.Velocity = (RoomControl.Player.Center - Center).Normalized * moveSpeed;
				}
				else {
					// Check for charging.
					if (chargeType != ChargeType.None) {
						int directionToPlayer = Directions.NearestFromVector(RoomControl.Player.Center - Center);
						if (Entity.AreEntitiesAligned(this, RoomControl.Player, directionToPlayer, chargeMinAlignment)) {
							StartCharging(directionToPlayer);
							return;
						}
					}

					if (isMoving) {
						UpdateMovingState();
					}
					else {
						UpdateStoppedState();
					}
				}
			}
		}

	}
}

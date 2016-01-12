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

namespace ZeldaOracle.Game.Entities.Monsters {

	public class BasicMonster : Monster {
		
		public enum ShootType {
			None,			// Don't shoot anything.
			OnStop,			// Shoot only when stopping after moving.
			WhileMoving,	// Shoot randomly while moving.
		}

		public enum AimType {
			Forward,				// Aim in the current forward direction.
			FacePlayer,				// Face toward the player and shoot in that direction.
			//FaceRandom,				// Face toward a random direction to shoot in.
			//SeekPlayer,				// Shoot toward the player.
			//SeekPlayerByDirection,	// Shoot toward the player in the nearest direction.
			//SeekPlayerByAngle,		// Shoot toward the player in the nearest angle.
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
				
		// States.
		protected bool		isPaused;
		protected int		pauseTimer;
		protected bool		isChasingPlayer;
		protected bool		isMoving;
		protected int		moveTimer;
		protected float		speed;
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
		
		public BasicMonster() {
			color			= MonsterColor.Red;
			MaxHealth		= 1;
			ContactDamage	= 1;

			scaleAnimationSpeed			= false;
			playAnimationOnlyWhenMoving	= true;
			isAnimationHorizontal		= false;

			moveSpeed					= 0.5f;
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
			
			if (GRandom.NextInt(4) == 0) {
				FacePlayer();
				return;
			}

			direction = (direction + 1 + GRandom.NextInt(3)) % 4;

			List<int> possibleDirections = new List<int>();

			for (int i = 0; i < Directions.Count; i++) {
				if (!Physics.IsPlaceMeetingSolid(position +
					(Directions.ToVector(i) * moveSpeed), Physics.CollisionBox))
				{
					possibleDirections.Add(i);
				}
			}

			if (possibleDirections.Count == 0)
				direction = GRandom.NextInt(Directions.Count);
			else
				direction = possibleDirections[GRandom.NextInt(possibleDirections.Count)];
		}
		
		protected void FaceRandomDirection() {
			direction = GRandom.NextInt(Directions.Count);
		}

		protected void FacePlayer() {
			Vector2F lookVector = RoomControl.Player.Center - Center;
			direction = Directions.NearestFromVector(lookVector);
		}

		protected void StartMoving() {
			isMoving = true;
			speed = moveSpeed;
			moveTimer = GRandom.NextInt(moveTime.Min, moveTime.Max);
			Physics.Velocity = Directions.ToVector(direction) * speed;
			ChangeDirection();

			if (!Graphics.IsAnimationPlaying || Graphics.Animation != animationMove)
				Graphics.PlayAnimation(animationMove);
		}

		protected void StopMoving() {
			moveTimer = GRandom.NextInt(stopTime.Min, stopTime.Max);
			isMoving = false;
			Physics.Velocity = Vector2F.Zero;
			
			// Shoot.
			if (shootType == ShootType.OnStop && projectileType != null && GRandom.NextInt(projectileShootOdds) == 0) {
				StartShooting();
			}
			else if (moveTimer == 0) {
				StartMoving();
				return;
			}

			Graphics.StopAnimation();
		}

		protected void StartShooting() {
			isShooting = true;
			pauseTimer = shootPauseDuration;
			
			if (aimType == AimType.FacePlayer)
				FacePlayer();
			//else if (aimType == AimType.FaceRandom)
				//FaceRandomDirection();
		}

		protected void Shoot() {
			Projectile projectile = (Projectile) projectileType.GetConstructor(Type.EmptyTypes).Invoke(null);
			Vector2F vectorToPlayer = (RoomControl.Player.Center - Center).Normalized * shootSpeed;
			/*
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
			}*/
			
			ShootFromDirection(projectile, direction, shootSpeed, Directions.ToVector(direction) * 8.0f);

			//Pause(shootPauseDuration);
		}

		protected void Pause(int duration) {
			pauseTimer = duration;
			isPaused = true;
		}

		protected void StartCharging(int chargeDirection) {
			direction	= chargeDirection;
			isCharging	= true;

			if (chargeType == ChargeType.ChargeForDuration) {
				moveTimer = GRandom.NextInt(chargeDuration.Min, chargeDuration.Max);
			}
		}

		protected void UpdateChargingState() {
			speed = Math.Min(chargeSpeed, speed + chargeAcceleration);
			
			Physics.Velocity = Directions.ToVector(direction) * speed;

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
			Physics.Velocity = Directions.ToVector(direction) * speed;
			
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
				//ChangeDirection();
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
			
			color = (MonsterColor) Properties.GetInteger("color", (int) color);

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

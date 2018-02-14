using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Tiles;

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
			FaceRandom,				// Face toward a random direction to shoot in.
			SeekPlayer,				// Shoot toward the player.
			//SeekPlayerByDirection,	// Shoot toward the player in the nearest direction.
			//SeekPlayerByAngle,		// Shoot toward the player in the nearest angle.
		}

		public enum ChargeType {
			None,				// Don't charge
			Charge,
			ChargeUntilCollision,	// Charge, stopping upon colliding.
			ChargeForDuration,		// Charge for a certain amount of time.
		}
		
		// Movement
		protected float		moveSpeed;
		protected bool		changeDirectionsOnCollide;
		protected RangeI	stopTime;
		protected RangeI	moveTime;
		protected bool		movesInAir;
		protected int		facePlayerOdds;
		protected int		numMoveAngles;
		protected bool		avoidHazardTiles;

		// Charging
		protected ChargeType chargeType;
		protected float		chargeSpeed;
		protected float		chargeAcceleration;
		protected int		chargeMinAlignment;
		protected RangeI	chargeDuration;
		protected int		chargeCooldown;
		protected int		chargeCooldownTimer;

		// Chasing
		protected float		chaseSpeed;
		protected int		chasePauseDuration;

		// Graphics
		// Animation: horizontal? vertical? scale with speed? only play when moving?
		protected Animation	animationMove;
		protected bool		scaleAnimationSpeed;
		protected bool		playAnimationOnlyWhenMoving;
		protected bool		isAnimationHorizontal;

		// Projectile
		protected ShootType	shootType;
		protected AimType	aimType;
		protected Type		projectileType;
		protected float		shootSpeed;
		protected int		projectileShootOdds;
		protected int		shootPauseDuration;
		protected Sound		shootSound;
				
		// States
		protected bool		isPaused;
		protected int		pauseTimer;
		protected bool		isChasingPlayer;
		protected bool		isMoving;
		protected int		moveTimer;
		protected float		speed;
		protected bool		isCharging;
		protected bool		isShooting;
		protected int		moveAngle;
		
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
			Color			= MonsterColor.Red;
			MaxHealth		= 1;
			ContactDamage	= 1;

			scaleAnimationSpeed			= false;
			playAnimationOnlyWhenMoving	= true;
			isAnimationHorizontal		= false;

			moveSpeed					= 0.5f;
			changeDirectionsOnCollide	= true;
			avoidHazardTiles			= true;
			syncAnimationWithDirection	= true;
			movesInAir					= false;
			facePlayerOdds				= 4;
			orientationStyle			= OrientationStyle.Direction;
			numMoveAngles				= Directions.Count;
			stopTime.Set(30, 60);
			moveTime.Set(30, 50);

			//Orientation x = DirectionEnum.Up;

			chargeType			= ChargeType.None;
			chargeDuration		= RangeI.Zero;
			chargeCooldown		= 66;
			chargeCooldownTimer	= 0;

			shootType			= ShootType.None;
			aimType				= AimType.Forward;
			projectileType		= null;
			shootSpeed			= 2.0f;
			projectileShootOdds	= 5;
			shootPauseDuration	= 30;
			shootSound			= null;
		}


		//-----------------------------------------------------------------------------
		// Behavior
		//-----------------------------------------------------------------------------
	
		protected virtual bool CanMoveInAngle(int moveAngle) {
			Vector2F v = GetMovementVelocity(moveAngle, moveSpeed);
			Vector2F testPosition = position + (v * 1.1f);
			if (Physics.IsPlaceMeetingSolid(testPosition) ||
				Physics.IsPlaceMeetingRoomEdge(testPosition))
			{
				return false;
			}
			
			if (Physics.GetTilesMeeting(testPosition, CollisionBoxType.Hard)
				.Any(t => t.IsHoleWaterOrLava))
				return false;

			return true;
		}

		protected void ChangeDirection() {
			
			// Face the player every so often
			if (facePlayerOdds > 0 && GRandom.NextInt(facePlayerOdds) == 0) {
				Vector2F lookVector = RoomControl.Player.Center - Center;
				int facePlayerAngle = Orientations.NearestFromVector(
					lookVector, numMoveAngles);
				if (CanMoveInAngle(facePlayerAngle)) {
					MoveAngle = facePlayerAngle;
					return;
				}
			}

			// Create a list of obstruction-free move angles
			List<int> possibleAngles = new List<int>();
			for (int i = 0; i < numMoveAngles; i++) {
				if (CanMoveInAngle(i)) {
					possibleAngles.Add(i);
				}
			}

			if (possibleAngles.Count == 0) {
				// No collision-free angles, so face a new random angle
				MoveAngle = (moveAngle + 1 +
					GRandom.NextInt(numMoveAngles - 1)) % numMoveAngles;
			}
			else {
				MoveAngle = GRandom.Choose(possibleAngles);
			}
		}
		
		protected void FaceRandomDirection() {
			MoveAngle = GRandom.NextInt(numMoveAngles);
		}

		protected void StartMoving() {
			isMoving	= true;
			speed		= moveSpeed;
			moveTimer	= GRandom.NextInt(moveTime.Min, moveTime.Max);

			ChangeDirection();

			Physics.Velocity = GetMovementVelocity(moveAngle, speed);

			if (!Graphics.IsAnimationPlaying || Graphics.Animation != animationMove)
				Graphics.PlayAnimation(animationMove);
		}

		protected void StopMoving() {
			moveTimer = GRandom.NextInt(stopTime.Min, stopTime.Max);
			isMoving = false;
			Physics.Velocity = Vector2F.Zero;
			
			// Shoot
			if (shootType == ShootType.OnStop && projectileType != null &&
				GRandom.NextInt(projectileShootOdds) == 0)
			{
				StartShooting();
			}
			else if (moveTimer == 0) {
				StartMoving();
				return;
			}

			if (playAnimationOnlyWhenMoving)
				Graphics.StopAnimation();
		}

		protected void StartShooting() {
			pauseTimer = shootPauseDuration;
			
			if (aimType == AimType.FacePlayer)
				FacePlayer();
			else if (aimType == AimType.FaceRandom)
				FaceRandomDirection();
			
			if (pauseTimer == 0) {
				Shoot();
			}
			else {
				isShooting = true;
				isMoving = false;
			}
		}

		protected void Shoot() {
			if (shootSound != null)
				AudioSystem.PlaySound(shootSound);
			
			// Construct the projectile
			Projectile projectile = (Projectile) projectileType
				.GetConstructor(Type.EmptyTypes).Invoke(null);

			// Determine projectile velocity
			Vector2F projectileUnitVelocity = 
				Directions.ToVector(direction);
			if (aimType == AimType.SeekPlayer) {
				Vector2F vectorToPlayer = RoomControl.Player.Center - Center;
				projectileUnitVelocity = vectorToPlayer.Normalized;
			}
			Vector2F projectileVelocity = projectileUnitVelocity * shootSpeed;
			
			// Spawn the projectile
			projectile.Owner = this;
			projectile.Direction = direction;
			projectile.Physics.Velocity = projectileVelocity;
			RoomControl.SpawnEntity(projectile,
				Center + (projectileUnitVelocity * 8.0f),
				zPosition);
		}

		protected void Pause(int duration) {
			pauseTimer = duration;
			isPaused = true;
		}

		protected void StartCharging(int chargeDirection) {
			direction	= chargeDirection;
			moveAngle	= direction * 4 / numMoveAngles;
			isCharging	= true;
			chargeCooldownTimer = chargeCooldown;

			if (chargeType == ChargeType.ChargeForDuration) {
				moveTimer = GRandom.NextInt(chargeDuration.Min, chargeDuration.Max);
			}
		}

		protected void EndCharging() {
			isCharging = false;
			speed = GMath.Min(speed, moveSpeed);
		}

		protected void UpdateChargingState() {
			speed = GMath.Min(chargeSpeed, speed + chargeAcceleration);

			Physics.Velocity = GetMovementVelocity(moveAngle, speed);

			if (chargeType == ChargeType.ChargeForDuration) {
				if (moveTimer <= 0) {
					EndCharging();
					return;
				}
			}
			else {
				if (Physics.IsColliding) {
					EndCharging();
					return;
				}
			}

			moveTimer--;
		}

		protected void UpdateMovingState() {
			Physics.Velocity = GetMovementVelocity(moveAngle, speed);
			
			// Stop moving after a duration
			if (moveTimer <= 0) {
				StopMoving();
				return;
			}

			// Change direction on collisions
			if (changeDirectionsOnCollide && physics.IsColliding)
			{
				ChangeDirection();
				Physics.Velocity = GetMovementVelocity(moveAngle, speed);
			}
			else if (avoidHazardTiles)
			{
				// Avoid moving into a hazardous tile
				foreach (Tile tile in Physics.GetTilesMeeting(
					position + physics.Velocity * 1.1f, CollisionBoxType.Hard))
				{
					if (tile.IsHoleWaterOrLava) {
						ChangeDirection();
						Physics.Velocity = GetMovementVelocity(moveAngle, speed);
						break;
					}
				}
			}
			
			// Shoot while moving
			if (shootType == ShootType.WhileMoving && projectileType != null &&
				GRandom.NextInt(projectileShootOdds) == 0)
			{
				StartShooting();
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

		protected Vector2F GetMovementVelocity(int moveAngle, float speed) {
			return Orientations.ToVector(moveAngle, numMoveAngles) * speed;
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

		protected override void FacePlayer() {
			Vector2F lookVector = RoomControl.Player.Center - Center;
			MoveAngle = Orientations.NearestFromVector(lookVector, numMoveAngles);
		}

		public override void Initialize() {
			base.Initialize();

			Graphics.PlayAnimation(animationMove);
			if (playAnimationOnlyWhenMoving)
				Graphics.StopAnimation();
			
			isMoving		= false;
			moveTimer		= GRandom.NextInt(stopTime.Min, stopTime.Max);
			isChasingPlayer	= false;
			isPaused		= false;
			pauseTimer		= 0;
			isShooting		= false;
			
			Color = (MonsterColor) Properties.GetInteger("color", (int) Color);

			FaceRandomDirection();
		}

		public override void UpdateAI() {
			
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
					if (chargeCooldownTimer > 0) {
						chargeCooldownTimer--;
					}
					else if (chargeType != ChargeType.None) {
						int directionToPlayer = Directions.NearestFromVector(
							RoomControl.Player.Center - Center);
						if (Entity.AreEntitiesCollisionAligned(this, RoomControl.Player,
							directionToPlayer, CollisionBoxType.Hard))
						{
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int MoveAngle {
			get { return moveAngle; }
			set {
				if (value != moveAngle) {
					moveAngle = value;
					
					// Update direction/angle
					if (orientationStyle == OrientationStyle.Direction)
						Direction = (moveAngle * Directions.Count) / numMoveAngles;
					else 
						Angle = (moveAngle * Angles.AngleCount) / numMoveAngles;
				}
			}
		}
	}
}

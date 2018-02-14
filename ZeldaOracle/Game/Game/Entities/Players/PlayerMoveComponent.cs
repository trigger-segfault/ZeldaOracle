using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Input.Controls;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Players {
	
	public enum PlayerMoveCondition {
		FreeMovement,	// Freely control movement
		OnlyInAir,		// Only control his movement in air.
		NoControl,		// No movement control.
	}
	
	public class PlayerMoveComponent {
		
		// Settings
		private bool				autoAccelerate;			// Should the player still accelerate without holding down a movement key?
		private PlayerMoveCondition	moveCondition;			// What are the conditions in which the player can move?
		private bool				canLedgeJump;
		private bool				canJump;
		private bool				canPush;
		private bool				canUseWarpPoint;		// Can the player go through warp points?

		// Internal
		private Player				player;
		private float				analogAngle;
		private bool				allowMovementControl;	// Is the player allowed to control his movement?
		private bool				analogMode;				// True if the analog stick is active.
		private bool[]				moveAxes;				// Which axes the player is moving on.
		private bool				isMoving;				// Is the player holding down a movement key?
		private Vector2F			motion;					// The vector that's driving the player's velocity.
		private Vector2F			velocityPrev;			// The player's velocity on the previous frame.
		private int					moveAngle;				// The angle the player is moving in.
		private int					moveDirection;			// The direction that the player wants to face.


		private Point2I				jumpStartTile;			// The tile the player started jumping on. (Used for jump color tiles)
		private bool				isCapeDeployed;
		private Tile				holeTile;
		private bool				doomedToFallInHole;
		private int					holeDoomTimer;
		private Vector2F			holeSlipVelocity;
		private Point2I				holeEnterQuadrent;
		private bool				fallingInHole;
		private bool				isOnColorBarrier;
		private Rectangle2F			climbCollisionBox;		// Used for checking if on a ladder in side-scroll mode.

		// Movement modes
		private PlayerMotionType	mode;
		private PlayerMotionType	moveModeNormal;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerMoveComponent(Player player) {
			this.player = player;

			// Default settings.
			autoAccelerate			= false;
			moveCondition			= PlayerMoveCondition.FreeMovement;
			canLedgeJump			= true;
			canJump					= true;
			canPush					= true;
			canUseWarpPoint			= true;

			// Internal.
			allowMovementControl	= true;
			moveAxes				= new bool[] { false, false };
			motion					= Vector2F.Zero;
			isMoving				= false;
			velocityPrev			= Vector2F.Zero;
			moveAngle				= Angles.South;
			mode					= new PlayerMotionType();
			jumpStartTile			= -Point2I.One;
			isCapeDeployed			= false;
			doomedToFallInHole		= false;
			holeTile				= null;
			holeDoomTimer			= 0;
			holeSlipVelocity		= Vector2F.Zero;
			fallingInHole			= false;
			isOnColorBarrier		= false;
			climbCollisionBox		= new Rectangle2F(-1, -7, 2, 9);

			// Controls.
			analogMode		= false;
			analogAngle		= 0.0f;

			// Normal movement.
			moveModeNormal = new PlayerMotionType();
			mode = moveModeNormal;
		}
		

		//-----------------------------------------------------------------------------
		// Movement Checks
		//-----------------------------------------------------------------------------
		
		public bool IsMovingInDirection(int direction) {
			return (IsMoving &&
				Directions.ToPoint(direction).Dot(Angles.ToPoint(moveAngle)) > 0);
		}
		
		
		//-----------------------------------------------------------------------------
		// Movement
		//-----------------------------------------------------------------------------

		public void StopMotion() {
			player.Physics.Velocity = Vector2F.Zero;
			motion = Vector2F.Zero;
		}

		public void StartSprinting(int duration, float sprintSpeedScale) {
			player.BeginConditionState(new PlayerSprintState(
				duration, sprintSpeedScale));
		}

		public void Jump() {

			if (player.IsOnGround) {
				// Allow initial jump movement if only can move in air.
				if (moveCondition != PlayerMoveCondition.NoControl && !mode.IsSlippery) {
					Vector2F moveVector = PollMovementKeys(true);

					if (isMoving) {
						float scaledSpeed = mode.MovementSpeed * player.StateParameters.MovementSpeedScale;
						Vector2F keyMotion		= moveVector * scaledSpeed;
						player.Physics.Velocity	= keyMotion;
						motion					= keyMotion;
					}
				}
				
				// Jump!
				isCapeDeployed = false;
				jumpStartTile = player.RoomControl.GetTileLocation(player.Position);
				player.Physics.Gravity = GameSettings.DEFAULT_GRAVITY;
				if (player.RoomControl.IsSideScrolling)
					player.Physics.ZVelocity = GameSettings.PLAYER_SIDESCROLL_JUMP_SPEED;
				else
					player.Physics.ZVelocity = GameSettings.PLAYER_JUMP_SPEED;
				if (player.WeaponState == null)
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_JUMP);
				
				// Make sure we go to the environment state for jumping
				if (player.IsOnSideScrollLadder)
					player.SideScrollLadderState.End();
				player.IntegrateStateParameters();
				player.RequestNaturalState();
				
				player.OnJump();
			}
		}

		// Deploy Roc's Cape
		public void DeployCape() {
			if (player.IsInAir && !isCapeDeployed && player.Physics.ZVelocity -
				GameSettings.DEFAULT_GRAVITY <= -GameSettings.PLAYER_CAPE_REQUIRED_FALLSPEED)
			{
				isCapeDeployed = true;
				player.Physics.Gravity = GameSettings.PLAYER_CAPE_GRAVITY;
				if (player.RoomControl.IsSideScrolling)
					player.Physics.ZVelocity = GameSettings.PLAYER_SIDESCROLL_CAPE_JUMP_SPEED + GameSettings.PLAYER_CAPE_GRAVITY;
				else
					player.Physics.ZVelocity = GameSettings.PLAYER_CAPE_JUMP_SPEED + GameSettings.PLAYER_CAPE_GRAVITY;
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_THROW);
				if (player.WeaponState == null)
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_CAPE);
			}
		}

		private Vector2F PollMovementKeys(bool allowMovementControl) {
			Vector2F moveVector = Vector2F.Zero;
			isMoving	= false;
			analogMode	= !Controls.AnalogMovement.Position.IsZero;

			if (analogMode) {
				// Check analog stick.
				analogAngle = Controls.AnalogMovement.Position.Direction;
				CheckAnalogStick(allowMovementControl);
			}
			else {
				// Check movement keys.
				if (!CheckMoveKey(Directions.Left, allowMovementControl) && !CheckMoveKey(Directions.Right, allowMovementControl))
					moveAxes[Axes.X] = false;
				if (!CheckMoveKey(Directions.Up, allowMovementControl) && !CheckMoveKey(Directions.Down, allowMovementControl))
					moveAxes[Axes.Y] = false;
			}

			// Update movement or acceleration.
			if (allowMovementControl && (isMoving || autoAccelerate)) {
				if (analogMode)
					moveVector = Controls.AnalogMovement.Position;
				else
					moveVector = Angles.ToVector(moveAngle);
			}

			// Clip all Y velocity when sidescrolling with gravity enabled,
			// so that we only move left or right
			if (player.RoomControl.IsSideScrolling && player.Physics.HasGravity)
			{
				if (analogMode)
					moveVector = new Vector2F(Controls.AnalogMovement.Position.X, 0.0f);
				else
					moveVector = new Vector2F(Angles.ToVector(moveAngle, false).X, 0.0f);
			}

			return moveVector;
		}

		private void UpdateMoveControls() {
			// Check if the player is allowed to control his motion.
			allowMovementControl = true;
			if (moveCondition == PlayerMoveCondition.NoControl)
				allowMovementControl = false;
			else if (player.IsOnGround && player.IsBeingKnockedBack)
				allowMovementControl = false;
			else if (moveCondition == PlayerMoveCondition.OnlyInAir && player.IsOnGround)
				allowMovementControl = false;
			else if (player.IsInAir) {
				if (player.RoomControl.IsSideScrolling && player.Physics.Velocity.Y <= 0.1f)
					allowMovementControl = false;
				else if (!player.RoomControl.IsSideScrolling && player.Physics.ZVelocity >= 0.1f)
					allowMovementControl = false;
			}

			// Player can ALWAYS change directions when in a minecart.
			if (player.IsInMinecart)
				allowMovementControl = true;

			// Check movement input
			Vector2F keyMoveVector = PollMovementKeys(allowMovementControl);
			
			// Update the players direction
			bool canUpdateDirection = false;
			if (player.StateParameters.AlwaysFaceUp)
				canUpdateDirection = (moveDirection == Directions.Up);
			else if (player.StateParameters.AlwaysFaceLeftOrRight)
				canUpdateDirection = (moveDirection == Directions.Left ||
					moveDirection == Directions.Right);
			else
				canUpdateDirection = !player.StateParameters.EnableStrafing;
			if (canUpdateDirection && allowMovementControl && isMoving)
				player.Direction = moveDirection;

			// Update movement or acceleration.
			if (allowMovementControl && (isMoving || autoAccelerate) && !player.IsInMinecart) {
				if (!isMoving)
					moveAngle = Directions.ToAngle(player.Direction);

				// Determine key-motion (the velocity we want to move at)
				float scaledSpeed = mode.MovementSpeed * player.StateParameters.MovementSpeedScale;
				Vector2F keyMotion = keyMoveVector * scaledSpeed;

				// Update acceleration-based motion.
				if (mode.IsSlippery) {
					// If player velocity has been halted by collisions, then
					// represent that in the motion vector
					Vector2F velocity = player.Physics.Velocity;
					if (GMath.Abs(velocity.X) < GMath.Abs(velocityPrev.X) ||
						GMath.Sign(velocity.X) != GMath.Sign(velocityPrev.X))
						motion.X = velocity.X;
					if (!player.RoomControl.IsSideScrolling) {
						if (GMath.Abs(velocity.Y) < GMath.Abs(velocityPrev.Y) ||
							GMath.Sign(velocity.Y) != GMath.Sign(velocityPrev.Y))
							motion.Y = velocity.Y;
					}

					// Apply acceleration and limit speed
					motion += keyMotion * mode.Acceleration;
					float newLength = motion.Length;
					if (newLength >= scaledSpeed)
						motion.Length = scaledSpeed;

					// Set the players velocity
					if (mode.DirectionSnapCount > 0 &&
						!player.RoomControl.IsSideScrolling)
					{
						// Snap velocity direction
						player.Physics.Velocity = Vector2F.SnapDirectionByCount(
							motion, mode.DirectionSnapCount);
					}
					else {
						// Don't snap velocity direction.
						player.Physics.Velocity = motion;
					}
				}
				else {
					// For non-acceleration based motion, move at regular speed
					motion = keyMotion;
					player.Physics.Velocity = motion;
				}
			}
			else {
				// Stop movement, using deceleration for slippery motion
				if (mode.IsSlippery) {
					float length = motion.Length;
					if (length < mode.MinSpeed)
						motion = Vector2F.Zero;
					else
						motion.Length = length - mode.Deceleration;
					player.Physics.Velocity = motion;
				}
				else {
					motion = Vector2F.Zero;
					player.Physics.Velocity = Vector2F.Zero;
				}
			}
			
			ChooseAnimation();
		}

		public void ChooseAnimation() {
			// Update movement animation.
			if (player.IsOnGround && !player.IsInMinecart &&
				moveCondition != PlayerMoveCondition.NoControl && 
				(player.Graphics.Animation == player.MoveAnimation ||
				player.Graphics.Animation == GameData.ANIM_PLAYER_DEFAULT ||
				player.Graphics.Animation == GameData.ANIM_PLAYER_CARRY ||
				player.Graphics.Animation == GameData.ANIM_PLAYER_MINECART_IDLE ||
				player.Graphics.Animation == GameData.ANIM_PLAYER_MERMAID_SWIM))
			{
				// Play/stop the move animation
				if (isMoving || IsSprinting) {
					if (!player.Graphics.IsAnimationPlaying)
						player.Graphics.PlayAnimation();
				}
				else {
					if (player.Graphics.IsAnimationPlaying)
						player.Graphics.StopAnimation();
				}
			}
			
			// Move animation can be replaced by cape animation.
			if (player.Graphics.Animation == player.MoveAnimation && player.IsInAir && isCapeDeployed)
				player.Graphics.SetAnimation(GameData.ANIM_PLAYER_CAPE);
			else if (player.IsOnGround && player.Graphics.Animation == GameData.ANIM_PLAYER_CAPE)
				player.Graphics.SetAnimation(player.MoveAnimation);
		}
		
		// Poll the movement key for the given direction, returning true if
		// it is down. This also manages the strafing behavior of movement.
		private bool CheckMoveKey(int dir, bool allowMovementControl) {
			if (Controls.GetArrowControl(dir).IsDown()) {
				if (allowMovementControl)
					isMoving = true;
			
				if (!moveAxes[(dir + 1) % 2])
					moveAxes[dir % 2] = true;

				if (moveAxes[dir % 2]) {
					moveDirection = dir;
					moveAngle = dir * 2;

					if (Controls.GetArrowControl((dir + 1) % 4).IsDown())
						moveAngle = (moveAngle + 1) % 8;
					if (Controls.GetArrowControl((dir + 3) % 4).IsDown())
						moveAngle = (moveAngle + 7) % 8;
				}
				return true;
			}
			return false;
		}

		// Update player direction angle angle from analog controls.
		private void CheckAnalogStick(bool allowMovementControl) {
			if (allowMovementControl)
				isMoving = true;
			
			moveAxes[0]		= true;
			moveAxes[1]		= true;
			moveDirection   = Directions.RoundFromRadians(analogAngle);
		}

		private void UpdateFallingInHoles() {
			if (fallingInHole) {
				holeDoomTimer--;
				
				// After a delay, disable the player's motion.
				if (holeDoomTimer < 0)
					player.Physics.Velocity = Vector2F.Zero;

				if (!player.Physics.IsInHole) {
					// Stop falling in a hole.
					fallingInHole		= false;
					doomedToFallInHole	= false;
					holeTile			= null;
					return;
				}
				else if (doomedToFallInHole) {
					// Collide with hole boundary's inside edges.
					Rectangle2F collisionBox = new Rectangle2F(-1, -1, 2, 2);
					player.Physics.PerformInsideEdgeCollisions(collisionBox, holeTile.Bounds);
				}
				else {
					// Check if the player has changed quadrents,
					// which dooms him to fall in the hole.
					Point2I holeTileLoc = player.RoomControl.GetTileLocation(player.Position);
					Tile newHoleTile	= player.RoomControl.GetTopTile(holeTileLoc);
					Point2I newQuadrent	= (Point2I) (player.Position / 8);
					if (newQuadrent != holeEnterQuadrent) {
						doomedToFallInHole = true;
						holeTile = newHoleTile;
					}
				}
				
				// Move toward the hole's center on each axis seperately.
				for (int i = 0; i < 2; i++) {
					float diff = holeTile.Center[i] - player.Center[i];
					if (GMath.Abs(diff) > 0.6f) {
						float dist = 0.25f;
						
						// Pull the player in more if he's moving away from the hole.
						if ((diff < 0.0f && player.Physics.Velocity[i] > 0.25f) ||
							(diff > 0.0f && player.Physics.Velocity[i] < -0.25f))
							dist = 0.5f;

						if (!(diff < 0.0f && player.Physics.Velocity[i] < -0.25f) &&
							!(diff > 0.0f && player.Physics.Velocity[i] > 0.25f))
						{
							holeSlipVelocity[i] = GMath.Sign(diff) * dist;
						}
					}
				}
				player.Position += holeSlipVelocity;
					
				// Fall in the hole when too close to the center.
				if (player.Center.DistanceTo(holeTile.Center) <= 1.0f) {
					AudioSystem.PlaySound(GameData.SOUND_PLAYER_FALL);
					player.SetPositionByCenter(holeTile.Center);
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_FALL);
					player.RespawnDeath();
					holeTile			= null;
					fallingInHole		= false;
					doomedToFallInHole	= false;
				}
			}
			else if (player.Physics.IsInHole && player.ControlState != player.RespawnDeathState) {
				// Start falling in a hole.
				Point2I holeTileLoc = player.RoomControl.GetTileLocation(player.Position);
				holeTile			= player.Physics.TopTile;
				holeEnterQuadrent	= (Point2I) (player.Position / 8);
				doomedToFallInHole	= false;
				fallingInHole		= true;
				holeDoomTimer		= 10;
			}
		}


		//-----------------------------------------------------------------------------
		// Update
		//-----------------------------------------------------------------------------

		public void Update() {
			// Get the movement mode for the current environment state
			if (player.EnvironmentState != null)
				mode = player.EnvironmentState.MotionSettings;
			else
				mode = moveModeNormal;
			
			// Update movement.
			UpdateMoveControls();
			UpdateFallingInHoles();
			velocityPrev = player.Physics.Velocity;

			if (allowMovementControl) {
				if (player.StateParameters.AlwaysFaceUp)
					player.Direction = Directions.Up;
				else if (player.StateParameters.AlwaysFaceLeftOrRight) {
					if (player.Direction == Directions.Up)
						player.Direction = Directions.Right;
					if (player.Direction == Directions.Down)
						player.Direction = Directions.Left;
				}
			}

			// Check for ledge jumping (ledges/waterfalls)
			CollisionInfo collisionInfo = player.Physics.CollisionInfo[moveDirection];
			if (canLedgeJump && isMoving &&
				collisionInfo.Type == CollisionType.Tile &&
				!player.RoomControl.IsSideScrolling)
			{
				Tile tile = collisionInfo.Tile;
				if (tile.IsLedge && moveDirection == tile.LedgeDirection && !tile.IsMoving)
					TryLedgeJump(tile.LedgeDirection);
				else if (tile.IsLeapLedge && moveDirection == tile.LedgeDirection && !tile.IsMoving)
					TryLeapLedgeJump(tile.LedgeDirection, tile);
			}
			
			// Check for walking on color barriers.
			if (player.IsOnGround && (player.Physics.TopTile is TileColorBarrier) &&
				((TileColorBarrier) player.Physics.TopTile).IsRaised)
			{
				IsOnColorBarrier = true;
			}
			else if (player.IsOnGround)
				IsOnColorBarrier = false;
		}

		/// <summary>Try to perform a ledge jump if the path is clear.</summary>
		private bool TryLedgeJump(int ledgeDirection) {
			Rectangle2F entityBox = player.Physics.PositionedCollisionBox;
			entityBox.Point += Directions.ToVector(ledgeDirection) * 1.0f;

			// Check if there any obstructions in front of the player.
			foreach (Tile tile in player.RoomControl.TileManager.GetTilesTouching(entityBox)) {
				if ((!tile.IsLedge || tile.LedgeDirection != ledgeDirection) &&
					tile.IsSolid && tile.CollisionModel != null)
				{
					// Check collisions with the tile's collision box.
					// Account for any safe edge-clipping.
					foreach (Rectangle2F box in tile.CollisionModel.Boxes) {
						Rectangle2F solidBox = box;
						solidBox.Point += tile.Position;

						if (entityBox.Intersects(solidBox) &&
							!player.Physics.IsSafeClippingInDirection(solidBox, (ledgeDirection + 1) % 4) &&
							!player.Physics.IsSafeClippingInDirection(solidBox, (ledgeDirection + 2) % 4) &&
							!player.Physics.IsSafeClippingInDirection(solidBox, (ledgeDirection + 3) % 4) &&
							!player.Physics.CanDodgeCollision(solidBox, ledgeDirection))
						{
							return false;
						}
					}
				}
			}

			// No obstructions: begin ledge jump!
			player.LedgeJumpState.LedgeJumpDirection = ledgeDirection;
			player.BeginControlState(player.LedgeJumpState);
			return true;
		}

		/// <summary>Try to perform a leap ledge jump to the opposite leap ledge.</summary>
		private bool TryLeapLedgeJump(int ledgeDirection, Tile startingTile) {
			// Check if there is an opposite leap ledge for landing on in our trajectory
			Tile landingTile = player.RoomControl.TileManager.GetTopTile(
				startingTile.Location + Directions.ToPoint(ledgeDirection) * 2);
			if (landingTile == null || !landingTile.IsLeapLedge ||
				landingTile.LedgeDirection != Directions.Reverse(ledgeDirection))
			{
				return false;
			}

			// Landing tile present: begin leap ledge jump!
			player.LeapLedgeJumpState.LedgeJumpDirection = ledgeDirection;
			player.BeginControlState(player.LeapLedgeJumpState);
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool AutoAccelerate {
			get { return autoAccelerate; }
			set { autoAccelerate = value; }
		}
		
		public bool CanLedgeJump {
			get { return canLedgeJump; }
			set { canLedgeJump = value; }
		}
		
		public bool CanJump {
			get { return canJump; }
			set { canJump = value; }
		}
		
		public bool CanPush {
			get { return canPush; }
			set { canPush = value; }
		}
		
		public bool IsMoving {
			get { return isMoving; }
			set { isMoving = value; }
		}
		
		public bool CanUseWarpPoint {
			get { return canUseWarpPoint; }
			set { canUseWarpPoint = value; }
		}
		
		public bool IsSprinting {
			get {
				return player.ConditionStates.Any(
					s => s is PlayerSprintState);
			}
		}
		
		public int MoveDirection {
			get { return moveDirection; }
		}
		
		public int MoveAngle {
			get { return moveAngle; }
		}
		
		public bool AllowMovementControl {
			get { return allowMovementControl; }
		}

		public bool IsCapeDeployed {
			get { return (player.IsInAir && isCapeDeployed); }
		}
		
		public PlayerMotionType MoveMode {
			get { return mode; }
		}
		
		public Vector2F Motion {
			get { return motion; }
		}
		
		public PlayerMoveCondition MoveCondition {
			get { return moveCondition; }
			set { moveCondition = value; }
		}

		public Point2I JumpStartTile {
			get { return jumpStartTile; }
			set { jumpStartTile = value; }
		}

		public bool IsOnColorBarrier {
			get { return isOnColorBarrier; }
			set {
				isOnColorBarrier = value;
				player.Graphics.DrawOffset = new Point2I(-8, -13); // TODO: magic numbers
				if (isOnColorBarrier)
					player.Graphics.DrawOffset -= new Point2I(0, 3);
			}
		}

		public bool IsDoomedToFallInHole {
			get { return (fallingInHole && doomedToFallInHole); }
		}

		public Rectangle2F ClimbCollisionBox {
			get { return climbCollisionBox; }
		}
	}
}

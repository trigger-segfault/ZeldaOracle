using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Input.Controls;

namespace ZeldaOracle.Game.Entities.Players {
	
	public enum PlayerMoveCondition {
		FreeMovement,	// Freely control movement
		OnlyInAir,		// Only control his movement in air.
		NoControl,		// No movement control.
	}
	
	public class PlayerMoveComponent {
		
		// Settings.
		private bool				autoAccelerate;			// Should the player still accelerate without holding down a movement key?
		private float				moveSpeedScale;			// Scales the movement speed to create the actual top-speed.
		private PlayerMoveCondition	moveCondition;			// What are the conditions in which the player can move?
		private bool				canLedgeJump;
		private bool				canJump;
		private bool				canPush;
		private bool				canUseWarpPoint;		// Can the player go through warp points?
		private bool				isStrafing;

		// Internal
		private Player				player;
		private AnalogStick			analogStick;
		private float				analogAngle;
		private bool				allowMovementControl;	// Is the player allowed to control his movement?
		private bool				analogMode;				// True if the analog stick is active.
		private InputControl[]		moveButtons;			// The 4 movement controls for each direction.
		private bool[]				moveAxes;				// Which axes the player is moving on.
		private bool				isMoving;				// Is the player holding down a movement key?
		private Vector2F			motion;					// The vector that's driving the player's velocity.
		private Vector2F			velocityPrev;			// The player's velocity on the previous frame.
		private int					moveAngle;				// The angle the player is moving in.
		private int					moveDirection;			// The direction that the player wants to face.
		private Point2I				jumpStartTile;			// The tile the player started jumping on. (Used for jump color tiles)

		// Movement modes.
		private PlayerMotionType	mode;
		private PlayerMotionType	moveModeNormal;		// For regular walking.
		private PlayerMotionType	moveModeSlow;		// For climbing ladders and stairs.
		private PlayerMotionType	moveModeIce;		// For walking on ice.
		private PlayerMotionType	moveModeAir;		// For jumping
		private PlayerMotionType	moveModeWater;		// For swimming.

		private Tile				holeTile;
		private bool				doomedToFallInHole;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerMoveComponent(Player player) {
			this.player = player;

			// Default settings.
			autoAccelerate			= false;
			moveSpeedScale			= 1.0f;
			moveCondition			= PlayerMoveCondition.FreeMovement;
			canLedgeJump			= true;
			canJump					= true;
			canPush					= true;
			canUseWarpPoint			= true;
			isStrafing				= false;

			// Internal.
			allowMovementControl	= true;
			moveAxes				= new bool[] { false, false };
			motion					= Vector2F.Zero;
			isMoving				= false;
			velocityPrev			= Vector2F.Zero;
			moveAngle				= Angles.South;
			mode					= new PlayerMotionType();
			jumpStartTile			= -Point2I.One;
			doomedToFallInHole		= false;
			holeTile				= null;

			// Controls.
			analogMode		= false;
			analogStick		= GamePad.GetStick(Buttons.LeftStick);
			analogAngle		= 0.0f;
			moveButtons		= new InputControl[4];
			moveButtons[Directions.Up]		= Controls.Up;
			moveButtons[Directions.Down]	= Controls.Down;
			moveButtons[Directions.Left]	= Controls.Left;
			moveButtons[Directions.Right]	= Controls.Right;

			// Normal movement.
			moveModeNormal = new PlayerMotionType();
			moveModeNormal.MoveSpeed			= 1.0f;
			moveModeNormal.CanLedgeJump			= true;
			moveModeNormal.CanRoomChange		= true;
			
			// Slow movement.
			moveModeSlow = new PlayerMotionType();
			moveModeSlow.MoveSpeed				= 0.5f;
			moveModeSlow.CanLedgeJump			= true;
			moveModeSlow.CanRoomChange			= true;
			
			// Ice movement.
			moveModeIce = new PlayerMotionType();
			moveModeIce.MoveSpeed				= 1.0f;
			moveModeIce.CanLedgeJump			= true;
			moveModeIce.CanRoomChange			= true;
			moveModeIce.IsSlippery				= true;
			moveModeIce.Acceleration			= 0.02f;
			moveModeIce.Deceleration			= 0.05f;
			moveModeIce.MinSpeed				= 0.05f;
			moveModeIce.DirectionSnapCount		= 32;
			
			// Air/jump movement.
			moveModeAir = new PlayerMotionType();
			moveModeAir.IsStrafing				= true;
			moveModeAir.MoveSpeed				= 1.0f;
			moveModeAir.CanLedgeJump			= false;
			moveModeAir.CanRoomChange			= false;
			moveModeAir.IsSlippery				= true;
			moveModeAir.Acceleration			= 0.1f;
			moveModeAir.Deceleration			= 0.0f;
			moveModeAir.MinSpeed				= 0.05f;
			moveModeAir.DirectionSnapCount		= 8;//32;
			
			// Water/swim movement.
			moveModeWater = new PlayerMotionType();
			moveModeWater.MoveSpeed				= 0.5f;
			moveModeWater.CanLedgeJump			= true;
			moveModeWater.CanRoomChange			= true;
			moveModeWater.IsSlippery			= true;
			moveModeWater.Acceleration			= 0.08f;
			moveModeWater.Deceleration			= 0.05f;
			moveModeWater.MinSpeed				= 0.05f;
			moveModeWater.DirectionSnapCount	= 32;

			mode = moveModeNormal;
		}
		
		
		//-----------------------------------------------------------------------------
		// Movement
		//-----------------------------------------------------------------------------
		
		public void StopMotion() {
			player.Physics.Velocity = Vector2F.Zero;
			motion = Vector2F.Zero;
		}

		public void Jump() {
			if (player.IsOnGround) {
				// Allow initial jump movement if only can move in air.
				if (moveCondition != PlayerMoveCondition.NoControl && !mode.IsSlippery) {
					Vector2F moveVector = PollMovementKeys(true);

					if (isMoving) {
						float scaledSpeed		= moveModeAir.MoveSpeed * moveSpeedScale;
						Vector2F keyMotion		= moveVector * scaledSpeed;
						player.Physics.Velocity	= keyMotion;
						motion					= keyMotion;
					}
				}
				
				// Jump!
				jumpStartTile = player.RoomControl.GetTileLocation(player.Origin);
				player.Physics.ZVelocity = GameSettings.PLAYER_JUMP_SPEED;
				if (player.CurrentState is PlayerNormalState)
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
			}
			else {
				if (player.CurrentState is PlayerNormalState)
					player.Graphics.PlayAnimation(player.MoveAnimation);
			}
		}

		private Vector2F PollMovementKeys(bool allowMovementControl) {
			Vector2F moveVector = Vector2F.Zero;
			isMoving	= false;
			analogMode	= !analogStick.Position.IsZero;

			if (analogMode) {
				// Check analog stick.
				analogAngle = analogStick.Position.Direction;
				CheckAnalogStick(allowMovementControl);
			}
			else {
				// Check movement keys.
				if (!CheckMoveKey(Directions.Left, allowMovementControl) && !CheckMoveKey(Directions.Right, allowMovementControl))
					moveAxes[0] = false; // x-axis
				if (!CheckMoveKey(Directions.Up, allowMovementControl) && !CheckMoveKey(Directions.Down, allowMovementControl))
					moveAxes[1] = false; // y-axis
			}

			// Update movement or acceleration.
			if (allowMovementControl && (isMoving || autoAccelerate)) {
				if (analogMode)
					moveVector = analogStick.Position;
				else
					moveVector = Angles.ToVector(moveAngle);
			}

			return moveVector;
		}

		private void UpdateMoveControls() {
			// Check if the player is allowed to control his motion.
			allowMovementControl = true;
			if (moveCondition == PlayerMoveCondition.NoControl)
				allowMovementControl = false;
			else if (moveCondition == PlayerMoveCondition.OnlyInAir && player.IsOnGround)
				allowMovementControl = false;
			else if (player.IsInAir && player.Physics.ZVelocity >= 0.1f)
				allowMovementControl = false;

			// Check movement input.
			Vector2F keyMoveVector = PollMovementKeys(allowMovementControl);
				
			// Don't affect the facing direction when strafing
			if (!isStrafing && !mode.IsStrafing && isMoving)
				player.Direction = moveDirection;

			// Don't auto-dodge collisions when moving at an angle.
			player.Physics.SetFlags(PhysicsFlags.AutoDodge,
				Angles.IsHorizontal(moveAngle) || Angles.IsVertical(moveAngle));

			// Update movement or acceleration.
			if (allowMovementControl && (isMoving || autoAccelerate)) {
				if (!isMoving)
					moveAngle = Directions.ToAngle(player.Direction);

				// Determine key-motion (the velocity we want to move at)
				float scaledSpeed = mode.MoveSpeed * moveSpeedScale;
				Vector2F keyMotion = keyMoveVector * scaledSpeed;

				// Update acceleration-based motion.
				if (mode.IsSlippery) {
					// If player velocity has been halted by collisions, mirror that in the motion vector.
					Vector2F velocity = player.Physics.Velocity;
					if (Math.Abs(velocity.X) < Math.Abs(velocityPrev.X) || Math.Sign(velocity.X) != Math.Sign(velocityPrev.X))
						motion.X = velocity.X;
					if (Math.Abs(velocity.Y) < Math.Abs(velocityPrev.Y) || Math.Sign(velocity.Y) != Math.Sign(velocityPrev.Y))
						motion.Y = velocity.Y;

					// Apply acceleration and limit speed.
					motion += keyMotion * mode.Acceleration;
					float newLength = motion.Length;
					if (newLength >= scaledSpeed)
						motion.Length = scaledSpeed;

					// TODO: For jumping, sideways motion should be accelerated somehow.

					// TODO: what does this do again?
					if (Math.Abs(newLength - (motion + (keyMotion * 0.08f)).Length) < mode.Acceleration * 2.0f) {
						motion += keyMotion * 0.04f;
					}

					// Set the players velocity.
					if (mode.DirectionSnapCount > 0) {
						// Snap velocity direction.
						float snapInterval = ((float) GMath.Pi * 2.0f) / mode.DirectionSnapCount;
						float theta = (float) Math.Atan2(-motion.Y, motion.X);
						if (theta < 0)
							theta += (float) Math.PI * 2.0f;
						int angle = (int) ((theta / snapInterval) + 0.5f);
						player.Physics.Velocity = new Vector2F(
							(float)  Math.Cos(angle * snapInterval) * motion.Length,
							(float) -Math.Sin(angle * snapInterval) * motion.Length);
					}
					else {
						// Don't snap velocity direction.
						player.Physics.Velocity = motion;
					}
				}
				else {
					// For non-acceleration based motion, move at regular speed.
					motion = keyMotion;
					player.Physics.Velocity = motion;
				}
			}
			else {
				// Stop movement, using deceleration for slippery motion.
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
			if (player.IsOnGround && moveCondition != PlayerMoveCondition.NoControl &&
				(player.Graphics.Animation == player.MoveAnimation ||
				player.Graphics.Animation == GameData.ANIM_PLAYER_DEFAULT ||
				player.Graphics.Animation == GameData.ANIM_PLAYER_CARRY))
			{
				if (isMoving && !player.Graphics.IsAnimationPlaying)
					player.Graphics.PlayAnimation();
				if (!isMoving && player.Graphics.IsAnimationPlaying)
					player.Graphics.StopAnimation();
			}
		}
		
		// Poll the movement key for the given direction, returning true if
		// it is down. This also manages the strafing behavior of movement.
		private bool CheckMoveKey(int dir, bool allowMovementControl) {
			if (moveButtons[dir].IsDown()) {
				if (allowMovementControl)
					isMoving = true;
			
				if (!moveAxes[(dir + 1) % 2])
					moveAxes[dir % 2] = true;

				if (moveAxes[dir % 2]) {
					moveDirection = dir;
					moveAngle = dir * 2;

					if (moveButtons[(dir + 1) % 4].IsDown())
						moveAngle = (moveAngle + 1) % 8;
					if (moveButtons[(dir + 3) % 4].IsDown())
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
			moveDirection	= (int) GMath.Round(analogAngle / 90.0f) % Directions.Count;
			moveDirection	= Directions.FlipVertical(moveDirection);
		}


		//-----------------------------------------------------------------------------
		// Update
		//-----------------------------------------------------------------------------

		public void Update() {
			// Determine movement mode.
			if (player.Physics.IsInAir)
				mode = moveModeAir;
			else if (player.Physics.IsInWater)
				mode = moveModeWater;
			else if (player.Physics.IsOnIce)
				mode = moveModeIce;
			else if (player.Physics.IsOnLadder || player.Physics.IsOnStairs)
				mode = moveModeSlow;
			else
				mode = moveModeNormal;

			// Update movement.
			UpdateMoveControls();
			velocityPrev = player.Physics.Velocity;

			// Check for falling in holes.
			if (player.Physics.IsInHole) {
				Point2I holeTileLoc = player.RoomControl.GetTileLocation(player.Origin);
				holeTile = player.RoomControl.GetTopTile(holeTileLoc);

				Vector2F position = player.Center;
				Vector2F goalPosition = holeTile.Center;

				Rectangle2F doomRect = new Rectangle2F(holeTile.Position, new Vector2F(16, 16));
				if (doomRect.Contains(player.Origin))
					doomedToFallInHole = true;

				// Pan on each axis seperately
				for (int i = 0; i < 2; i++) {
					float diff = goalPosition[i] - position[i];
					if (Math.Abs(diff) > 0.5f)
						position[i] += Math.Sign(diff) * 0.5f;
					else
						position[i] = goalPosition[i];
				}

				player.SetPositionByCenter(position);
			}
			else {
				doomedToFallInHole = false;
			}
			
			if (doomedToFallInHole && holeTile != null) {
				Rectangle2F doomRect = new Rectangle2F(holeTile.Position, new Vector2F(16, 16));
				// collide with hole boundries.
				if (player.Origin.X + player.Physics.Velocity.X < doomRect.Left) {
					player.Origin = new Vector2F(doomRect.Left, player.Origin.Y);
					player.Physics.Velocity = new Vector2F(0.0f, player.Physics.Velocity.Y);
				}
				if (player.Origin.X + player.Physics.Velocity.X + 1 > doomRect.Right) {
					player.Origin = new Vector2F(doomRect.Right - 1, player.Origin.Y);
					player.Physics.Velocity = new Vector2F(0.0f, player.Physics.Velocity.Y);
				}
				
				if (player.Origin.Y + player.Physics.Velocity.Y < doomRect.Top) {
					player.Origin = new Vector2F(player.Origin.X, doomRect.Top);
					player.Physics.Velocity = new Vector2F(player.Physics.Velocity.X, 0.0f);
				}
				if (player.Origin.Y + player.Physics.Velocity.Y + 1 > doomRect.Bottom) {
					player.Origin = new Vector2F(player.Origin.X, doomRect.Bottom - 1);
					player.Physics.Velocity = new Vector2F(player.Physics.Velocity.X, 0.0f);
				}
			}

			// Check for ledge jumping (ledges/waterfalls)
			CollisionInfo collisionInfo = player.Physics.CollisionInfo[moveDirection];
			if (canLedgeJump && mode.CanLedgeJump && isMoving && collisionInfo.Type == CollisionType.Tile && !collisionInfo.Tile.IsMoving) {
				Tile tile = collisionInfo.Tile;
				
				if (tile.IsLedge &&
					moveDirection == tile.LedgeDirection &&
					collisionInfo.Direction == tile.LedgeDirection)
				{
					// Ledge jump!
					player.LedgeJumpState.LedgeBeginTile = tile;
					player.BeginState(player.LedgeJumpState);
					return;
				}
			}
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool AutoAccelerate {
			get { return autoAccelerate; }
			set { autoAccelerate = value; }
		}
		
		public float MoveSpeedScale {
			get { return moveSpeedScale; }
			set { moveSpeedScale = value; }
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
		
		public bool IsStrafing {
			get { return isStrafing; }
			set { isStrafing = value; }
		}
		
		public bool IsMoving {
			get { return isMoving; }
			set { isMoving = value; }
		}
		
		public bool CanUseWarpPoint {
			get { return canUseWarpPoint; }
			set { canUseWarpPoint = value; }
		}
		
		public int MoveDirection {
			get { return moveDirection; }
		}
		
		public int MoveAngle {
			get { return moveAngle; }
		}
		
		public PlayerMotionType MoveMode {
			get { return mode; }
		}
		
		public PlayerMoveCondition MoveCondition {
			get { return moveCondition; }
			set { moveCondition = value; }
		}

		public Point2I JumpStartTile {
			get { return jumpStartTile; }
			set { jumpStartTile = value; }
		}
	}
}

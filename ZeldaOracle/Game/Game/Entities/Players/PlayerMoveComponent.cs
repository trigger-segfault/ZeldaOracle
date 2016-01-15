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
		private bool				isSprinting;
		private int					sprintTimer;
		private float				sprintSpeedScale;

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
		private bool				isCapeDeployed;
		private Tile				holeTile;
		private bool				doomedToFallInHole;
		private int					holeDoomTimer;
		private Vector2F			holeSlipVelocity;
		private Point2I				holeEnterQuadrent;
		private bool				fallingInHole;

		// Movement modes.
		private PlayerMotionType	mode;
		private PlayerMotionType	moveModeNormal;		// For regular walking.
		private PlayerMotionType	moveModeSlow;		// For climbing ladders and stairs, or when in grass.
		private PlayerMotionType	moveModeGrass;		// For walking in grass.
		private PlayerMotionType	moveModeIce;		// For walking on ice.
		private PlayerMotionType	moveModeAir;		// For jumping
		private PlayerMotionType	moveModeWater;		// For swimming.


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
			isSprinting				= false;
			sprintTimer				= 0;
			sprintSpeedScale		= 1.5f;

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
			moveModeSlow = new PlayerMotionType(moveModeNormal);
			moveModeSlow.MoveSpeed = 0.5f;
			
			// Grass movement.
			moveModeGrass = new PlayerMotionType(moveModeNormal);
			moveModeGrass.MoveSpeed = 0.75f;
			
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

		public void StartSprinting(int duration, float sprintSpeedScale) {
			this.sprintTimer		= duration;
			this.sprintSpeedScale	= sprintSpeedScale;
			this.isSprinting		= true;
		}

		public void Jump() {
			if (player.IsOnGround) {
				// Allow initial jump movement if only can move in air.
				if (moveCondition != PlayerMoveCondition.NoControl && !mode.IsSlippery) {
					Vector2F moveVector = PollMovementKeys(true);

					if (isMoving) {
						float scaledSpeed		= moveModeAir.MoveSpeed * moveSpeedScale;
						if (isSprinting && mode != moveModeWater)
							scaledSpeed *= sprintSpeedScale;
						Vector2F keyMotion		= moveVector * scaledSpeed;
						player.Physics.Velocity	= keyMotion;
						motion					= keyMotion;
					}
				}
				
				// Jump!
				isCapeDeployed = false;
				jumpStartTile = player.RoomControl.GetTileLocation(player.Position);
				player.Physics.ZVelocity = GameSettings.PLAYER_JUMP_SPEED;
				if (player.CurrentState is PlayerNormalState)
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
			}
			else {
				if (player.CurrentState is PlayerNormalState)
					player.Graphics.PlayAnimation(player.MoveAnimation);
			}
		}

		public void DeployCape() {
			// 23 frame delay from jump start
			if (player.IsInAir && !isCapeDeployed && player.Physics.ZVelocity -
				GameSettings.DEFAULT_GRAVITY <= -GameSettings.PLAYER_CAPE_REQUIRED_FALLSPEED)
			{
				player.Physics.ZVelocity = GameSettings.PLAYER_CAPE_JUMP_SPEED + GameSettings.PLAYER_CAPE_GRAVITY;
				 isCapeDeployed = true;
				if (player.CurrentState is PlayerNormalState)
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_CAPE);
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
					moveAxes[Axes.X] = false;
				if (!CheckMoveKey(Directions.Up, allowMovementControl) && !CheckMoveKey(Directions.Down, allowMovementControl))
					moveAxes[Axes.Y] = false;
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
			else if (player.IsOnGround && player.IsBeingKnockedBack)
				allowMovementControl = false;
			else if (moveCondition == PlayerMoveCondition.OnlyInAir && player.IsOnGround)
				allowMovementControl = false;
			else if (player.IsInAir && player.Physics.ZVelocity >= 0.1f)
				allowMovementControl = false;

			if (player.IsInMinecart)
				allowMovementControl = true;

			// Check movement input.
			Vector2F keyMoveVector = PollMovementKeys(allowMovementControl);
			
			// Don't affect the facing direction when strafing
			if (!isStrafing && !mode.IsStrafing && isMoving)
				player.Direction = moveDirection;

			if (player.IsInMinecart)
				player.MinecartState.Update();

			// Update movement or acceleration.
			if (allowMovementControl && (isMoving || autoAccelerate) && !player.IsInMinecart) {
				if (!isMoving)
					moveAngle = Directions.ToAngle(player.Direction);

				// Determine key-motion (the velocity we want to move at)
				float scaledSpeed = mode.MoveSpeed * moveSpeedScale;
				if (isSprinting && mode != moveModeWater)
					scaledSpeed *= sprintSpeedScale;
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
						player.Physics.Velocity = Vector2F.SnapDirectionByCount(motion, mode.DirectionSnapCount);
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
			if (player.IsOnGround && !player.IsInMinecart &&
				moveCondition != PlayerMoveCondition.NoControl && 
				(player.Graphics.Animation == player.MoveAnimation ||
				player.Graphics.Animation == GameData.ANIM_PLAYER_DEFAULT ||
				player.Graphics.Animation == GameData.ANIM_PLAYER_CARRY ||
				player.Graphics.Animation == GameData.ANIM_PLAYER_MINECART_IDLE))
			{
				// Play/stop the move animation.
				if (isMoving || isSprinting) {
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
				player.Graphics.Animation = GameData.ANIM_PLAYER_CAPE;
			else if (player.IsOnGround && player.Graphics.Animation == GameData.ANIM_PLAYER_CAPE)
				player.Graphics.Animation = player.MoveAnimation;
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

		private void UpdateFallingInHoles() {
			if (fallingInHole) {
				holeDoomTimer--;

				if (doomedToFallInHole) {
					// The player is doomed to fall in the hole, he cannot escape it.
					if (holeDoomTimer < 0)
						player.Physics.Velocity = Vector2F.Zero;
					
					// Collide with hole boundries.
					Rectangle2F holeRect = new Rectangle2F(holeTile.Position, new Vector2F(16, 16));
					Rectangle2F collisionBox = new Rectangle2F(Vector2F.Zero, Vector2F.One);
					player.Physics.PerformInsideEdgeCollisions(collisionBox, holeRect);
				}
				else if (!player.Physics.IsInHole) {
					// Stop falling in a hole.
					fallingInHole		= false;
					doomedToFallInHole	= false;
					holeTile			= null;
					return;
				}
				else {
					// Check if the player has changed quadrents.
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
					if (Math.Abs(diff) > 0.6f) {
						float dist = 0.25f;
						
						// Pull the player in more if he's moving away from the hole.
						if ((diff < 0 && player.Physics.Velocity[i] > 0.25f) ||
							(diff > 0 && player.Physics.Velocity[i] < -0.25f))
							dist = 0.5f;

						if (!(diff < 0 && player.Physics.Velocity[i] < -0.25f) &&
							!(diff > 0 && player.Physics.Velocity[i] > 0.25f))
						{
							holeSlipVelocity[i] = Math.Sign(diff) * dist;
						}
					}
				}
				player.Position += holeSlipVelocity;
					
				// Fall in the hole when close to the center.
				if (player.Center.DistanceTo(holeTile.Center) <= 1.0f) {
					player.SetPositionByCenter(holeTile.Center);
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_FALL_HOLE);
					player.RespawnDeath();
					holeTile			= null;
					fallingInHole		= false;
					doomedToFallInHole	= false;
				}
			}
			else if (player.Physics.IsInHole && player.CurrentState != player.RespawnDeathState) {
				// Start falling in a hole.
				Point2I holeTileLoc = player.RoomControl.GetTileLocation(player.Position);
				holeTile			= player.RoomControl.GetTopTile(holeTileLoc);
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
			// Determine movement mode.
			if (player.Physics.IsInAir)
				mode = moveModeAir;
			else if (player.Physics.IsInWater)
				mode = moveModeWater;
			else if (player.Physics.IsOnIce)
				mode = moveModeIce;
			else if (player.Physics.IsOnLadder || player.Physics.IsOnStairs)
				mode = moveModeSlow;
			else if (player.Physics.IsInGrass)
				mode = moveModeGrass;
			else
				mode = moveModeNormal;

			// Update movement.
			UpdateMoveControls();
			UpdateFallingInHoles();
			velocityPrev = player.Physics.Velocity;

			// Update sprinting.
			if (isSprinting) {
				if (sprintTimer % 10 == 0) {
					//Sounds.PLAYER_LAND.play();
					player.RoomControl.SpawnEntity(
						new Effect(GameData.ANIM_EFFECT_SPRINT_PUFF, DepthLayer.EffectSprintPuff),
						player.Position);
				}
				sprintTimer--;
				if (sprintTimer <= 0)
					isSprinting = false;
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
		
		public bool IsSprinting {
			get { return isSprinting; }
		}
		
		public int MoveDirection {
			get { return moveDirection; }
		}
		
		public int MoveAngle {
			get { return moveAngle; }
		}

		public bool IsCapeDeployed {
			get { return (player.IsInAir && isCapeDeployed); }
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

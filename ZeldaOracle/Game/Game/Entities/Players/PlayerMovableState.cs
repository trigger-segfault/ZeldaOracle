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
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerMovableState : PlayerState {

		private Keys[]		moveKeys;				// The 4 movement keys for each direction.
		private bool[]		moveAxes;				// Which axes the player is moving on.
		protected bool		isMoving;				// Is the player holding down a movement key?
		private Vector2F	motion;					// The vector that's driving the player's velocity.
		private Vector2F	velocityPrev;			// The player's velocity on the previous frame.

		protected float		moveSpeed;				// The top-speed for movement.
		protected float		moveSpeedScale;			// Scales the movement speed to create the actual top-speed.
		protected bool		isSlippery;				// Is the movement acceleration-based?
		protected float		acceleration;			// Acceleration when moving.
		protected float		deceleration;			// Deceleration when not moving.
		protected float		minSpeed;				// Minimum speed threshhold used to jump back to zero when decelerating.
		protected bool		autoAccelerate;			// Should the player still accelerate without holding down a movement key?
		protected int		directionSnapCount;		// The number of intervals movement directions should snap to for acceleration-based movement.
		protected bool		allowMovementControl;	// Is the player allowed to control his movement?


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerMovableState() : base() {
			// Internal.
			moveAxes	= new bool[] { false, false };
			motion		= Vector2F.Zero;
			isMoving	= false;

			// Controls.
			moveKeys = new Keys[4];
			moveKeys[Directions.Up]		= Keys.Up;
			moveKeys[Directions.Down]	= Keys.Down;
			moveKeys[Directions.Left]	= Keys.Left;
			moveKeys[Directions.Right]	= Keys.Right;

			// Movement settings.
			allowMovementControl	= true;
			moveSpeed				= GameSettings.PLAYER_MOVE_SPEED; // 0.5f for swimming, 1.5f for sprinting.
			moveSpeedScale			= 1.0f;
			acceleration			= 0.08f; // 0.08f for ice/swimming, 0.1f for jumping
			deceleration			= 0.05f;
			minSpeed				= 0.05f;
			isSlippery				= false;
			autoAccelerate			= false;
			directionSnapCount		= 0;	// 8 for swimming/jumping, 16 for ice.
		}
		
		
		//-----------------------------------------------------------------------------
		// Movement.
		//-----------------------------------------------------------------------------

		private void UpdateMoveControls() {
			isMoving = false;

			// Check movement keys.
			if (!CheckMoveKey(Directions.Left) && !CheckMoveKey(Directions.Right))
				moveAxes[0] = false;	// x-axis
			if (!CheckMoveKey(Directions.Down) && !CheckMoveKey(Directions.Up))
				moveAxes[1] = false;	// y-axis

			// Don't auto-dodge collisions when moving at an angle.
			player.Physics.SetFlags(PhysicsFlags.AutoDodge,
				Angles.IsHorizontal(player.Angle) || Angles.IsVertical(player.Angle));

			// Update movement or acceleration.
			if (allowMovementControl && (isMoving || (autoAccelerate && isSlippery))) {
				if (!isMoving)
					player.Angle = Directions.ToAngle(player.Direction);

				float scaledSpeed = moveSpeed * moveSpeedScale;
				Vector2F keyMotion = Angles.ToVector(Player.Angle) * scaledSpeed; // The velocity we want to move at.

				// Update acceleration-based motion.
				if (isSlippery) {
					// If player velocity has been halted by collisions, mirror that in the motion vector.
					Vector2F velocity = player.Physics.Velocity;
					if (Math.Abs(velocity.X) < Math.Abs(velocityPrev.X) || Math.Sign(velocity.X) != Math.Sign(velocityPrev.X))
						motion.X = velocity.X;
					if (Math.Abs(velocity.Y) < Math.Abs(velocityPrev.Y) || Math.Sign(velocity.Y) != Math.Sign(velocityPrev.Y))
						motion.Y = velocity.Y;

					// Apply acceleration and limit speed.
					motion += keyMotion * acceleration;
					float newLength = motion.Length;
					if (newLength >= scaledSpeed)
						motion.Length = scaledSpeed;

					// TODO: what does this do again?
					if (Math.Abs(newLength - (motion + (keyMotion * 0.08f)).Length) < acceleration * 2.0f) {
						motion += keyMotion * 0.04f;
					}

					// Set the players velocity.
					if (directionSnapCount > 0) {
						// Snap velocity direction.
						float snapInterval = ((float) GMath.Pi * 2.0f) / directionSnapCount;
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
					Player.Physics.Velocity = motion;
				}
			}
			else {
				// Stop movement, using deceleration for slippery motion.
				if (isSlippery) {
					float length = motion.Length;
					if (length < minSpeed)
						motion = Vector2F.Zero;
					else
						motion.Length = length - deceleration;
					player.Physics.Velocity = motion;
				}
				else {
					motion = Vector2F.Zero;
					Player.Physics.Velocity = Vector2F.Zero;
				}
			}
		}
		
		// Poll the movement key for the given direction, returning true if
		// it is down. This also manages the strafing behavior of movement.
		private bool CheckMoveKey(int dir) {
			if (Keyboard.IsKeyDown(moveKeys[dir])) {
				isMoving = true;
			
				if (!moveAxes[(dir + 1) % 2])
					moveAxes[dir % 2] = true;
				if (moveAxes[dir % 2]) {
					Player.Direction = dir;
					Player.Angle = dir * 2;
			
					if (Keyboard.IsKeyDown(moveKeys[(dir + 1) % 4])) 
						Player.Angle = (Player.Angle + 1) % 8;
					if (Keyboard.IsKeyDown(moveKeys[(dir + 3) % 4]))
						Player.Angle = (Player.Angle + 7) % 8;
				}
				return true;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			player.Angle	= Directions.ToAngle(player.Direction);
			motion			= player.Physics.Velocity;
		}
		
		public override void OnEnd() {
			player.Angle = Directions.ToAngle(player.Direction);
		}

		public override void Update() {
			UpdateMoveControls();
			velocityPrev = player.Physics.Velocity;
		}

	}
}

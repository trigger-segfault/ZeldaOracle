using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerLedgeJumpState : PlayerState {

		protected Vector2F	velocity;
		protected bool		ledgeExtendsToNextRoom;
		protected bool		hasRoomChanged;
		protected int		direction;
		protected bool		isHoldingSword;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerLedgeJumpState() {
			ledgeExtendsToNextRoom = false;
			isHoldingSword = false;
		}

		private bool CanLandAtPosition(Vector2F position) {
			foreach (Tile tile in player.Physics.GetTilesMeeting(position, CollisionBoxType.Hard)) {
				if (tile.IsSolid && tile.CollisionStyle == CollisionStyle.Rectangular && !(tile is TileColorBarrier) && !tile.IsBreakable) {
					return false;
				}
			}
			return true;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			StateParameters.EnableAutomaticRoomTransitions	= true;
			StateParameters.EnableStrafing					= true;
			StateParameters.DisableSolidCollisions			= true;
			StateParameters.DisableInteractionCollisions	= true;
			StateParameters.DisablePlayerControl			= true;

			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);

			// Play the jump animation
			if (player.WeaponState == null)
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
			else
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);

			// Face the ledge direction
			if (!player.StateParameters.EnableStrafing)
				player.Direction = direction;

			// Find the landing position, calculating the move distance in pixels
			Vector2F pos = player.Position + Directions.ToVector(direction);
			int distance = 0;
			while (!CanLandAtPosition(pos)) {
				distance += 1;
				pos += Directions.ToVector(direction);
			}
			
			if (!player.RoomControl.RoomBounds.Contains(pos)) {
				// Fake jumping by using the xy-velocity instead of the z-velocity.
				hasRoomChanged = false;
				velocity = new Vector2F(0.0f, -1.0f);
				player.Physics.ZVelocity = 0;
				ledgeExtendsToNextRoom = true;
			}
			else {
				// Small ledge distances have special jump speeds.
				float jumpSpeed = 1.5f;
				if (distance >= 28)
					jumpSpeed = 2.0f;
				else if (distance >= 20)
					jumpSpeed = 1.75f;

				// Calculate the movement speed based on jump speed, knowing
				// they should take the same amount of time to perform.
				float jumpTime = (2.0f * jumpSpeed) / GameSettings.DEFAULT_GRAVITY;
				float speed    = distance / jumpTime;//  GMath.Clamp((float) distance / jumpTime, 0.7f, 2.5f);//5.0f);

				// For larger ledges, calculate the speed so that both
				// the movement speed and the jump speed equal eachother.
				if (speed > 1.5f) {
					speed = GMath.Sqrt(0.5f * distance * GameSettings.DEFAULT_GRAVITY);
					jumpSpeed = speed;
				}

				velocity = Directions.ToVector(direction) * speed;
				player.Physics.ZVelocity = jumpSpeed;
				ledgeExtendsToNextRoom = false;
			}
			
			player.Physics.Velocity = velocity;
			player.Position += velocity;
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_JUMP);
		}
		
		public override void OnEnd(PlayerState newState) {
		}

		public override void OnEnterRoom() {
			if (ledgeExtendsToNextRoom) {
				hasRoomChanged = true;

				// Find the landing position, calculating the movement
				// distance in pixels.
				Vector2F pos = player.Position;
				int distance = 0;
				while (!CanLandAtPosition(pos)) {
					distance += 1;
					pos += Directions.ToVector(direction);
				}

				// Move the player to be on the landing spot, with a z-position
				// of the movement distance.
				player.ZPosition = pos.Y - player.Position.Y;
				player.Position = pos;
				player.Physics.ZVelocity = -velocity.Y;
				player.Physics.Velocity = Vector2F.Zero;
			}
		}

		public override void Update() {
			base.Update();

			bool isDone = false;
			
			// Update velocity while checking we've reached the landing spot.
			if (ledgeExtendsToNextRoom) {
				if (hasRoomChanged) {
					isDone = player.IsOnGround;
				}
				else {
					velocity.Y += GameSettings.DEFAULT_GRAVITY;
					player.Physics.Velocity = velocity;
				}
			}
			else {
				player.Physics.Velocity = velocity;
				isDone = CanLandAtPosition(player.Position);
			}

			if (isDone) {
				player.Physics.Velocity = Vector2F.Zero;

				// If we landed on a tile, then break it
				player.LandOnSurface();

				if (ledgeExtendsToNextRoom)
					player.MarkRespawn();

				End();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int LedgeJumpDirection {
			get { return direction; }
			set { direction = value; }
		}
	}
}

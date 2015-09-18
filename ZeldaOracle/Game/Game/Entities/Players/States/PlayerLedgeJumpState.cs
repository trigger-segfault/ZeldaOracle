using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerLedgeJumpState : PlayerState {
		
		private Vector2F	velocity;
		private Tile		ledgeBeginTile;
		private bool		ledgeExtendsToNextRoom;
		private bool		hasRoomChanged;
		private int			direction;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerLedgeJumpState() {
			ledgeBeginTile = null;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			direction = ledgeBeginTile.LedgeDirection;

			// TODO: player.passable = true;
			player.CheckGroundTiles			= false;
			player.AutoRoomTransition		= true;
			player.Physics.CollideWithWorld = false;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);

			// Find the landing position, calculating the move distance in pixels.
			Vector2F pos = player.Position + Directions.ToVector(direction);
			int distance = 0;
			while (player.Physics.IsPlaceMeetingSolid(pos, player.Physics.CollisionBox)) {
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
		}
		
		public override void OnEnd() {
			player.CheckGroundTiles			= true;
			player.AutoRoomTransition		= false;
			player.Physics.CollideWithWorld = true;
			base.OnEnd();
		}

		public override void OnEnterRoom() {
			if (ledgeExtendsToNextRoom) {
				hasRoomChanged = true;
				
				// Find the landing position, calculating the movement distance in pixels.
				Vector2F pos = player.Position;
				int distance = 0;
				while (player.Physics.IsPlaceMeetingSolid(pos, player.Physics.CollisionBox)) {
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
				isDone = !player.Physics.IsPlaceMeetingSolid(player.Position, player.Physics.CollisionBox);
			}

			// If done, return to the normal player state.
			if (isDone) {
				player.Physics.Velocity = Vector2F.Zero;
				player.BeginNormalState();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Tile LedgeBeginTile {
			get { return ledgeBeginTile; }
			set { ledgeBeginTile = value; }
		}
	}
}

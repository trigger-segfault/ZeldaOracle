using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerLedgeJumpState : PlayerState {

		protected Vector2F	velocity;
		protected bool		ledgeExtendsToNextRoom;
		protected bool		hasRoomChanged;
		protected Direction		direction;
		protected Vector2F	landingPosition;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerLedgeJumpState() {
			StateParameters.ProhibitReleasingSword			= true;
			StateParameters.EnableAutomaticRoomTransitions	= true;
			StateParameters.EnableStrafing					= true;
			StateParameters.DisableSolidCollisions			= true;
			StateParameters.DisableInteractionCollisions	= true;
			StateParameters.DisablePlayerControl			= true;
		}

		private Vector2F GetLandingPosition(Vector2F position) {
			Vector2F moveVector = Directions.ToVector(direction);
			Vector2F landingPosition = position + (moveVector * 4);
			while (!CanLandAtPosition(landingPosition))
				landingPosition += Directions.ToVector(direction);
			landingPosition += moveVector;
			return landingPosition;
		}

		private bool CanLandAtPosition(Vector2F position) {
			Rectangle2F collisionBox = player.Physics.CollisionBox;

			// Inset the sides of the collision box to allow edge clipping
			Vector2F edgeClipping = Vector2F.Zero;
			int lateralAxis = Axes.GetOpposite(Directions.ToAxis(direction));
			edgeClipping[lateralAxis] = player.Physics.EdgeClipAmount;
			collisionBox.Inflate(-edgeClipping);

			foreach (Tile tile in
				player.Physics.GetSolidTilesMeeting(position, collisionBox))
			{
				if (tile.CollisionStyle == CollisionStyle.Rectangular &&
					!(tile is TileColorBarrier) && !tile.IsBreakable)
				{
					return false;
				}
			}
			return true;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.StopPushing();

			// Play the jump animation
			if (player.WeaponState == null)
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
			else
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);

			// Face the ledge direction
			if (!player.StateParameters.EnableStrafing)
				player.Direction = direction;

			// Find the landing position
			landingPosition = GetLandingPosition(player.Position);
			
			if (!player.RoomControl.RoomBounds.Contains(landingPosition)) {
				// The ledge extends into the next room
				// Fake jumping by using the y-velocity instead of the z-velocity
				ledgeExtendsToNextRoom = true;
				hasRoomChanged = false;
				velocity = new Vector2F(0.0f, -1.0f);
				player.Physics.ZVelocity = 0;
			}
			else {
				// Determine the jump speed based on the distance needed to move
				// Smaller ledge distances have slower jump speeds
				float distance = (landingPosition - player.Position).Dot(
					Directions.ToVector(direction));
				float jumpSpeed = 1.5f;
				if (distance >= 28)
					jumpSpeed = 2.0f;
				else if (distance >= 20)
					jumpSpeed = 1.75f;

				// Calculate the movement speed based on jump speed, knowing
				// they should take the same amount of time to perform.
				float jumpTime = (2.0f * jumpSpeed) / GameSettings.DEFAULT_GRAVITY;
				float speed = distance / jumpTime;

				// For longer ledge distances, calculate the speed so that both
				// the movement speed and the jump speed equal each other.
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
			player.Physics.Velocity = Vector2F.Zero;

			// If we landed on a tile, then break it
			player.LandOnSurface();

			if (ledgeExtendsToNextRoom)
				player.MarkRespawn();
		}

		public override void OnEnterRoom() {
			if (ledgeExtendsToNextRoom) {
				hasRoomChanged = true;

				// Find the landing position in this room
				landingPosition = GetLandingPosition(player.Position);
				
				// Move the player to be at the landing spot, and raise its z-position
				// so that it falls onto the landing spot
				player.ZPosition = landingPosition.Y - player.Position.Y;
				player.Position = landingPosition;
				player.Physics.ZVelocity = -velocity.Y;
				player.Physics.Velocity = Vector2F.Zero;
			}
		}

		public override void Update() {
			// Update velocity while checking we've reached the landing spot
			if (ledgeExtendsToNextRoom) {
				if (hasRoomChanged) {
					// End once the player has fallen to the ground
					if (player.IsOnGround)
						End();
				}
				else {
					velocity.Y += GameSettings.DEFAULT_GRAVITY;
					player.Physics.Velocity = velocity;
				}
			}
			else {
				player.Physics.Velocity = velocity;

				// End once the player has reached the landing position
				if ((player.Position + velocity - landingPosition).Dot(
					Directions.ToVector(direction)) >= 0.0f)
				{
					player.Position = landingPosition;
					End();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Direction LedgeJumpDirection {
			get { return direction; }
			set { direction = value; }
		}
	}
}

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSideScrollLadderEnvironmentState : PlayerEnvironmentState {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public PlayerSideScrollLadderEnvironmentState() {
			StateParameters.DisableGravity			= true;
			StateParameters.EnableGroundOverride	= true;
			StateParameters.AlwaysFaceUp			= true;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Return the top-most ladder the player is colliding with when
		/// placed at the given position</summary>
		private Tile GetHighestLadder(Vector2F position) {
			Rectangle2F pollLadderBox = player.Movement.ClimbCollisionBox;
			pollLadderBox.Point += position;
			Tile highestLadder = null;
			foreach (Tile tile in RoomControl.TileManager.GetTilesTouching(pollLadderBox)) {
				if (tile.IsLadder && (highestLadder == null || tile.Bounds.Top < highestLadder.Bounds.Top))
					highestLadder = tile;
			}
			return highestLadder;
		}

		/// <summary>Check for climbing off the top of the ladder</summary>
		private bool CheckClimbingToLadderTop(Player player, Tile ladderTile) {
			Rectangle2F collisionBox = player.Physics.PositionedCollisionBox;
			Rectangle2F collisionBoxNext = Rectangle2F.Translate(
				collisionBox, player.Physics.Velocity);
			Rectangle2F ladderBox = ladderTile.Bounds;
			Rectangle2F climbBox = Rectangle2F.Translate(
				player.Movement.ClimbCollisionBox, player.Position);

			if (climbBox.Intersects(ladderBox) &&
				collisionBox.Bottom > ladderBox.Top &&
				collisionBoxNext.Bottom <= ladderBox.Top)
			{
				player.Physics.VelocityY = 0.0f;
				player.Y = ladderBox.Top - player.Physics.CollisionBox.Bottom;
				player.Physics.MovementCollisions[Directions.Down] = true;
				if (!player.Physics.CollisionInfo[Directions.Down].IsColliding)
					player.Physics.CollisionInfo[Directions.Down].SetCollision(ladderTile, Directions.Down);
				End();
				return true;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
		}

		public override void OnEnd(PlayerState newState) {
			// Any downwards safe clipping should be turned into an actual collision
			// now that gravity is affecting the player again.
			if (player.Physics.ClipCollisionInfo[Directions.Down].IsAllowedClipping &&
				!player.Physics.CollisionInfo[Directions.Down].IsColliding)
			{
				player.Physics.MovementCollisions[Directions.Down] = true;
				player.Physics.CollisionInfo[Directions.Down].SetCollision(
					player.Physics.ClipCollisionInfo[Directions.Down].CollidedObject,
					Directions.Down);
			}
			// TODO: play jump animation if falling?
		}

		public override bool CanTransitionToState(PlayerState state) {
			if (!RoomControl.IsSideScrolling)
				return true;
			if (state == player.SideScrollSwimState)
				return true;
			// Do not allow automatic transitions to other environment states
			return false;
		}

		public override void Update() {
			// Get the ladder the player is touching
			Tile ladder = GetHighestLadder(player.Position);

			// Check if not on a ladder anymore
			if (ladder == null) {
				End();
				return;
			}

			// Check for climbing to the top of a ladder
			if (CheckClimbingToLadderTop(player, ladder))
				return;

			// Check for climbing to the bottom of a ladder
			if (player.Physics.CollisionInfo[Directions.Down].IsCollidingAndNotAutoDodged &&
				player.Movement.IsMoving && player.Movement.MoveDirection == Directions.Down)
			{
				End();
				return;
			}
		}
	}
}

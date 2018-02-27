using System.Linq;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerMinecartState : PlayerState {
		
		private float minecartSpeed;
		private float moveDistance;
		private Minecart minecart;
		private Point2I tileLocation;
		private Direction direction;
		private TileMinecartTrack trackTile;
		private Vector2F playerOffset;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerMinecartState() {
			minecartSpeed	= 1.0f;
			minecart		= null;
			trackTile		= null;

			StateParameters.EnableAutomaticRoomTransitions	= true;
			StateParameters.DisableInteractionCollisions	= true;
			StateParameters.DisableSolidCollisions			= true;
			StateParameters.DisableSurfaceContact			= true;
			StateParameters.PlayerAnimations.Default		= GameData.ANIM_PLAYER_MINECART_IDLE;
			StateParameters.PlayerAnimations.Aim			= GameData.ANIM_PLAYER_MINECART_AIM;
			StateParameters.PlayerAnimations.Throw			= GameData.ANIM_PLAYER_MINECART_THROW;
			StateParameters.PlayerAnimations.Swing			= GameData.ANIM_PLAYER_MINECART_SWING;
			StateParameters.PlayerAnimations.SwingNoLunge	= GameData.ANIM_PLAYER_MINECART_SWING_NOLUNGE;
			StateParameters.PlayerAnimations.SwingBig		= GameData.ANIM_PLAYER_MINECART_SWING_BIG;
			StateParameters.PlayerAnimations.Carry			= GameData.ANIM_PLAYER_MINECART_CARRY;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			tileLocation	= minecart.TrackTileLocation;
			trackTile		= null;
			direction		= Direction.Invalid;
			
			// Check if the minecart is actually on a track
			if (minecart.TrackTile == null) {
				ExitMinecart(player.Direction.Reverse());
				return;
			}

			// Determine start direction
			foreach (Direction dir in minecart.TrackTile.GetDirections()) {
				bool isStop;
				if (MoveInDirection(dir, out isStop))
					break;
			}
			
			// Exit the minecart if there are no possible directions to move in
			if (!direction.IsValid) {
				ExitMinecart(player.Direction.Reverse());
				return;
			}

			minecart.StartMoving();
			moveDistance = 0.0f;
			UpdatePlayerPosition();
			
			// Notify the current player state we have entered a minecart
			foreach (PlayerState state in player.ActiveStates)
				state.OnEnterMinecart();
		}
		
		public override void OnEnd(PlayerState newState) {
			// Reset changed player state variables
			player.ViewFocusOffset = Vector2F.Zero;

			// Notify the current player state we have exited the minecart
			foreach (PlayerState state in player.ActiveStates)
				state.OnExitMinecart();
		}

		public override void OnEnterRoom() {
			if (direction == Direction.Right)
				tileLocation.X -= player.RoomControl.Room.Width;
			if (direction == Direction.Left)
				tileLocation.X += player.RoomControl.Room.Width;
			if (direction == Direction.Down)
				tileLocation.Y -= player.RoomControl.Room.Height;
			if (direction == Direction.Up)
				tileLocation.Y += player.RoomControl.Room.Height;

			Vector2F center = player.Center - playerOffset;
			Point2I startLoc = tileLocation - direction.ToPoint();
			Vector2F startPosCenter = (startLoc + Vector2F.Half) * GameSettings.TILE_SIZE;
			Vector2F directionVector = direction.ToVector();

			moveDistance = directionVector.Dot(center - startPosCenter);
		}

		public override void Update() {			
			// Update movement
			moveDistance += minecartSpeed;
			if (moveDistance >= GameSettings.TILE_SIZE) {
				// Move to the next tile
				if (!MoveToNextTile()) {
					ExitMinecart(direction);
					return;
				}
			}

			UpdatePlayerPosition();
			
			AudioSystem.LoopSoundWhileActive(GameData.SOUND_MINECART_LOOP);
		}
		

		//-----------------------------------------------------------------------------
		// Navigation Methods
		//-----------------------------------------------------------------------------

		/// <summary>Exit the minecart, dropping the player off in the given
		/// direction.</summary>
		private void ExitMinecart(Direction direction) {
			minecart.StopMoving(trackTile, tileLocation);
			
			// Hop out of the minecart
			Point2I landingTile = tileLocation + direction.ToPoint();
			player.JumpOutOfMinecart(landingTile);
		}

		/// <summary>Update the player's position relative to the minecart.</summary>
		private void UpdatePlayerPosition() {
			// Determine player offset based on minecart animation
			if (minecart.Graphics.AnimationPlayer.PlaybackTime < 6) { // TODO: Magic number
				if (direction.IsHorizontal)
					playerOffset = new Vector2F(0, -8);
				else
					playerOffset = new Vector2F(-1, -9);
			}
			else {
				if (direction.IsHorizontal)
					playerOffset = new Vector2F(0, -9);
				else
					playerOffset = new Vector2F(0, -9);
			}
			player.ViewFocusOffset = -playerOffset;
			
			// Set the player position
			Vector2F nextTileCenter =
				(tileLocation + Vector2F.Half) * GameSettings.TILE_SIZE;
			player.SetPositionByCenter(nextTileCenter -
				direction.ToVector(GameSettings.TILE_SIZE) +
				direction.ToVector(
					GMath.Min(moveDistance, GameSettings.TILE_SIZE)));
			player.Position += playerOffset;

			minecart.AttachmentOffset = player.CenterOffset -
				new Vector2F(8, 8) - playerOffset;
		}

		/// <summary>Attempt to move along the track in the given direction.</summary>
		private bool MoveInDirection(Direction moveDirection, out bool isStop) {
			Point2I nextLocation = tileLocation + moveDirection.ToPoint();
			
			// Find the next track tile and check for obstructions.
			int comeFromDirection = moveDirection.Reverse();
			TileMinecartTrack nextTrackTile;
			if (!FindTrackTile(nextLocation,
				comeFromDirection, out nextTrackTile, out isStop))
				return false;

			moveDistance	-= GameSettings.TILE_SIZE;
			direction		= moveDirection;
			trackTile		= nextTrackTile;
			tileLocation	= nextLocation;
			minecart.Graphics.SubStripIndex = direction.Axis;
			
			// Open any minecart doors in the next tile.
			if (player.RoomControl.IsTileInBounds(nextLocation)) {
				for (int i = 0; i < player.RoomControl.Room.LayerCount; i++) {
					TileMinecartDoor tileDoor = player.RoomControl.GetTile(
						tileLocation, i) as TileMinecartDoor;
					if (tileDoor != null && tileDoor.Direction == comeFromDirection)
						tileDoor.Open();
				}
			}

			return true;
		}

		// Attempt to move to the next tile along the track.
		private bool MoveToNextTile() {
			Direction comeFromDirection = direction.Reverse();

			// Look for a track tile.
			if (trackTile == null) {
				bool isStop;
				FindTrackTile(tileLocation, direction, out trackTile, out isStop);
				if (trackTile == null)
					return false;
			}

			// Get the next direction to move in.
			for (int i = 0; i < Direction.Count; i++) {
				Direction dir = comeFromDirection.Rotate(i + 1);
				if (trackTile.GetDirections().Contains(dir)) {
					bool isStop;
					if (MoveInDirection(dir, out isStop))
						return true;
					if (isStop)
						return false;
				}
			}

			return false;
		}
		
		// Check if it is okay to move to a location with a track tile that has the given direction.
		// Outputs the track tile that was found,
		// And outputs true to isStop if there was a stop-point at the given locatin.
		// Returns true if it is okay to move to the given tile.
		private bool FindTrackTile(Point2I location, int direction, out TileMinecartTrack track, out bool isStop) {
			isStop = false;
			track = null;

			if (!player.RoomControl.IsTileInBounds(location))
				return true;

			for (int i = player.RoomControl.Room.LayerCount - 1; i >= 0; i--) {
				Tile tile = player.RoomControl.GetTile(location, i);
				
				if (tile != null) {
					TileMinecartTrack trackTile = tile as TileMinecartTrack;

					if (trackTile != null && trackTile.GetDirections().Contains(direction)) {
						track = trackTile;
						return true;
					}
					if (tile is TileMinecartStop) {
						isStop = true;
						return false;
					}
					// Minecart doors are not obstructions, but other solid tiles are.
					if (tile.IsSolid && !(tile is TileMinecartDoor)) {
						return false;
					}
				}
			}

			return false;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Minecart Minecart {
			get { return minecart; }
			set { minecart = value; }
		}
	}
}

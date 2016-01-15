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
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Players.States.SwingStates;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerMinecartState : PlayerState {
		
		private float minecartSpeed;
		private float moveDistance;
		private Minecart minecart;
		private Point2I tileLocation;
		private int direction;
		private TileMinecartTrack trackTile;
		private AnimationPlayer minecartAnimationPlayer;
		private Vector2F playerOffset;
		private TileDataInstance startingTrackTileData;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerMinecartState() {
			minecartSpeed	= 1.0f;
			minecart		= null;
			trackTile		= null;
			minecartAnimationPlayer = new AnimationPlayer();
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			tileLocation	= minecart.TrackTileLocation;
			trackTile		= null;
			direction		= -1;
			
			if (minecart.TrackTile == null) {
				ExitMinecart(Directions.Reverse(player.Direction));
				return;
			}

			startingTrackTileData = minecart.TrackTile.TileData;
			if (startingTrackTileData != null)
				startingTrackTileData.Properties.Set("minecart", false);

			// Determine start direction.
			foreach (int dir in minecart.TrackTile.GetDirections()) {
				if (MoveInDirection(dir))
					break;
			}
			
			// Error, no directions available to move in.
			if (direction < 0) {
				ExitMinecart(Directions.Reverse(player.Direction));
				return;
			}

			// No other player states should change these variables while in a minecart.
			player.AutoRoomTransition			= true;
			player.Physics.CollideWithWorld		= false; 
			player.Physics.CollideWithEntities	= false;

			// Play the animations.
			player.MoveAnimation = GameData.ANIM_PLAYER_MINECART_IDLE;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_MINECART_IDLE);
			minecartAnimationPlayer.Play(GameData.ANIM_MINECART);

			// Setup position.
			moveDistance = 0.0f;
			UpdatePlayerPosition();
						
			// Destroy the minecart.
			minecart.Destroy();
		}
		
		public override void OnEnd(PlayerState newState) {
			// Reset changed player state variables.
			player.AutoRoomTransition			= false;
			player.Physics.CollideWithWorld		= true;
			player.Physics.CollideWithEntities	= true;
			player.ViewFocusOffset				= Vector2F.Zero;
			
			// Revert to default player animation.
			if (player.MoveAnimation == GameData.ANIM_PLAYER_MINECART_IDLE)
				player.MoveAnimation = GameData.ANIM_PLAYER_DEFAULT;

			// Notify the current player state we have exited the minecart.
			if (player.CurrentState != null)
				player.CurrentState.OnExitMinecart();
		}

		public override void OnEnterRoom() {
			if (direction == Directions.Right)
				tileLocation.X -= player.RoomControl.Room.Width;
			if (direction == Directions.Left)
				tileLocation.X += player.RoomControl.Room.Width;
			if (direction == Directions.Down)
				tileLocation.Y -= player.RoomControl.Room.Height;
			if (direction == Directions.Up)
				tileLocation.Y += player.RoomControl.Room.Height;

			Vector2F center = player.Center - playerOffset;
			Point2I startLoc = tileLocation - Directions.ToPoint(direction);
			Vector2F startPosCenter = (startLoc + new Vector2F(0.5f, 0.5f)) * GameSettings.TILE_SIZE;
			Vector2F directionVector = Directions.ToVector(direction);

			moveDistance = directionVector.Dot(center - startPosCenter);
		}

		public override void Update() {
			base.Update();

			// Update minecart animation.
			minecartAnimationPlayer.Update();
			
			// Keep the default player animation to be in a minecart.
			if (player.MoveAnimation == GameData.ANIM_PLAYER_DEFAULT)
				player.MoveAnimation = GameData.ANIM_PLAYER_MINECART_IDLE;

			// Update movement.
			moveDistance += minecartSpeed;
			if (moveDistance >= GameSettings.TILE_SIZE) {
				// Move to the next tile.
				if (!MoveToNextTile()) {
					ExitMinecart(direction);
					return;
				}
			}

			UpdatePlayerPosition();
		}

		public override void DrawUnder(RoomGraphics g) {
			// Draw the minecart below the player.
			g.DrawAnimation(minecartAnimationPlayer,
				player.Center - new Vector2F(8, 8) - playerOffset, DepthLayer.PlayerAndNPCs);

		}


		//-----------------------------------------------------------------------------
		// Navigation methods
		//-----------------------------------------------------------------------------

		// Exit the minecart, dropping the player off in the given direction.
		private void ExitMinecart(int direction) {
			// End of the track.
			player.SetPositionByCenter(
				((tileLocation + new Vector2F(0.5f, 0.5f)) * GameSettings.TILE_SIZE) +
				(Directions.ToVector(direction) * GameSettings.TILE_SIZE));

			// Spawn another minecart entity.
			if (minecart == null || minecart.IsDestroyed) {
				minecart = new Minecart(trackTile);
				player.RoomControl.SpawnEntity(minecart, tileLocation * GameSettings.TILE_SIZE);
				minecart = null;
			}

			if (trackTile != null)
				trackTile.Properties.SetBase("minecart", true);

			// End the minecart state.
			End(null);
		}

		// Update the player's position relative to the minecart.
		private void UpdatePlayerPosition() {
			// Determine player offset based on minecart animation.
			if (minecartAnimationPlayer.PlaybackTime < 12) {
				if (Directions.IsHorizontal(direction))
					playerOffset = new Vector2F(0, -8);
				else
					playerOffset = new Vector2F(-1, -9);
			}
			else {
				if (Directions.IsHorizontal(direction))
					playerOffset = new Vector2F(0, -9);
				else
					playerOffset = new Vector2F(0, -9);
			}
			player.ViewFocusOffset = -playerOffset;
			
			// Set the player position.
			Vector2F nextTileCenter = (tileLocation + new Vector2F(0.5f, 0.5f)) * GameSettings.TILE_SIZE;
			player.SetPositionByCenter(
				nextTileCenter - (Directions.ToVector(direction) * GameSettings.TILE_SIZE)
				+ (Directions.ToVector(direction) * Math.Min(moveDistance, GameSettings.TILE_SIZE)));
			player.Position += playerOffset;
		}

		// Attempt to move along the track in the given directin.
		private bool MoveInDirection(int moveDirection) {
			Point2I nextLocation = tileLocation + Directions.ToPoint(moveDirection);

			// Find the next track tile and check for obstructions.
			int comeFromDirection = Directions.Reverse(moveDirection);
			TileMinecartTrack nextTrackTile = GetTrackTile(nextLocation, comeFromDirection);

			bool isNextTileInBounds = player.RoomControl.IsTileInBounds(nextLocation);
			if (nextTrackTile == null && isNextTileInBounds)
				return false;
			
			moveDistance -= GameSettings.TILE_SIZE;
			direction		= moveDirection;
			trackTile		= nextTrackTile;
			tileLocation	= nextLocation;
			minecartAnimationPlayer.SubStripIndex = (Directions.IsHorizontal(direction) ? 0 : 1);
			
			// Open any minecart doors in the next tile.
			if (isNextTileInBounds) {
				for (int i = 0; i < player.RoomControl.Room.LayerCount; i++) {
					TileMinecartDoor tileDoor = player.RoomControl.GetTile(tileLocation, i) as TileMinecartDoor;
					if (tileDoor != null && tileDoor.Direction == comeFromDirection)
						tileDoor.Open();
				}
			}

			return true;
		}

		// Attempt to move to the next tile along the track.
		private bool MoveToNextTile() {
			int comeFromDirection = Directions.Reverse(direction);

			// Look for a track tile.
			if (trackTile == null) {
				trackTile = GetTrackTile(tileLocation, direction);
				if (trackTile == null)
					return false;
			}

			// Get the next direction to move in.
			foreach (int dir in trackTile.GetDirections()) {
				if (dir != comeFromDirection)
					return MoveInDirection(dir);
			}

			return false;
		}
		
		// Get the track tile at the location that has the given direction.
		// Returns NULL if there are obstructions over the track.
		private TileMinecartTrack GetTrackTile(Point2I location, int direction) {
			if (!player.RoomControl.IsTileInBounds(location))
				return null;

			for (int i = player.RoomControl.Room.LayerCount - 1; i >= 0; i--) {
				Tile tile = player.RoomControl.GetTile(location, i);
				
				if (tile != null) {
					TileMinecartTrack trackTile = tile as TileMinecartTrack;

					if (trackTile != null && trackTile.GetDirections().Contains(direction))
						return trackTile;

					// Minecart doors are not obstructions, but other solid tiles are.
					if (tile.IsSolid && !(tile is TileMinecartDoor))
						return null;
				}
			}

			return null;
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

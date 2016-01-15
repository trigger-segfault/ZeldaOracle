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
			// Determine start direction.
			TileMinecartTrack tile = minecart.TrackTile;
			tileLocation = tile.Location;
			direction = -1;
			trackTile = null;
			foreach (int dir in minecart.TrackTile.GetDirections()) {
				trackTile = GetNextTrackTile(tileLocation, dir);
				if (trackTile != null) {
					direction = dir;
					break;
				}
			}

			// Error, no directions available to move in.
			if (trackTile == null) {
				End(null);
				return;
			}
			// Begin moving to the next tile.
			if (!MoveToNextTile()) {
				End(null);
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

		private void ExitMinecart(int direction) {
			// End of the track.
			player.SetPositionByCenter(
				((tileLocation + new Vector2F(0.5f, 0.5f)) * GameSettings.TILE_SIZE) +
				(Directions.ToVector(direction) * GameSettings.TILE_SIZE));

			// Spawn another minecart entity.
			if (minecart == null || minecart.IsDestroyed) {
				minecart = new Minecart(trackTile);
				player.RoomControl.SpawnEntity(minecart);
				minecart = null;
			}

			// End the minecart state.
			End(null);
		}

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
			player.SetPositionByCenter(
				trackTile.Center - (Directions.ToVector(direction) * GameSettings.TILE_SIZE)
				+ (Directions.ToVector(direction) * Math.Min(moveDistance, GameSettings.TILE_SIZE)));
			player.Position += playerOffset;
		}

		// Get the tracktile in the direction, or NULL if the path is blocked.
		private TileMinecartTrack GetNextTrackTile(Point2I location, int direction) {
			int comeFromDirection = Directions.Reverse(direction);
			Point2I nextLocation = tileLocation + Directions.ToPoint(direction);

			for (int i = player.RoomControl.Room.LayerCount - 1; i >= 0; i--) {
				Tile tile = player.RoomControl.GetTile(nextLocation, i);
				
				if (tile != null) {
					TileMinecartTrack trackTile = tile as TileMinecartTrack;
					if (trackTile != null && trackTile.GetDirections().Contains(comeFromDirection))
						return trackTile;
					else if (!(tile is TileMinecartDoor))
						return null;
				}
			}
			return null;
		}

		// Move to the next track tile.
		private bool MoveToNextTile() {
			moveDistance -= GameSettings.TILE_SIZE;
			tileLocation += Directions.ToPoint(direction);

			// Get the next direction to move in.
			int comeFromDirection = Directions.Reverse(direction);
			int nextDirection = -1;
			foreach (int dir in trackTile.GetDirections()) {
				if (dir != comeFromDirection) {
					nextDirection = dir;
					break;
				}
			}
			if (nextDirection < 0)
				return false;
			
			// Get the track tile we are moving toward.
			TileMinecartTrack nextTackTile = GetNextTrackTile(tileLocation, nextDirection);
			if (nextTackTile == null)
				return false;

			trackTile = nextTackTile;
			direction = nextDirection;
			minecartAnimationPlayer.SubStripIndex = (Directions.IsHorizontal(direction) ? 0 : 1);
			
			// Open any minecart doors in the next tile.
			for (int i = 0; i < player.RoomControl.Room.LayerCount; i++) {
				TileMinecartDoor tileDoor = player.RoomControl.GetTile(trackTile.Location, i) as TileMinecartDoor;
				if (tileDoor != null)
					tileDoor.Open();
			}

			return true;
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

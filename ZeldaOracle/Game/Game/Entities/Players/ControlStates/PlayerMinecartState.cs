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
using ZeldaOracle.Common.Audio;

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

			StateParameters.EnableAutomaticRoomTransitions	= true;
			StateParameters.DisableInteractionCollisions	= true;
			StateParameters.DisableSolidCollisions			= true;
			StateParameters.PlayerAnimations.Default		= GameData.ANIM_PLAYER_MINECART_IDLE;
			StateParameters.PlayerAnimations.Aim			= GameData.ANIM_PLAYER_MINECART_AIM;
			StateParameters.PlayerAnimations.Throw			= GameData.ANIM_PLAYER_MINECART_THROW;
			StateParameters.PlayerAnimations.Swing			= GameData.ANIM_PLAYER_MINECART_SWING;
			StateParameters.PlayerAnimations.SwingNoLunge	= GameData.ANIM_PLAYER_MINECART_SWING_NOLUNGE;
			StateParameters.PlayerAnimations.Carry			= GameData.ANIM_PLAYER_MINECART_CARRY;
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

			// Remove the miencart from the track.
			minecart.TrackTile.SpawnsMinecart = false;

			// Determine start direction.
			foreach (int dir in minecart.TrackTile.GetDirections()) {
				bool isStop;
				if (MoveInDirection(dir, out isStop))
					break;
			}
			
			// Error, no directions available to move in.
			if (direction < 0) {
				ExitMinecart(Directions.Reverse(player.Direction));
				return;
			}

			// Play the animations.
			player.MoveAnimation = GameData.ANIM_PLAYER_MINECART_IDLE;
			if (player.WeaponState != player.CarryState)
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_MINECART_IDLE);
			minecartAnimationPlayer.Play(GameData.ANIM_MINECART);

			// Setup position.
			moveDistance = 0.0f;
			UpdatePlayerPosition();
						
			// Destroy the minecart.
			minecart.Destroy();
			
			// Notify the current player state we have entered a minecart.
			foreach (PlayerState state in player.ActiveStates)
				state.OnEnterMinecart();
		}
		
		public override void OnEnd(PlayerState newState) {
			// Reset changed player state variables.
			player.ViewFocusOffset = Vector2F.Zero;
			
			// Revert to default player animation.
			if (player.MoveAnimation == GameData.ANIM_PLAYER_MINECART_IDLE)
				player.MoveAnimation = GameData.ANIM_PLAYER_DEFAULT;

			// Notify the current player state we have exited the minecart.
			foreach (PlayerState state in player.ActiveStates)
				state.OnExitMinecart();
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

			AudioSystem.LoopSoundWhileActive(GameData.SOUND_MINECART_LOOP);

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

			// Collide with monsters.
			
		}

		public override void DrawUnder(RoomGraphics g) {
			// Draw the minecart below the player.
			g.DrawAnimationPlayer(minecartAnimationPlayer,
				player.Center - new Vector2F(8, 8) - playerOffset, DepthLayer.PlayerAndNPCs);

		}


		//-----------------------------------------------------------------------------
		// Navigation methods
		//-----------------------------------------------------------------------------

		// Exit the minecart, dropping the player off in the given direction.
		private void ExitMinecart(int direction) {
			// Spawn another minecart entity.
			if (minecart == null || minecart.IsDestroyed) {
				minecart = new Minecart(trackTile);
				player.RoomControl.SpawnEntity(minecart, tileLocation * GameSettings.TILE_SIZE);
				minecart = null;
			}

			// Make the current track tile remember it has a minecart on it.
			if (trackTile != null)
				trackTile.SpawnsMinecart = true;

			// Hop out of the minecart.
			Point2I landingTile = tileLocation + Directions.ToPoint(direction);
			player.JumpOutOfMinecart(landingTile);
		}

		// Update the player's position relative to the minecart.
		private void UpdatePlayerPosition() {
			// Determine player offset based on minecart animation.
			if (minecartAnimationPlayer.PlaybackTime < 6) { // TODO: Magic number
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

		// Attempt to move along the track in the given direction.
		private bool MoveInDirection(int moveDirection, out bool isStop) {
			Point2I nextLocation = tileLocation + Directions.ToPoint(moveDirection);
			
			// Find the next track tile and check for obstructions.
			int comeFromDirection = Directions.Reverse(moveDirection);
			TileMinecartTrack nextTrackTile;
			if (!FindTrackTile(nextLocation, comeFromDirection, out nextTrackTile, out isStop))
				return false;

			moveDistance	-= GameSettings.TILE_SIZE;
			direction		= moveDirection;
			trackTile		= nextTrackTile;
			tileLocation	= nextLocation;
			minecartAnimationPlayer.SubStripIndex = (Directions.IsHorizontal(direction) ? 0 : 1);
			
			// Open any minecart doors in the next tile.
			if (player.RoomControl.IsTileInBounds(nextLocation)) {
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
				bool isStop;
				FindTrackTile(tileLocation, direction, out trackTile, out isStop);
				if (trackTile == null)
					return false;
			}

			// Get the next direction to move in.
			for (int i = 0; i < Directions.Count; i++) {
				int dir = (comeFromDirection + i + 1) % Directions.Count;
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

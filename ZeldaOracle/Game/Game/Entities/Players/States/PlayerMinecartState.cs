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

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerMinecartState : PlayerState {
		
		private float minecartSpeed;
		private float moveDistance;
		private Minecart minecart;
		private Point2I tileLocation;
		private int direction;
		private TileMinecartTrack trackTile;
		private AnimationPlayer minecartAnimationPlayer;


		//-----------------------------------------------------------------------------
		// Constructors
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
			
			player.AutoRoomTransition			= true;
			player.Physics.CollideWithWorld		= false;
			player.Physics.CollideWithEntities	= false;
			//player.IsStateControlled			= true;
			player.Movement.MoveCondition		= PlayerMoveCondition.FixedPosition; // Fixed in place with pivot?
			//player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);

			TileMinecartTrack tile = minecart.TrackTile;
			minecart.Destroy();
			tileLocation = tile.Location;

			// Determine start direction.
			direction = -1;
			trackTile = null;
			foreach (int dir in minecart.TrackTile.GetDirections()) {
				trackTile = GetNextTrackTile(tileLocation, dir);
				if (trackTile != null) {
					direction = dir;
					break;
				}
			}

			moveDistance = 0.0f;

			// Error, no directions available to move in.
			if (trackTile == null) {
				player.BeginState(player.NormalState);
				return;
			}

			if (!MoveToNextTile()) {
				player.BeginState(player.NormalState);
				return;
			}
			
			player.SetPositionByCenter(minecart.Center);
		}
		
		public override void OnEnd(PlayerState newState) {
			
			player.AutoRoomTransition			= false;
			player.Physics.CollideWithWorld		= true;
			player.Physics.CollideWithEntities	= true;
			//player.IsStateControlled			= false;
			player.Movement.MoveCondition		= PlayerMoveCondition.FreeMovement;
			
		}

		public override void Update() {
			base.Update();

			minecartAnimationPlayer.Update();

			moveDistance += minecartSpeed;
			
			player.SetPositionByCenter(
				trackTile.Center - (Directions.ToVector(direction) * GameSettings.TILE_SIZE)
				+ (Directions.ToVector(direction) * Math.Min(moveDistance, GameSettings.TILE_SIZE)));

			if (moveDistance >= GameSettings.TILE_SIZE) {
				if (MoveToNextTile()) {
					// Success
				}
				else {
					// End of the track.
					player.BeginState(player.NormalState);
					player.Position += Directions.ToVector(direction) * GameSettings.TILE_SIZE;
					// Spawn another minecart entity.
					Minecart minecart = new Minecart(trackTile);
					player.RoomControl.SpawnEntity(minecart);
				}
			}
		}

		public override void DrawUnder(RoomGraphics g) {
			g.DrawAnimation(minecartAnimationPlayer, player.Center - new Vector2F(8, 8), DepthLayer.PlayerAndNPCs);
		}


		//-----------------------------------------------------------------------------
		// Navigation methods
		//-----------------------------------------------------------------------------

		private TileMinecartTrack GetNextTrackTile(Point2I location, int direction) {
			int comeFromDirection = Directions.Reverse(direction);
			Point2I nextLocation = tileLocation + Directions.ToPoint(direction);

			for (int i = player.RoomControl.Room.LayerCount - 1; i >= 0; i--) {
				Tile tile = player.RoomControl.GetTile(nextLocation, i);
				
				if (tile != null) {
					TileMinecartTrack trackTile = tile as TileMinecartTrack;
					if (trackTile != null && trackTile.GetDirections().Contains(comeFromDirection))
						return trackTile;
					else
						return null;
				}
			}
			return null;
		}

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
			
			if (Directions.IsHorizontal(direction))
				minecartAnimationPlayer.Play(GameData.ANIM_MINECART_HORIZONTAL);
			else
				minecartAnimationPlayer.Play(GameData.ANIM_MINECART_VERTICAL);
			
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

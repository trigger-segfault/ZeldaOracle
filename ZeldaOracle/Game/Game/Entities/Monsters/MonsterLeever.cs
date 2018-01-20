using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterLeever : Monster {
		
		private enum LeeverState {
			Burrowed,
			Burrowing,
			Unburrowing,
			Unburrowed,
		}
		
		private float moveSpeed;
		private int timer;
		private int duration;
		private LeeverState leeverState;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterLeever() {
			// General
			healthMax		= 2;
			ContactDamage	= 2;

			// Graphics
			syncAnimationWithDirection	= false;
			color						= MonsterColor.Orange;

			// Movement
			moveSpeed = 0.5f;
		}

		
		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private bool CanUnburrowAtLocation(Point2I location) {
			Tile tile = RoomControl.GetTopTile(location);
			return (tile == null || !tile.IsSolid);
		}

		private bool GetRandomLocation(out Point2I location) {
			List<Point2I> possibleLocations = new List<Point2I>();

			// Get a list of all non-solid tile locations
			for (int x = 0; x < RoomControl.Room.Width; x++) {
				for (int y = 0; y < RoomControl.Room.Height; y++) {
					Point2I loc = new Point2I(x, y);
					if (CanUnburrowAtLocation(loc))
						possibleLocations.Add(loc);
				}
			}

			if (possibleLocations.Count == 0) {
				location = Point2I.Zero;
				return false;
			}
			
			// Pick a random location
			int index = GRandom.NextInt(possibleLocations.Count);
			location = possibleLocations[index];
			return true;
		}

		private void Burrow() {
			leeverState = LeeverState.Burrowing;
			duration	= 100;
			timer		= 0;
			IsPassable	= true;
			Physics.Velocity = Vector2F.Zero;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_LEEVER_BURROW);
		}

		private bool Unburrow() {
			if (color == MonsterColor.Red) {
				// Unborrow between 3 to 5 tiles in front of the player
				// First create a list of the possibly locations to unburrow
				List<Point2I> possibleLocations = new List<Point2I>();
				Point2I dirPoint = Directions.ToPoint(RoomControl.Player.Direction);
				Point2I loc = RoomControl.GetTileLocation(RoomControl.Player.Position);

				for (int i = 0; i <= 5 && RoomControl.IsTileInBounds(loc); i++) {
					Tile t = RoomControl.GetTopTile(loc);
					if (i >= 3 && CanUnburrowAtLocation(loc))
						possibleLocations.Add(loc);
					loc += dirPoint;
				}

				if (possibleLocations.Count > 0) {
					// Randomly pick one of the possible unburrow locations
					int index = GRandom.NextInt(possibleLocations.Count);
					SetPositionByCenter(possibleLocations[index] *
						GameSettings.TILE_SIZE + new Vector2F(8, 8));
				}
				else {
					// No possible unburrow locations.
					return false;
				}
			}
			else if (color == MonsterColor.Blue) {
				// Unburrow at a random location in the room
				Point2I location;
				if (GetRandomLocation(out location)) {
					SetPositionByCenter(location *
						GameSettings.TILE_SIZE + new Vector2F(8, 8));
				}
				else {
					// No possible unburrow locations.
					return false;
				}
			}

			duration = 180;
			timer = 0;
			Direction = Directions.NearestFromVector(
				RoomControl.Player.Center - Center);
			Graphics.IsVisible = true;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_LEEVER_UNBURROW);
			leeverState = LeeverState.Unburrowing;
			return true;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_LEEVER);
						
			leeverState = LeeverState.Burrowed;
			timer = 0;
			duration = 100;
			IsPassable = true;
			Graphics.IsVisible = false;
		}

		public override void UpdateAI() {
			timer++;

			if (leeverState == LeeverState.Burrowing) {
				if (Graphics.IsAnimationDone) {
					Graphics.IsVisible = false;
					leeverState = LeeverState.Burrowed;
				}
			}
			else if (leeverState == LeeverState.Unburrowing) {
				if (Graphics.IsAnimationDone) {
					IsPassable = false;
					Graphics.PlayAnimation(GameData.ANIM_MONSTER_LEEVER);
					leeverState = LeeverState.Unburrowed;
				}
			}
			else if (leeverState == LeeverState.Burrowed) {
				if (timer > duration) {
					Unburrow();
				}
			}
			else {
				if (timer > duration || (Physics.IsColliding &&
					color != MonsterColor.Orange))
				{
					// Burrow after a delay or upon hitting a wall
					Burrow();
				}
				else if (color == MonsterColor.Red) {
					physics.Velocity = Directions.ToVector(Direction) * moveSpeed;
				}
				else if (color == MonsterColor.Blue) {
					// Re-face player regularly.
					if (timer % 30 == 0) {
						Direction = Directions.NearestFromVector(
							RoomControl.Player.Center - Center);
					}
					physics.Velocity = Directions.ToVector(Direction) * moveSpeed;
				}
				else if (color == MonsterColor.Orange) {
					// Chase player
					Vector2F vectorToPlayer = RoomControl.Player.Center - Center;
					physics.Velocity = vectorToPlayer.Normalized * moveSpeed;
				}
			}
		}
	}
}

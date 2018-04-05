using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles;

namespace ZeldaOracle.Game.Tiles.Custom {
	public class TileArrowShooter : Tile {

		private int timer;
		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			timer = 0;
		}

		public override void Update() {
			if (timer == 0) {
				/*bool playerDetected = false;
				// TODO: Implement a better way for tiles to scan for the player?
				CollisionTestSettings settings = new CollisionTestSettings(typeof(Player), ViewBox, CollisionBoxType.Soft);
				for (int i = 0; i < RoomControl.Entities.Count; i++) {
					Player other = RoomControl.Entities[i] as Player;
					if (other != null && CollisionTest.PerformCollisionTest(Position, other, settings).IsColliding) {
						playerDetected = true;
						break;
					}
				}*/
				Rectangle2F playerBox =
					RoomControl.Player.Interactions.PositionedInteractionBox;
				if (playerBox.Intersects(ViewBox)) {
					// Shoot an arrow
					MonsterArrow projectile = new MonsterArrow(true);
					ShootFromDirection(projectile, Direction, 2f, Directions.ToVector(Direction) * 2f);

					// Set the duration before the next shot
					timer = GameSettings.TILE_ARROW_SHOOTER_SHOOT_INTERVAL;
				}
			}

			if (timer > 0) {
				timer--;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Direction Direction {
			get { return Properties.Get<int>("direction", Direction.Down); }
		}

		public Rectangle2F ViewBox {
			get {
				Vector2F start, distance;
				if (Direction == Direction.Left || Direction == Direction.Up) {
					start = Vector2F.Zero;
					distance = Position;
				}
				else {
					start = Position + GameSettings.TILE_SIZE;
					distance = RoomControl.Room.PixelSize - start;
				}
				if (Directions.IsHorizontal(Direction)) {
					return new Rectangle2F(start.X, Position.Y - 3,
						distance.X, GameSettings.TILE_SIZE + 6);
				}
				else if (Directions.IsVertical(Direction)) {
					return new Rectangle2F(Position.X - 3, start.Y,
						GameSettings.TILE_SIZE + 6, distance.Y);
				}
				return Rectangle2F.Zero;
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Properties.Set("direction", Direction.Right)
				.SetDocumentation("Direction", "direction", "", "Arrow Shooter", "The direction arrows are shot in.").Hide();
		}
	}
}

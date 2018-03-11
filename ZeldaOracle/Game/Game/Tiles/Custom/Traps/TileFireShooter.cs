using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles;

namespace ZeldaOracle.Game.Tiles.Custom {
	public class TileFireShooter : Tile {
		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Update() {
			// Determine if it's time to shoot
			int time = RoomControl.CurrentRoomTicks;
			// Every other tile shoots at a slightly different time.
			if (IsSecondaryTile)
				time -= GameSettings.TILE_FIRE_SHOOTER_SHOOT_OFFSET;
			time = GMath.Wrap(time, GameSettings.TILE_FIRE_SHOOTER_SHOOT_INVERVAL);

			if (time == 0) {
				// Shoot a fireball
				FireShooterProjectile projectile = new FireShooterProjectile();
				ShootFromDirection(projectile, Direction, 2f, Directions.ToVector(Direction) * 2f);
				AudioSystem.PlaySound(GameData.SOUND_FIRE_SHOOTER);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Direction Direction {
			get { return Properties.GetInteger("direction", Direction.Down); }
		}

		public bool IsSecondaryTile {
			get { return (Location.X + Location.Y) % 2 == 0; }
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Properties.Set("direction", Direction.Right)
				.SetDocumentation("Direction", "direction", "", "Fire Shooter", "The direction fire is shot in.").Hide();
		}
	}
}

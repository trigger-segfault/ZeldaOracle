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

		public const int SHOOT_INVTERVAL = 16;
		public const int SHOOT_TIMER_OFFSET = 7;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileFireShooter() { }


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			
		}

		public override void Update() {
			int time = RoomControl.CurrentRoomTicks;
			// Every other tile shoots at a slightly different time.
			if ((Location.X + Location.Y) % 2 == 0)
				time += SHOOT_INVTERVAL - SHOOT_TIMER_OFFSET;
			time %= SHOOT_INVTERVAL;

			if (time == 0) {
				// Shoot a fireball
				FireShooterProjectile projectile = new FireShooterProjectile();
				ShootFromDirection(projectile, Direction, 2f, Directions.ToVector(Direction) * 2);
				AudioSystem.PlaySound(GameData.SOUND_MONSTER_DIE);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Direction {
			get { return Properties.GetInteger("direction", Directions.Down); }
		}


		//-----------------------------------------------------------------------------
		// Projectiles
		//-----------------------------------------------------------------------------
		
		public Projectile ShootFromDirection(Projectile projectile, int direction, float speed, Vector2F positionOffset, int zPositionOffset = 0) {
			//projectile.Owner        = this;
			projectile.TileOwner	= this;
			projectile.Direction	= direction;
			projectile.Physics.Velocity = Directions.ToVector(direction) * speed;
			RoomControl.SpawnEntity(projectile, Center + positionOffset, zPositionOffset);
			return projectile;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Tile.DrawTileData(g, args);
		}
	}
}

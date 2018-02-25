﻿using System;
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
	public class TileArrowCannon : Tile {
		
		private int timer;
		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			// Set the initial time before the first shot
			int startCount = GameSettings.TILE_ARROW_CANNON_SHOOT_STARTS.Length;
			timer = GameSettings.TILE_ARROW_CANNON_SHOOT_STARTS[GRandom.NextInt(startCount)];
		}

		public override void Update() {
			if (timer == 0) {
				// Shoot an arrow
				MonsterArrow projectile = new MonsterArrow(true);
				ShootFromDirection(projectile, Direction, 2f, Directions.ToVector(Direction) * 6f);

				// Set the duration before the next shot
				int intervalCount = GameSettings.TILE_ARROW_CANNON_SHOOT_INTERVALS.Length;
				timer = GameSettings.TILE_ARROW_CANNON_SHOOT_INTERVALS[GRandom.NextInt(intervalCount)];
			}
			
			timer--;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Direction {
			get { return Properties.GetInteger("direction", Directions.Down); }
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Tiles {

	public class TilePlatform : Tile {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilePlatform() {
			fallsInHoles = false;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			/*
			path = new TilePath();
			path.Repeats = true;

			path.AddMove(Directions.Down,	1, 0.5f, 0);
			path.AddMove(Directions.Up,		4, 0.5f, 0);
			path.AddMove(Directions.Down,	3, 0.5f, 0);
			*/
			
		}

		public override void Update() {
			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			//base.Draw(g);

			for (int y = 0; y < Height; y++) {
				for (int x = 0; x < Width; x++) {
					g.DrawSprite(GameData.SPR_TILE_MOVING_PLATFORM, Zone.ImageVariantID,
						Position + new Point2I(x, y) * GameSettings.TILE_SIZE, DepthLayer.TileLayer1);
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Propreties
		//-----------------------------------------------------------------------------

		public override bool IsSurface {
			get { return false; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Tiles {

	public class TilePlatform : Tile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilePlatform() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {

		}

		public override void Update() {
			base.Update();

		}

		public override void Draw(Graphics2D g) {
			//base.Draw(g);

			for (int y = 0; y < Height; y++) {
				for (int x = 0; x < Width; x++) {
					g.DrawSprite(GameData.SPR_TILE_MOVING_PLATFORM, Zone.ImageVariantID, Position + new Point2I(x, y) * GameSettings.TILE_SIZE);
				}
			}
		}
	}
}

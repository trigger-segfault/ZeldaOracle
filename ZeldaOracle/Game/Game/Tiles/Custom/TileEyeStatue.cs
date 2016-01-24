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

	public class TileEyeStatue : Tile {
		private Point2I eyeOffset;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileEyeStatue() {
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		public void UpdateEyeOffset() {
			Player player = RoomControl.Player;

			if (player != null) {
				Vector2F lookVector = player.Center - Center;
				int lookAngle = Angles.NearestFromVector(lookVector);
				eyeOffset = Angles.ToPoint(lookAngle);
			}
			else {
				eyeOffset = Point2I.Zero;
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			eyeOffset = Point2I.Zero;
		}

		public override void Update() {
			base.Update();
			UpdateEyeOffset();
		}

		public override void Draw(Graphics2D g) {
			base.Draw(g);

			g.DrawSprite(GameData.SPR_TILE_STATUE_EYE, Position + eyeOffset);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;

namespace ZeldaOracle.Game.Tiles {

	public class TileSeedBouncer : Tile, ZeldaAPI.SeedBouncer {

		// Seed bouncers can be in 8 possible angles, but really only 4
		// of them are unique (because each angle has an equal opposite angle).
		private int angle;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileSeedBouncer() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			angle = Properties.GetInteger("angle", 0);

			Graphics.PlayAnimation(GameData.ANIM_TILE_SEED_BOUNCER);
			Graphics.SubStripIndex = angle;

			fallsInHoles = false;
		}


		//-----------------------------------------------------------------------------
		// Zelda API methods
		//-----------------------------------------------------------------------------

		public void RotateClockwise(int amount = 1) {
			Angle = Angles.Add(angle, amount, WindingOrder.Clockwise);
		}
		
		public void RotateCounterClockwise(int amount = 1) {
			Angle = Angles.Add(angle, amount, WindingOrder.CounterClockwise);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Angle {
			get { return angle; }
			set {
				angle = value;
				Graphics.SubStripIndex = angle;
			}
		}
	}
}

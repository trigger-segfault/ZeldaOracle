using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Tiles {

	public class TileSomariaBlock : Tile {


		public TileSomariaBlock() {

		}

		public override void OnInitialize() {
			DropList = null;
		}

		public override void OnFallInHole() {
			Break(false);
		}
		
		public override void OnFallInWater() {
			Break(false);
		}
		
		public override void OnFallInLava() {
			Break(false);
		}
	}
}

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

	public class TileSeedBouncer : Tile {

		private int angle;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileSeedBouncer() {
			animationPlayer = new AnimationPlayer();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		/*
		public override void OnSeedHit(SeedType seedType, SeedEntity seed) {
			if (seedType == SeedType.Mystery && !isActivated) {
				isActivated		= true;
				sparkleIndex	= 0;
				timer			= 0;
			}
		}*/

		public override void OnInitialize() {
			angle = Properties.GetInteger("angle", 0);
			animationPlayer.Play(GameData.ANIM_TILE_SEED_BOUNCER);
			animationPlayer.SubStripIndex = angle;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Angle {
			get { return angle; }
		}
	}
}

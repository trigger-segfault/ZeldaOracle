using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Effects {
	public class Fire : Effect {


		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Fire() :
			base(GameData.ANIM_EFFECT_SEED_EMBER)
		{
			graphics.DrawOffset = new Point2I(-8, -8);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
		}

		public override void OnDestroy() {
			// Burn tiles.
			Point2I location = RoomControl.GetTileLocation(position);
			
			if (RoomControl.IsTileInBounds(location)) {
				for (int i = 0; i < RoomControl.Room.LayerCount; i++) {
					Tile tile = RoomControl.GetTile(location, i);
					if (tile != null) {
						tile.OnBurn();
					}
				}
			}
		}

		public override void Update() {
			base.Update();
			// TODO: collide with monsters.
		}
	}
}

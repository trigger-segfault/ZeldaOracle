using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Tiles {

	public class TileLever : SwitchTileBase, ZeldaAPI.Lever {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileLever() {

		}
		

		//-----------------------------------------------------------------------------
		// Lever Methods
		//-----------------------------------------------------------------------------

		public override void OnToggle(bool switchState) {/*
			if (switchState)
				CustomSprite = GameData.SPR_TILE_LEVER_RIGHT;
			else
				CustomSprite = GameData.SPR_TILE_LEVER_LEFT;*/
			SpriteIndex = (switchState ? 1 : 0);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();
			/*
			if (SwitchState)
				CustomSprite = GameData.SPR_TILE_LEVER_RIGHT;
			else
				CustomSprite = GameData.SPR_TILE_LEVER_LEFT;*/
			SpriteIndex = (SwitchState ? 1 : 0);
		}
	}
}

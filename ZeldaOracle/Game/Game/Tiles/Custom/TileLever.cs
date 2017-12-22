using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
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

		public override void OnToggle(bool switchState) {
			AudioSystem.PlaySound(GameData.SOUND_SWITCH);
		}
		
		public override void SetSwitchState(bool switchState) {
			base.SetSwitchState(switchState);
			//if (switchState)
				//Graphics.PlaySprite(GameData.SPR_TILE_LEVER_RIGHT);
			//else
				//Graphics.PlaySprite(GameData.SPR_TILE_LEVER_LEFT);
			Graphics.PlayAnimation(SpriteList[SwitchState ? 1 : 0]);
		}

		
		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			bool switchState = args.Properties.GetBoolean("switch_state", false);
			Tile.DrawTileDataIndex(g, args, switchState ? 1 : 0);
		}


		//-----------------------------------------------------------------------------
		// Scripting API
		//-----------------------------------------------------------------------------

		bool ZeldaAPI.Lever.IsFacingLeft {
			get { return !SwitchState; }
		}
		
		bool ZeldaAPI.Lever.IsFacingRight {
			get { return SwitchState; }
		}
	}
}

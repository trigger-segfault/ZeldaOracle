using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorSwitch : SwitchTileBase, ZeldaAPI.ColorSwitch {

		private PuzzleColor color;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorSwitch() {

		}
		

		//-----------------------------------------------------------------------------
		// Lever Methods
		//-----------------------------------------------------------------------------

		public override void OnToggle(bool switchState) {
			if (switchState) {
				color = PuzzleColor.Blue;
				CustomSprite = GameData.SPR_TILE_COLOR_SWITCH_BLUE;
			}
			else {
				color = PuzzleColor.Red;
				CustomSprite = GameData.SPR_TILE_COLOR_SWITCH_RED;
			}

			AudioSystem.PlaySound(GameData.SOUND_SWITCH);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();
			
			if (SwitchState) {
				color = PuzzleColor.Blue;
				CustomSprite = GameData.SPR_TILE_COLOR_SWITCH_BLUE;
			}
			else {
				color = PuzzleColor.Red;
				CustomSprite = GameData.SPR_TILE_COLOR_SWITCH_RED;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return color; }
		}
	}
}

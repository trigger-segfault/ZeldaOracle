using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control {
	
	public class HUD {

		private GameManager gameManager;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public HUD(GameManager gameManager) {
			this.gameManager = gameManager;
		}

		
		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		public void Draw(Graphics2D g) {
			Sprite spr = GameData.SPR_HUD_BACKGROUND;
			
			Rectangle2I r = new Rectangle2I(0, 0, GameSettings.SCREEN_WIDTH, 16);
			g.DrawSprite(spr, r);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------


	}
}

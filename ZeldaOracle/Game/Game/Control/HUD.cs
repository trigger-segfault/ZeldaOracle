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

		private bool advancedGame;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public HUD(GameManager gameManager) {
			this.gameManager = gameManager;
			this.advancedGame = false;
		}

		
		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		public void Draw(Graphics2D g) {
			Sprite spr = GameData.SPR_HUD_BACKGROUND;
			
			Rectangle2I r = new Rectangle2I(0, 0, GameSettings.SCREEN_WIDTH, 16);
			g.DrawSprite(spr, r);
			DrawItems(g);
			DrawRupees(g);
			DrawHearts(g);
		}
		private void DrawItems(Graphics2D g) {
			bool twoHandedEquipped = false;
			Sprite aR = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(7, 0));
			Sprite aL = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(8, 0));
			Sprite bR = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(7, 1));
			Sprite bL = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(8, 1));
			Sprite eTR = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(9, 0));
			Sprite eTL = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(10, 0));
			Sprite eBR = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(9, 1));
			Sprite eBL = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(10, 1));
			Sprite eTM = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(11, 0));
			Sprite eBM = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(11, 1));

			if (twoHandedEquipped) {
				// B bracket side
				g.DrawSprite(bR, new Point2I(8, 0));
				g.DrawSprite(eBR, new Point2I(8, 8));
				// A bracket side
				g.DrawSprite(aL, new Point2I(56, 0));
				g.DrawSprite(eBL, new Point2I(56, 8));

				// item1.Draw(g, new Point2I(16, 0));
			}
			else if (!advancedGame) {
				// B bracket
				g.DrawSprite(bR, new Point2I(0, 0));
				g.DrawSprite(eBR, new Point2I(0, 8));
				g.DrawSprite(eTL, new Point2I(32, 0));
				g.DrawSprite(eBL, new Point2I(32, 8));
				// A bracket
				g.DrawSprite(aR, new Point2I(40, 0));
				g.DrawSprite(eBR, new Point2I(40, 8));
				g.DrawSprite(eTL, new Point2I(72, 0));
				g.DrawSprite(eBL, new Point2I(72, 8));

				// item1.Draw(g, new Point2I(8, 0));
				// item1.Draw(g, new Point2I(48, 0));
			}
			else {
				// B bracket side
				g.DrawSprite(bR, new Point2I(0, 0));
				g.DrawSprite(eBR, new Point2I(0, 8));
				// Both bracket side
				g.DrawSprite(eTM, new Point2I(32, 0));
				g.DrawSprite(eBM, new Point2I(32, 8));
				// A bracket side
				g.DrawSprite(aL, new Point2I(64, 0));
				g.DrawSprite(eBL, new Point2I(64, 8));

				// item1.Draw(g, new Point2I(8, 0));
				// item1.Draw(g, new Point2I(40, 0));
			}
		}

		private void DrawRupees(Graphics2D g) {
			bool inDungeon = false;
			int advancedOffset = (advancedGame ? 8 : 0);
			int numKeys = 0;
			int numRupees = 0;
			Sprite rupee = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(0, 2));
			Sprite ore = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(1, 2));
			Sprite key = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(0, 1));
			Sprite keyX = new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(1, 1));
			if (inDungeon) {
				g.DrawSprite(key, new Point2I(80 - advancedOffset, 0));
				g.DrawSprite(keyX, new Point2I(88 - advancedOffset, 0));
				g.DrawString(GameData.FONT_SMALL, numKeys.ToString(), new Point2I(96 - advancedOffset, 0), Color.Black);
			}
			else {
				g.DrawSprite(rupee, new Point2I(80 - advancedOffset, 0));
			}
			g.DrawString(GameData.FONT_SMALL, numRupees.ToString("000"), new Point2I(80 - advancedOffset, 8), Color.Black);
		}
		private void DrawHearts(Graphics2D g) {
			Sprite[] hearts = new Sprite[]{
				new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(0, 0)),
				new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(1, 0)),
				new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(2, 0)),
				new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(3, 0)),
				new Sprite(GameData.SHEET_MENU_SMALL, new Point2I(4, 0))
			};
			for (int i = 0; i < gameManager.RoomControl.Player.MaxHealth / 4; i++) {
				int fullness = GMath.Clamp(gameManager.RoomControl.Player.Health, 0, 4);
				if (!advancedGame)
					g.DrawSprite(hearts[fullness], new Point2I(104 + (i % 7) * 8, (i / 7) * 8));
				else
					g.DrawSprite(hearts[fullness], new Point2I(96 + (i % 8) * 8, (i / 8) * 8));

			}
		}



		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------


	}
}

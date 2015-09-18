using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control {
	
	public class HUD {

		private GameControl gameControl;

		private int dynamicRupees;
		private SoundInstance rupeeSound;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public HUD(GameControl gameControl) {
			this.gameControl	= gameControl;
			this.dynamicRupees	= 0;
			this.rupeeSound		= null;
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		public void Update() {
			int rupees = Inventory.GetAmmo("rupees").Amount;
			if (dynamicRupees < rupees) {
				dynamicRupees++;
				if (dynamicRupees < rupees) {
					if (rupeeSound == null || !rupeeSound.IsPlaying) {
						rupeeSound = Resources.RootSoundGroup.GetSound("UI/get_rupee_loop").Play(true);
					}
				}
				else {
					if (rupeeSound != null && rupeeSound.IsPlaying) {
						rupeeSound.Stop();
					}
					rupeeSound = Resources.RootSoundGroup.GetSound("UI/get_rupee").Play();
				}
			}
			else if (dynamicRupees > rupees) {
				dynamicRupees--;
				if (dynamicRupees > rupees) {
					if (rupeeSound == null || !rupeeSound.IsPlaying) {
						rupeeSound = Resources.RootSoundGroup.GetSound("UI/get_rupee_loop").Play(true);
					}
				}
				else {
					if (rupeeSound != null && rupeeSound.IsPlaying) {
						rupeeSound.Stop();
					}
					rupeeSound = Resources.RootSoundGroup.GetSound("UI/get_rupee").Play();
				}
			}
		}

		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		// Draws the HUD.
		public void Draw(Graphics2D g, bool light) {
			SpriteSheet sheetMenuSmall = (light ? GameData.SHEET_MENU_SMALL_LIGHT : GameData.SHEET_MENU_SMALL);
			Sprite background = new Sprite(sheetMenuSmall, new Point2I(2, 4));
			
			Rectangle2I r = new Rectangle2I(0, 0, GameSettings.SCREEN_WIDTH, 16);
			g.DrawSprite(background, r);
			DrawItems(g, light);
			DrawRupees(g, light);
			DrawHearts(g, light);
		}

		// Draws the equipped usable items.
		private void DrawItems(Graphics2D g, bool light) {
			SpriteSheet sheetMenuSmall = (light ? GameData.SHEET_MENU_SMALL_LIGHT : GameData.SHEET_MENU_SMALL);
			Sprite aR = new Sprite(sheetMenuSmall, new Point2I(7, 0));
			Sprite aL = new Sprite(sheetMenuSmall, new Point2I(8, 0));
			Sprite bR = new Sprite(sheetMenuSmall, new Point2I(7, 1));
			Sprite bL = new Sprite(sheetMenuSmall, new Point2I(8, 1));
			Sprite eTR = new Sprite(sheetMenuSmall, new Point2I(9, 0));
			Sprite eTL = new Sprite(sheetMenuSmall, new Point2I(10, 0));
			Sprite eBR = new Sprite(sheetMenuSmall, new Point2I(9, 1));
			Sprite eBL = new Sprite(sheetMenuSmall, new Point2I(10, 1));
			Sprite eTM = new Sprite(sheetMenuSmall, new Point2I(11, 0));
			Sprite eBM = new Sprite(sheetMenuSmall, new Point2I(11, 1));

			if (Inventory.IsTwoHandedEquipped) {
				// B bracket side
				g.DrawSprite(bR, new Point2I(8, 0));
				g.DrawSprite(eBR, new Point2I(8, 8));
				// A bracket side
				g.DrawSprite(aL, new Point2I(56, 0));
				g.DrawSprite(eBL, new Point2I(56, 8));

				Inventory.EquippedUsableItems[0].DrawInInventory(g, new Point2I(16, 0), light);
			}
			else if (!gameControl.IsAdvancedGame) {
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

				if (Inventory.EquippedUsableItems[1] != null)
					Inventory.EquippedUsableItems[1].DrawInInventory(g, new Point2I(8, 0), light);
				if (Inventory.EquippedUsableItems[0] != null)
					Inventory.EquippedUsableItems[0].DrawInInventory(g, new Point2I(48, 0), light);
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

				if (Inventory.EquippedUsableItems[1] != null)
					Inventory.EquippedUsableItems[1].DrawInInventory(g, new Point2I(8, 0), light);
				if (Inventory.EquippedUsableItems[0] != null)
					Inventory.EquippedUsableItems[0].DrawInInventory(g, new Point2I(40, 0), light);
			}
		}

		// Draws the ruppes and dungeon keys.
		private void DrawRupees(Graphics2D g, bool light) {
			SpriteSheet sheetMenuSmall = (light ? GameData.SHEET_MENU_SMALL_LIGHT : GameData.SHEET_MENU_SMALL);
			Color black = (light ? new Color(16, 16, 16) : Color.Black);
			bool inDungeon = false;
			int advancedOffset = (gameControl.IsAdvancedGame ? 8 : 0);
			int numKeys = 0;
			Sprite rupee = new Sprite(sheetMenuSmall, new Point2I(0, 2));
			Sprite ore = new Sprite(sheetMenuSmall, new Point2I(1, 2));
			Sprite key = new Sprite(sheetMenuSmall, new Point2I(0, 1));
			Sprite keyX = new Sprite(sheetMenuSmall, new Point2I(1, 1));
			if (inDungeon) {
				g.DrawSprite(key, new Point2I(80 - advancedOffset, 0));
				g.DrawSprite(keyX, new Point2I(88 - advancedOffset, 0));
				g.DrawString(GameData.FONT_SMALL, numKeys.ToString(), new Point2I(96 - advancedOffset, 0), black);
			}
			else {
				g.DrawSprite(rupee, new Point2I(80 - advancedOffset, 0));
			}
			g.DrawString(GameData.FONT_SMALL, dynamicRupees.ToString("000"), new Point2I(80 - advancedOffset, 8), black);
		}

		// Draws the player's life.
		private void DrawHearts(Graphics2D g, bool light) {
			SpriteSheet sheetMenuSmall = (light ? GameData.SHEET_MENU_SMALL_LIGHT : GameData.SHEET_MENU_SMALL);
			Sprite[] hearts = new Sprite[]{
				new Sprite(sheetMenuSmall, new Point2I(0, 0)),
				new Sprite(sheetMenuSmall, new Point2I(1, 0)),
				new Sprite(sheetMenuSmall, new Point2I(2, 0)),
				new Sprite(sheetMenuSmall, new Point2I(3, 0)),
				new Sprite(sheetMenuSmall, new Point2I(4, 0))
			};
			for (int i = 0; i < gameControl.Player.MaxHealth / 4; i++) {
				int fullness = GMath.Clamp(gameControl.Player.Health, 0, 4);
				if (!gameControl.IsAdvancedGame)
					g.DrawSprite(hearts[fullness], new Point2I(104 + (i % 7) * 8, (i / 7) * 8));
				else
					g.DrawSprite(hearts[fullness], new Point2I(96 + (i % 8) * 8, (i / 8) * 8));

			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Inventory Inventory {
			get { return gameControl.Inventory; }
		}

		public int DynamicRupees {
			get { return dynamicRupees; }
			set { dynamicRupees = value; }
		}
	}
}

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
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {
	
	public class HUD {

		// The game control containing this HUD.
		private GameControl gameControl;
		// Used to slowly increment rupee count.
		private int dynamicRupees;
		// Used to slowly increment player health.
		private int dynamicHealth;
		// Used to update the health positively at a slower pace.
		private int healthTimer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public HUD(GameControl gameControl) {
			this.gameControl	= gameControl;
			this.dynamicRupees	= 0;
			this.dynamicHealth	= 3 * 4;
			this.healthTimer	= 0;
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		// Updates the rupees and player life incrementation.
		public void Update() {
			int rupees = Inventory.GetAmmo("rupees").Amount;
			int health = gameControl.Player.Health;

			// Update dynamic rupees.
			if (dynamicRupees != rupees) {
				dynamicRupees += Math.Sign(rupees - dynamicRupees);
				AudioSystem.LoopSoundWhileActive(GameData.SOUND_GET_RUPEE_LOOP);
			}

			// Update dynamic health.
			if (dynamicHealth < health) {
				if (healthTimer < 3) {
					healthTimer++;
				}
				else {
					healthTimer = 0;
					dynamicHealth++;
					if (dynamicHealth % 4 == 0) {
						AudioSystem.PlaySound(GameData.SOUND_GET_HEART);
					}
				}
			}
			else if (dynamicHealth > health) {
				dynamicHealth--;
			}
			else {
				healthTimer = 0;
			}
		}

		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		// Draws the HUD.
		public void Draw(Graphics2D g, bool light) {
			int lightDark = (light ? GameData.VARIANT_LIGHT : GameData.VARIANT_DARK);

			Rectangle2I r = new Rectangle2I(0, 0, GameSettings.SCREEN_WIDTH, 16);
			g.DrawSprite(GameData.SPR_HUD_BACKGROUND, lightDark, r);
			
			DrawItems(g, lightDark);
			DrawRupees(g, lightDark);
			DrawHearts(g, lightDark);
		}

		// Draws the equipped usable items.
		private void DrawItems(Graphics2D g, int lightDark) {
			if (Inventory.IsTwoHandedEquipped) {
				// B bracket side
				g.DrawSprite(GameData.SPR_HUD_BRACKET_LEFT_B, lightDark, new Point2I(8, 0));
				// A bracket side
				g.DrawSprite(GameData.SPR_HUD_BRACKET_RIGHT_A, lightDark, new Point2I(56, 0));

				Inventory.EquippedWeapons[0].DrawSlot(g, new Point2I(16, 0), lightDark);
			}
			else if (!gameControl.IsAdvancedGame) {
				// B bracket
				g.DrawSprite(GameData.SPR_HUD_BRACKET_LEFT_B, lightDark, new Point2I(0, 0));
				g.DrawSprite(GameData.SPR_HUD_BRACKET_RIGHT, lightDark, new Point2I(32, 0));
				// A bracket
				g.DrawSprite(GameData.SPR_HUD_BRACKET_LEFT_A, lightDark, new Point2I(40, 0));
				g.DrawSprite(GameData.SPR_HUD_BRACKET_RIGHT, lightDark, new Point2I(72, 0));

				if (Inventory.EquippedWeapons[1] != null)
					Inventory.EquippedWeapons[1].DrawSlot(g, new Point2I(8, 0), lightDark);
				if (Inventory.EquippedWeapons[0] != null)
					Inventory.EquippedWeapons[0].DrawSlot(g, new Point2I(48, 0), lightDark);
			}
			else {
				// B bracket side
				g.DrawSprite(GameData.SPR_HUD_BRACKET_LEFT_B, lightDark, new Point2I(0, 0));
				// Both bracket side
				g.DrawSprite(GameData.SPR_HUD_BRACKET_LEFT_RIGHT, lightDark, new Point2I(32, 0));
				// A bracket side
				g.DrawSprite(GameData.SPR_HUD_BRACKET_RIGHT_A, lightDark, new Point2I(64, 0));

				if (Inventory.EquippedWeapons[1] != null)
					Inventory.EquippedWeapons[1].DrawSlot(g, new Point2I(8, 0), lightDark);
				if (Inventory.EquippedWeapons[0] != null)
					Inventory.EquippedWeapons[0].DrawSlot(g, new Point2I(40, 0), lightDark);
			}
		}

		// Draws the ruppes and dungeon keys.
		private void DrawRupees(Graphics2D g, int lightDark) {
			Color black = (lightDark == GameData.VARIANT_LIGHT ? new Color(16, 16, 16) : Color.Black);
			int advancedOffset = (gameControl.IsAdvancedGame ? 8 : 0);
			Dungeon dungeon = gameControl.RoomControl.Dungeon;

			if (dungeon != null) {
				// Display the small key count.
				g.DrawSprite(GameData.SPR_HUD_KEY, lightDark, new Point2I(80 - advancedOffset, 0));
				g.DrawSprite(GameData.SPR_HUD_X, lightDark, new Point2I(88 - advancedOffset, 0));
				g.DrawString(GameData.FONT_SMALL, dungeon.NumSmallKeys.ToString(), new Point2I(96 - advancedOffset, 0), black);
			}
			else {
				// Display rupee icon.
				g.DrawSprite(GameData.SPR_HUD_RUPEE, lightDark, new Point2I(80 - advancedOffset, 0));
			}

			g.DrawString(GameData.FONT_SMALL, dynamicRupees.ToString("000"), new Point2I(80 - advancedOffset, 8), black);
		}

		// Draws the player's life.
		private void DrawHearts(Graphics2D g, int lightDark) {
			for (int i = 0; i < gameControl.Player.MaxHealth / 4; i++) {
				int fullness = GMath.Clamp(dynamicHealth - i * 4, 0, 4);
				if (!gameControl.IsAdvancedGame)
					g.DrawSprite(GameData.SPR_HUD_HEARTS[fullness], lightDark, new Point2I(104 + (i % 7) * 8, (i / 7) * 8));
				else
					g.DrawSprite(GameData.SPR_HUD_HEARTS[fullness], lightDark, new Point2I(96 + (i % 8) * 8, (i / 8) * 8));
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the player's inventory.
		public Inventory Inventory {
			get { return gameControl.Inventory; }
		}

		// Gets or sets the dynamic rupee count for the HUD.
		public int DynamicRupees {
			get { return dynamicRupees; }
			set { dynamicRupees = value; }
		}

		// Gets or sets the dynamic health count for the HUD.
		public int DynamicHealth {
			get { return dynamicHealth; }
			set { dynamicHealth = value; }
		}
	}
}

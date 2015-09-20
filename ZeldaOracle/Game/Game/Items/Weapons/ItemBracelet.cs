using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemBracelet : ItemWeapon {

		private PlayerGrabState grabState;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBracelet(int level = 0) : base() {
			this.id				= "item_bracelet";
			this.name			= new string[] { "Power Bracelet", "Power Gloves" };
			this.description	= new string[] { "A strength booster.", "Used to lift large objects." };
			this.level			= level;
			this.maxLevel		= 1;
			this.sprite			= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, 0, 1),
				new Sprite(GameData.SHEET_ITEMS_SMALL, 1, 1)
			};
			this.spriteLight	= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, 0, 1),
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, 1, 1)
			};
			this.grabState		= new PlayerGrabState();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Grab, pull, and pickup objects.
		public override void OnButtonPress() {
			/*
			float minDist = 0;
			Tile grabTile = null;

			// If multiple tiles, pick the closer one.
			for (int i = 0; i < player.FrontTiles.Length; i++) {
				Tile tile = player.FrontTiles[i];

				if (tile != null) {
					Vector2F disp = tile.Center - Player.Center;

					int axis = (player.Direction + 1) % 2;
					float dist = Math.Abs(disp[axis]);

					if (grabTile == null || dist < minDist) {
						grabTile = tile;
						minDist = dist;
					}
				}
			}

			if (grabTile != null && !(player.CurrentState is PlayerGrabState)) {
				player.BeginState(new PlayerGrabState());
			}
			*/
		}
		
		// Grab, pull, and pickup objects.
		public override void OnButtonDown() {
			// Check for a tile to grab.
			Tile grabTile = player.Physics.GetMeetingSolidTile(player.Position, player.Direction);
			if (grabTile != null && player.CurrentState != grabState) {
				grabState.BraceletEquipSlot = CurrentEquipSlot;
				player.BeginState(grabState);
			}
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, bool light) {
			DrawSprite(g, position, light);
			DrawSprite(g, position, light);
			DrawLevel(g, position, light);
		}
	}
}

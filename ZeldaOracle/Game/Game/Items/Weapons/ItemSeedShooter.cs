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
using ZeldaOracle.Game.Items.Ammos;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemSeedShooter : ItemWeapon {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSeedShooter()
			: base() {
			this.id				= "item_seed_shooter";
			this.name			= new string[] { "Seed Shooter" };
			this.description	= new string[] { "Used to bounce seeds around." };
			this.currentAmmo	= 0;
			this.sprite			= new Sprite[] { new Sprite(GameData.SHEET_ITEMS_SMALL, 8, 0) };
			this.spriteLight	= new Sprite[] { new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, 8, 0) };
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			
		}

		// Called when the item is added to the inventory list.
		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);
			int[] maxAmounts = { 20, 30, 50 };

			this.currentAmmo = 0;

			this.ammo = new Ammo[] {
				inventory.AddAmmo(
					new AmmoSatchelSeeds(
						"ammo_ember_seeds", "Ember Seeds", "A burst of fire!",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(0, 3)),
						new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(0, 3)),
						0, maxAmounts[level]
					),
					false
				),
				inventory.AddAmmo(
					new AmmoSatchelSeeds(
						"ammo_scent_seeds", "Scent Seeds", "An aromatic blast!",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(1, 3)),
						new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(1, 3)),
						0, maxAmounts[level]
					),
					false
				),
				inventory.AddAmmo(
					new AmmoSatchelSeeds(
						"ammo_pegasus_seeds", "Pegasus Seeds", "Steals speed?",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(2, 3)),
						new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(2, 3)),
						0, maxAmounts[level]
					),
					false
				),
				inventory.AddAmmo(
					new AmmoSatchelSeeds(
						"ammo_gale_seeds", "Gale Seeds", "A mighty blow!",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(3, 3)),
						new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(3, 3)),
						0, maxAmounts[level]
					),
					false
				),
				inventory.AddAmmo(
					new AmmoSatchelSeeds(
						"ammo_mystery_seeds", "Mystery Seeds", "A producer of unknown effects.",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(4, 3)),
						new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(4, 3)),
						0, maxAmounts[level]
					),
					false
				)
			};
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, bool light) {
			DrawSprite(g, position, light);
			DrawAmmo(g, position, light);
			g.DrawSprite(light ? ammo[currentAmmo].SpriteLight : ammo[currentAmmo].Sprite, position + new Point2I(8, 0));
		}

		// Called when the item's level is changed.
		public override void OnLevelUp() {

		}


		// Called when the item has been obtained.
		public override void OnObtained() {
			inventory.ObtainAmmo(this.ammo[0]);
		}

		// Called when the item has been unobtained.
		public override void OnUnobtained() {

		}

		// Called when the item has been stolen.
		public override void OnStolen() {

		}

		// Called when the stolen item has been returned.
		public override void OnReturned() {

		}
	}
}

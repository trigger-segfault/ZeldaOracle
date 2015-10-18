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
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Items.Ammos;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemSeedSatchel : ItemWeapon {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSeedSatchel() {
			this.id				= "item_seed_satchel";
			this.name			= new string[] { "Seed Satchel", "Seed Satchel", "Seed Satchel" };
			this.description	= new string[] { "A bag for carrying seeds.", "A bag for carrying seeds.", "A bag for carrying seeds." };
			this.maxLevel		= Item.Level3;
			this.currentAmmo	= 0;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWithSword | ItemFlags.UsableWhileInHole;

			sprite = new Sprite[] {
				GameData.SPR_ITEM_ICON_SATCHEL,
				GameData.SPR_ITEM_ICON_SATCHEL,
				GameData.SPR_ITEM_ICON_SATCHEL,
			};
			spriteEquipped = new Sprite[] {
				GameData.SPR_ITEM_ICON_SATCHEL_EQUIPPED,
				GameData.SPR_ITEM_ICON_SATCHEL_EQUIPPED,
				GameData.SPR_ITEM_ICON_SATCHEL_EQUIPPED,
			};
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private Seed DropSeed(SeedType type) {
			Seed seed = ThrowSeed(type);
			seed.Position = Player.Center;
			seed.Physics.Velocity = Vector2F.Zero;
			seed.Physics.ZVelocity = 0.0f;
			return seed;
		}
		
		private Seed ThrowSeed(SeedType type) {
			Seed seed = new Seed(type);

			Vector2F velocity = Directions.ToVector(Player.Direction);
			Vector2F pos = Player.Origin + (velocity * 4.0f);
			Player.RoomControl.SpawnEntity(seed, pos, Player.ZPosition + 6);
			seed.Physics.Velocity = velocity * 0.75f;
			
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
			Player.BeginBusyState(10);

			return seed;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			string ammoID = ammo[currentAmmo].ID;

			if (ammoID == "ammo_ember_seeds") {
				ThrowSeed(SeedType.Ember);
			}
			else if (ammoID == "ammo_scent_seeds") {
				ThrowSeed(SeedType.Scent);
			}
			else if (ammoID == "ammo_gale_seeds") {
				DropSeed(SeedType.Gale);
			}
			else if (ammoID == "ammo_mystery_seeds") {
				ThrowSeed(SeedType.Mystery);
			}
			else if (ammoID == "ammo_pegasus_seeds") {
				// Start sprinting.
				if (!Player.Movement.IsSprinting) {
					Player.RoomControl.SpawnEntity(new Effect(
						GameData.ANIM_EFFECT_PEGASUS_DUST), Player.Center - new Point2I(0, 8));
					Player.Movement.StartSprinting(
						GameSettings.PLAYER_SPRINT_DURATION,
						GameSettings.PLAYER_SPRINT_SPEED_SCALE);
				}
			}
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
						0, maxAmounts[level]
					),
					false
				),
				inventory.AddAmmo(
					new AmmoSatchelSeeds(
						"ammo_scent_seeds", "Scent Seeds", "An aromatic blast!",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(1, 3)),
						0, maxAmounts[level]
					),
					false
				),
				inventory.AddAmmo(
					new AmmoSatchelSeeds(
						"ammo_pegasus_seeds", "Pegasus Seeds", "Steals speed?",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(2, 3)),
						0, maxAmounts[level]
					),
					false
				),
				inventory.AddAmmo(
					new AmmoSatchelSeeds(
						"ammo_gale_seeds", "Gale Seeds", "A mighty blow!",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(3, 3)),
						0, maxAmounts[level]
					),
					false
				),
				inventory.AddAmmo(
					new AmmoSatchelSeeds(
						"ammo_mystery_seeds", "Mystery Seeds", "A producer of unknown effects.",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(4, 3)),
						0, maxAmounts[level]
					),
					false
				)
			};
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, int lightOrDark) {
			DrawSprite(g, position, lightOrDark);
			DrawAmmo(g, position, lightOrDark);
			g.DrawSprite(ammo[currentAmmo].Sprite, lightOrDark, position + new Point2I(8, 0));
		}

		// Called when the item's level is changed.
		public override void OnLevelUp() {
			int[] maxAmounts = { 20, 30, 50 };
			for (int i = 0; i < ammo.Length; i++) {
				ammo[i].MaxAmount = maxAmounts[level];
				if (ammo[i].IsObtained)
					ammo[i].Amount = maxAmounts[level];
			}
		}


		// Called when the item has been obtained.
		public override void OnObtained() {
			inventory.ObtainAmmo(this.ammo[0]);
			this.ammo[0].Amount = 20;
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

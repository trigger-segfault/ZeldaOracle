using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players.States;

namespace ZeldaOracle.Game.Items {
	public class ItemBombs : ItemWeapon {
		
		private Bomb bombEntity;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBombs() {
			this.id				= "item_bombs";
			this.name			= new string[] { "Bombs", "Bombs", "Bombs" };
			this.description	= new string[] { "Very explosive.", "Very explosive.", "Very explosive." };
			this.maxLevel		= Item.Level3;
			this.currentAmmo	= 0;
			
			this.flags			= ItemFlags.None;

			sprite = new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(13, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(13, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(13, 0))
			};
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		public override void OnButtonPress() {
			if (bombEntity == null || bombEntity.IsDestroyed) {
				// Conjure a new bomb.
				bombEntity = new Bomb();
				Player.BeginState(new PlayerCarryState(bombEntity));
			}
			else {
				// Pickup a bomb from the ground.
				if (Player.Physics.IsSoftMeetingEntity(bombEntity)) {
					Player.BeginState(new PlayerCarryState(bombEntity));
					bombEntity.RemoveFromRoom();
				}
			}
		}

		// Called when the item is added to the inventory list
		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);
			int[] maxAmounts = { 10, 20, 30 };

			this.currentAmmo = 0;
			this.ammo = new Ammo[] {
				inventory.AddAmmo(
					new Ammo(
						"ammo_bombs", "Bombs", "Very explosive.",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(13, 0)),
						maxAmounts[level], maxAmounts[level]
					),
					false
				)
			};
		}

		// Called when the item's level is changed.
		public override void OnLevelUp() {
			int[] maxAmounts = { 10, 20, 30 };
			inventory.GetAmmo("ammo_bombs").MaxAmount = maxAmounts[level];
			inventory.GetAmmo("ammo_bombs").Amount = maxAmounts[level];
		}

		// Called when the item has been obtained.
		public override void OnObtained() {
			inventory.ObtainAmmo(inventory.GetAmmo("ammo_bombs"));
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, int lightOrDark) {
			DrawSprite(g, position, lightOrDark);
			DrawAmmo(g, position, lightOrDark);
		}
	}
}

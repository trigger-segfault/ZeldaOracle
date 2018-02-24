using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items {
	public class ItemBombs : ItemWeapon {
		
		private EntityTracker<Bomb> bombTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBombs() {
			this.id				= "item_bombs";
			this.name			= new string[] { "Bombs", "Bombs", "Bombs" };
			this.description	= new string[] { "Very explosive.", "Very explosive.", "Very explosive." };
			this.maxLevel		= Item.Level3;
			this.currentAmmo	= 0;
			this.bombTracker	= new EntityTracker<Bomb>(1);

			this.flags			= ItemFlags.UsableWhileInHole;

			sprite = new ISprite[] {
				GameData.SPR_ITEM_ICON_BOMB,
				GameData.SPR_ITEM_ICON_BOMB,
				GameData.SPR_ITEM_ICON_BOMB,
			};
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		public override bool OnButtonPress() {
			if (bombTracker.IsEmpty) {
				if (HasAmmo()) {
					// Conjure a new bomb
					UseAmmo();
					Bomb bomb = new Bomb();
					bombTracker.TrackEntity(bomb);
					Player.CarryState.SetCarryObject(bomb);
					Player.BeginWeaponState(Player.CarryState);
					return true;
				}
			}
			else {
				// Pickup a bomb from the ground
				Bomb bomb = bombTracker.GetEntity();
				if (bomb != null && bomb.IsPickupable && Player.Physics.IsBraceletMeetingEntity(
					bomb, GameSettings.PLAYER_BRACELET_BOXES[Player.Direction]))
				{
					Player.CarryState.SetCarryObject(bomb);
					Player.BeginWeaponState(Player.CarryState);
					bomb.RemoveFromRoom();
					return true;
				}
			}
			return false;
		}

		// Called when the item is added to the inventory list
		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);

			this.currentAmmo = 0;
			this.ammo = new Ammo[] {
				inventory.GetAmmo("ammo_bombs")
			};
		}

		// Called when the item's level is changed.
		public override void OnLevelUp() {
			int[] maxAmounts	= { 10, 20, 30 };
			ammo[0].MaxAmount	= maxAmounts[level];
			ammo[0].Amount		= maxAmounts[level];
		}

		// Called when the item has been obtained.
		public override void OnObtained() {
			inventory.ObtainAmmo(ammo[0]);
			ammo[0].Amount = ammo[0].MaxAmount;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position) {
			DrawSprite(g, position);
			DrawAmmo(g, position);
		}
	}
}

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
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Players.States;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemBow : ItemWeapon {
		
		private EntityTracker<Arrow> arrowTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBow() {
			this.id				= "item_bow";
			this.name			= new string[] { "Wooden Bow", "Wooden Bow", "Wooden Bow" };
			this.description	= new string[] { "Weapon of a marksman.", "Weapon of a marksman.", "Weapon of a marksman." };
			this.maxLevel		= Item.Level3;
			this.currentAmmo	= 0;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWithSword | ItemFlags.UsableWhileInHole;
			this.arrowTracker	= new EntityTracker<Arrow>(2);

			sprite = new Sprite[] {
				GameData.SPR_ITEM_ICON_BOW,
				GameData.SPR_ITEM_ICON_BOW,
				GameData.SPR_ITEM_ICON_BOW
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			if (arrowTracker.IsMaxedOut || !HasAmmo())
				return;
			
			UseAmmo();
			Player.Direction = Player.UseDirection;
			
			// Shoot and track the arrow projectile.
			Arrow arrow = new Arrow();
			Player.ShootFromDirection(arrow, Player.Direction,
				GameSettings.PROJECTILE_ARROW_SPEED,
				Directions.ToVector(Player.Direction) * 8.0f);
			arrowTracker.TrackEntity(arrow);

			// Begin the busy state.
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
			Player.BeginBusyState(10);
		}

		// Called when the item is added to the inventory list.
		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);

			this.currentAmmo = 0;
			this.ammo = new Ammo[] {
				inventory.GetAmmo("ammo_arrows")
			};
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, int lightOrDark) {
			DrawSprite(g, position, lightOrDark);
			DrawAmmo(g, position, lightOrDark);
		}

		// Called when the item's level is changed.
		public override void OnLevelUp() {
			int[] maxAmounts	= { 30, 40, 50 };
			ammo[0].MaxAmount	= maxAmounts[level];
			ammo[0].Amount		= maxAmounts[level];
		}


		// Called when the item has been obtained.
		public override void OnObtained() {
			inventory.ObtainAmmo(ammo[0]);
			ammo[0].Amount = ammo[0].MaxAmount;
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

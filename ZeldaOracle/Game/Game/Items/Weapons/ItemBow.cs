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
			if (ammo[currentAmmo].IsEmpty || arrowTracker.IsMaxedOut)
				return;
			
			Player.Direction = Player.UseDirection;

			// Spawn the arrow.
			// TODO: keep track of arrows (only can shoot 2 at a time).
			Arrow arrow = new Arrow();
			arrow.Owner				= Player;
			arrow.Position			= Player.Center + (Directions.ToVector(Player.Direction) * 8.0f);
			arrow.ZPosition			= Player.ZPosition;
			arrow.Direction			= Player.Direction;
			arrow.Physics.Velocity	= Directions.ToVector(Player.Direction) * GameSettings.PROJECTILE_ARROW_SPEED;
			RoomControl.SpawnEntity(arrow);

			ammo[currentAmmo].Amount--;
			arrowTracker.TrackEntity(arrow);

			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
			Player.BeginBusyState(10);
		}

		// Called when the item is added to the inventory list.
		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);
			int[] maxAmounts = { 30, 40, 50 };

			this.currentAmmo = 0;
			this.ammo = new Ammo[] {
				inventory.AddAmmo(
					new Ammo(
						"ammo_arrows", "Arrows", "A standard arrow.",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(15, 1)),
						maxAmounts[level], maxAmounts[level]
					),
					false
				)
			};
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, int lightOrDark) {
			DrawSprite(g, position, lightOrDark);
			DrawAmmo(g, position, lightOrDark);
		}

		// Called when the item's level is changed.
		public override void OnLevelUp() {
			int[] maxAmounts = { 30, 40, 50 };
			inventory.GetAmmo("ammo_arrows").MaxAmount = maxAmounts[level];
			inventory.GetAmmo("ammo_arrows").Amount = maxAmounts[level];
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

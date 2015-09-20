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

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemBow : ItemWeapon {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBow() : base() {
			this.id				= "item_bow";
			this.name			= new string[] { "Wooden Bow", "Wooden Bow", "Wooden Bow" };
			this.description	= new string[] { "Weapon of a marksman.", "Weapon of a marksman.", "Weapon of a marksman." };
			this.maxLevel		= Item.Level3;
			this.currentAmmo	= 0;
			this.sprite			= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, 13, 1),
				new Sprite(GameData.SHEET_ITEMS_SMALL, 13, 1),
				new Sprite(GameData.SHEET_ITEMS_SMALL, 13, 1)
			};
			this.spriteLight	= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, 13, 1),
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, 13, 1),
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, 13, 1)
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			// Shoot an arrow!
			if (ammo[currentAmmo].IsEmpty)
				return;
			
			Projectile projectile = new Projectile();
				
			// General
			projectile.Owner			= Player;
			projectile.Position			= new Vector2F(Player.X, Player.Y - 8) + (Directions.ToVector(Player.MoveDirection) * 8.0f);
			projectile.ZPosition		= player.ZPosition;
			projectile.Angle			= Directions.ToAngle(player.MoveDirection);
			projectile.Physics.Velocity	= Directions.ToVector(Player.MoveDirection) * 3.0f;

			player.Direction = player.MoveDirection;

			// Graphics.
			projectile.Graphics.SubStripIndex = projectile.Angle;
			projectile.Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW);

			// Physics.
			projectile.Physics.CollisionBox	= new Rectangle2F(-2, -2, 4, 4);
			projectile.EnablePhysics(PhysicsFlags.CollideWorld | PhysicsFlags.LedgePassable |
								PhysicsFlags.HalfSolidPassable | PhysicsFlags.DestroyedOutsideRoom);

			// Crash event.
			Vector2F v = projectile.Physics.Velocity;
			projectile.EventCollision += delegate() {
				// Create crash effect.
				Effect effect = new Effect();
				effect.Position = projectile.Position;
				effect.CreateDestroyTimer(32);
					
				effect.Physics.Velocity		= -(v.Normalized) * 0.25f;
				effect.Physics.ZVelocity	= 1;
				effect.Physics.Gravity		= 0.07f;
				effect.EnablePhysics(PhysicsFlags.HasGravity);
					
				effect.Graphics.IsShadowVisible = false;
				effect.Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW_CRASH);

				RoomControl.SpawnEntity(effect);
				projectile.Destroy();
			};

			RoomControl.SpawnEntity(projectile);
			ammo[currentAmmo].Amount--;

			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
			player.BeginState(new PlayerBusyState(10));
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
						new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(15, 1)),
						maxAmounts[level], maxAmounts[level]
					),
					false
				)
			};
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, bool light) {
			DrawSprite(g, position, light);
			DrawAmmo(g, position, light);
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

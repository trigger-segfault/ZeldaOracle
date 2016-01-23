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
using ZeldaOracle.Game.Entities.Projectiles.Seeds;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemSeedShooter : SeedBasedItem {
		
		private EntityTracker<SeedProjectile> seedTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSeedShooter() {
			this.id				= "item_seed_shooter";
			this.name			= new string[] { "Seed Shooter" };
			this.description	= new string[] { "Used to bounce seeds around." };
			this.currentAmmo	= 0;
			this.sprite			= new Sprite[] { GameData.SPR_ITEM_ICON_SEED_SHOOTER };
			this.spriteEquipped	= new Sprite[] { GameData.SPR_ITEM_ICON_SEED_SHOOTER_EQUIPPED };
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWhileInHole;
			this.seedTracker	= new EntityTracker<SeedProjectile>(1);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			if (HasAmmo()) {
				Player.SeedShooterState.Weapon = this;
				Player.BeginState(Player.SeedShooterState);
			}
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, int lightOrDark) {
			DrawSprite(g, position, lightOrDark);
			DrawAmmo(g, position, lightOrDark);
			g.DrawSprite(ammo[currentAmmo].Sprite, lightOrDark, position + new Point2I(8, 0));
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EntityTracker<SeedProjectile> SeedTracker {
			get { return seedTracker; }
		}
	}
}

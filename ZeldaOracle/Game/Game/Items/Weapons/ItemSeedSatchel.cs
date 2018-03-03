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
using ZeldaOracle.Game.Entities.Projectiles.Seeds;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemSeedSatchel : SeedBasedItem {

		private EntityTracker<DroppedSeed> emberSeedTracker;
		private EntityTracker<DroppedSeed> scentSeedTracker;
		private EntityTracker<DroppedSeed> galeSeedTracker;
		private EntityTracker<DroppedSeed> mysterySeedTracker;


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
			this.emberSeedTracker	= new EntityTracker<DroppedSeed>(1);
			this.scentSeedTracker	= new EntityTracker<DroppedSeed>(1);
			this.galeSeedTracker	= new EntityTracker<DroppedSeed>(1);
			this.mysterySeedTracker	= new EntityTracker<DroppedSeed>(1);

			sprite = new ISprite[] {
				GameData.SPR_ITEM_ICON_SATCHEL,
				GameData.SPR_ITEM_ICON_SATCHEL,
				GameData.SPR_ITEM_ICON_SATCHEL,
			};
			spriteEquipped = new ISprite[] {
				GameData.SPR_ITEM_ICON_SATCHEL_EQUIPPED,
				GameData.SPR_ITEM_ICON_SATCHEL_EQUIPPED,
				GameData.SPR_ITEM_ICON_SATCHEL_EQUIPPED,
			};
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private DroppedSeed DropSeed(SeedType type) {
			DroppedSeed seed = ThrowSeed(type);
			seed.Position = Player.Center;
			seed.Physics.Velocity = Vector2F.Zero;
			seed.Physics.ZVelocity = 0.0f;
			return seed;
		}
		
		private DroppedSeed ThrowSeed(SeedType type) {
			DroppedSeed seed = new DroppedSeed(type);

			// Spawn the seed
			Vector2F velocity = Directions.ToVector(Player.Direction);
			Vector2F pos = Player.Position + (velocity * 4.0f);
			seed.Physics.Velocity = velocity * 0.75f;
			Player.RoomControl.SpawnEntity(seed, pos, Player.ZPosition + 6);
			UseAmmo();
			
			// Play the throw animation
			Player.BeginBusyState(10, Player.Animations.Throw);

			return seed;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override bool OnButtonPress() {
			if (!HasAmmo())
				return false;

			SeedType seedType = CurrentSeedType;

			if (seedType == SeedType.Ember) {
				if (emberSeedTracker.IsAvailable)
					emberSeedTracker.TrackEntity(ThrowSeed(SeedType.Ember));
			}
			else if (seedType == SeedType.Scent) {
				if (scentSeedTracker.IsAvailable)
					scentSeedTracker.TrackEntity(ThrowSeed(SeedType.Scent));
			}
			else if (seedType == SeedType.Pegasus) {
				// Start sprinting.
				if (!Player.Movement.IsSprinting) {
					Player.RoomControl.SpawnEntity(
						new Effect(GameData.ANIM_EFFECT_PEGASUS_DUST, DepthLayer.EffectPegasusDust, true),
						Player.Center - new Point2I(0, 8));
					Player.Movement.StartSprinting(
						GameSettings.PLAYER_SPRINT_DURATION,
						GameSettings.PLAYER_SPRINT_SPEED_SCALE);
					UseAmmo();
				}
				else
					return false;
			}
			else if (seedType == SeedType.Gale) {
				if (galeSeedTracker.IsAvailable)
					galeSeedTracker.TrackEntity(DropSeed(SeedType.Gale));
			}
			else if (seedType == SeedType.Mystery) {
				if (mysterySeedTracker.IsAvailable)
					mysterySeedTracker.TrackEntity(ThrowSeed(SeedType.Mystery));
			}

			return true;
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position) {
			DrawSprite(g, position);
			DrawAmmo(g, position);
			g.DrawSprite(ammo[currentAmmo].Sprite, position + new Point2I(8, 0));
		}

		// Called when the item's level is changed.
		public override void OnLevelUp() {
			// Increase the max amount of seeds you can carry.
			int[] maxAmounts = { 20, 30, 50 };
			for (int i = 0; i < ammo.Length; i++) {
				ammo[i].MaxAmount = maxAmounts[level];
				if (ammo[i].IsObtained)
					ammo[i].Amount = maxAmounts[level];
			}
		}

		// Called when the item has been obtained.
		public override void OnObtained() {
			// Obtain the first seed type (Ember seeds).
			inventory.ObtainAmmo(this.ammo[0]);
			this.ammo[0].Amount = 20;
		}
	}
}

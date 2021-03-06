﻿using System;
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
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemSlingshot : SeedBasedItem {

		private EntityTracker<SeedProjectile> seedTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSlingshot() {
			Flags =
				WeaponFlags.UsableInMinecart |
				WeaponFlags.UsableWhileJumping |
				WeaponFlags.UsableWhileInHole;

			seedTracker = new EntityTracker<SeedProjectile>(3);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override bool OnButtonPress() {
			if (!seedTracker.IsEmpty || !HasAmmo())
				return false;

			UseAmmo();

			SeedType seedType = CurrentSeedType;

			Direction direction = Player.UseDirection;
			Player.Direction = direction;

			// Determine the seed spawn position based on player facing direction.
			Vector2F seedPos;
			if (direction == Direction.Up)
				seedPos = direction.ToVector(1);
			else if (direction == Direction.Down)
				seedPos = direction.ToVector(8);
			else
				seedPos = new Vector2F(0, 6) + direction.ToVector(4);

			// Spawn the main seed projectile.
			SeedProjectile seed = new SeedProjectile(seedType, false);
			Player.ShootProjectile(seed,
				direction.ToVector(GameSettings.SLINGSHOT_SEED_SPEED),
				seedPos, 5);
			seedTracker.TrackEntity(seed);

			// Spawn the extra 2 seeds for the Hyper Slingshot.
			if (Level == Item.Level2) {
				for (int i = 0; i < 2; i++) {
					int sideDirection = direction + (i == 0 ? 1 : 3);
					
					// Calculate the velocity based on a degree offset.
					float radians = direction * GMath.QuarterAngle;
					if (i == 0)
						radians += GameSettings.SLINGSHOT_SEED_RADIAN_OFFSET;
					else
						radians -= GameSettings.SLINGSHOT_SEED_RADIAN_OFFSET;
					Vector2F velocity = Vector2F.FromPolar(GameSettings.SLINGSHOT_SEED_SPEED, radians);
					
					// Spawn the seed.
					seed = new SeedProjectile(seedType, false);
					Player.ShootProjectile(seed, velocity, seedPos, 5);
					seedTracker.TrackEntity(seed);
				}
			}

			// Set the tool animation.
			Player.EquipTool(Player.ToolVisual);
			if (Level == Item.Level1)
				Player.ToolVisual.PlayAnimation(GameData.ANIM_SLINGSHOT_1);
			else
				Player.ToolVisual.PlayAnimation(GameData.ANIM_SLINGSHOT_2);
			Player.ToolVisual.AnimationPlayer.SubStripIndex = direction;

			// Begin the player busy state.
			Player.BeginBusyState(10, Player.Animations.Throw)
				.SetEndAction(delegate(PlayerState playerState)
			{
				playerState.Player.UnequipTool(playerState.Player.ToolVisual);
			});

			return true;
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position) {
			DrawSprite(g, position);
			DrawAmmo(g, position);
			g.DrawSprite(CurrentAmmo.Sprite, position + new Point2I(8, 0));
		}
	}
}

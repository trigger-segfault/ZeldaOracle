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

	public class ItemSlingshot : SeedBasedItem {

		private EntityTracker<SeedProjectile> seedTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSlingshot() {
			this.id				= "item_slingshot";
			this.name			= new string[] { "Slingshot", "Hyper Slingshot" };
			this.description	= new string[] { "Used to shoot seeds.", "Shoots in 3 directions." };
			this.maxLevel		= Item.Level2;
			this.currentAmmo	= 0;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWhileInHole;
			this.sprite			= new Sprite[] { GameData.SPR_ITEM_ICON_SLINGSHOT_1, GameData.SPR_ITEM_ICON_SLINGSHOT_2 };
			this.spriteEquipped	= new Sprite[] { GameData.SPR_ITEM_ICON_SLINGSHOT_1, GameData.SPR_ITEM_ICON_SLINGSHOT_2_EQUIPPED };
			this.seedTracker	= new EntityTracker<SeedProjectile>(3);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			if (!seedTracker.IsEmpty || !HasAmmo())
				return;

			UseAmmo();

			SeedType seedType = CurrentSeedType;
			int direction = Player.UseDirection;

			Player.Direction = direction;

			// Determine the seed spawn position based on player facing direction.
			Vector2F seedPos = Player.Center;
			if (direction == Directions.Up)
				seedPos = Player.Center + (Directions.ToVector(direction) * 1);
			else if (direction == Directions.Down)
				seedPos = Player.Center + (Directions.ToVector(direction) * 8);
			else
				seedPos = Player.Center + new Vector2F(0, 6) + (Directions.ToVector(direction) * 4);

			// Spawn the seed projectile.
			SeedProjectile seed = new SeedProjectile(seedType, false);
			seed.Owner = Player;
			seed.Physics.Velocity = Directions.ToVector(direction) * GameSettings.SLINGSHOT_SEED_SPEED;
			Player.RoomControl.SpawnEntity(seed, seedPos, Player.ZPosition + 5);
			seedTracker.TrackEntity(seed);

			// Spawn the extra 2 seeds for the Hyper Slingshot.
			if (level == Item.Level2) {
				for (int i = 0; i < 2; i++) {
					int sideDirection = direction + (i == 0 ? 1 : 3);
					seed = new SeedProjectile(seedType, false);
					seed.Owner = Player;

					// Calculate the velocity based on a degree offset.
					float degrees = direction * GMath.QuarterAngle;
					if (i == 0)
						degrees += GameSettings.SLINGSHOT_SEED_DEGREE_OFFSET;
					else
						degrees -= GameSettings.SLINGSHOT_SEED_DEGREE_OFFSET;
					seed.Physics.Velocity = Vector2F.CreatePolar(GameSettings.SLINGSHOT_SEED_SPEED, degrees);
					seed.Physics.VelocityY = -seed.Physics.VelocityY;

					Player.RoomControl.SpawnEntity(seed, seedPos, Player.ZPosition + 5);
					seedTracker.TrackEntity(seed);
				}
			}

			// Set the tool animation.
			if (level == Item.Level1)
				Player.ToolVisual.PlayAnimation(GameData.ANIM_SLINGSHOT_1);
			else
				Player.ToolVisual.PlayAnimation(GameData.ANIM_SLINGSHOT_2);
			Player.ToolVisual.AnimationPlayer.SubStripIndex = direction;

			// Begin the player busy state.
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
			Player.BusyState.SetEndAction(delegate(PlayerState playerState) {
				playerState.Player.UnequipTool(playerState.Player.ToolVisual);
			});
			Player.BeginBusyState(10);
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, int lightOrDark) {
			DrawSprite(g, position, lightOrDark);
			DrawAmmo(g, position, lightOrDark);
			g.DrawSprite(ammo[currentAmmo].Sprite, lightOrDark, position + new Point2I(8, 0));
		}
	}
}

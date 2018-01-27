using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles {

	public class PlayerBoomerang : Boomerang {

		private ItemBoomerang itemBoomerang;
		private List<Collectible> collectibles;
		private Point2I tileLocation;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerBoomerang(ItemBoomerang itemBoomerang) {
			this.itemBoomerang = itemBoomerang;

			speed		= GameSettings.PROJECTILE_BOOMERANG_SPEEDS[itemBoomerang.Level];
			returnDelay	= GameSettings.PROJECTILE_BOOMERANG_RETURN_DELAYS[itemBoomerang.Level];

			if (itemBoomerang.Level == Item.Level2) {
				physics.CustomTileIsNotSolidCondition = delegate(Tile tile) {
					// Don't collide with boomerangable tiles, but instead break them
					// as we pass over them.
					return !tile.IsBoomerangable;
				};
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (itemBoomerang.Level == Item.Level1)
				Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_BOOMERANG_1);
			else
				Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_BOOMERANG_2);

			collectibles = new List<Collectible>();
			tileLocation = new Point2I(-1, -1);
		}

		protected override void OnReturnedToOwner() {
			base.OnReturnedToOwner();

			// Collect the collectables.
			for (int i = 0; i < collectibles.Count; i++)
				collectibles[i].Collect();
		}

		public override void OnCollideMonster(Monster monster) {
			monster.TriggerInteraction(InteractionType.Boomerang, this);
		}

		public override void Update() {
			AudioSystem.LoopSoundWhileActive(GameData.SOUND_BOOMERANG_LOOP);

			// Check for boomerangable tiles.
			if (itemBoomerang.Level == Item.Level2) {
				Point2I tileLoc = RoomControl.GetTileLocation(position);
				if (tileLoc != tileLocation && RoomControl.IsTileInBounds(tileLoc)) {
					Tile tile = RoomControl.GetTopTile(tileLoc);
					if (tile != null)
						tile.OnBoomerang();
				}
				tileLocation = tileLoc;
			}

			// Pickup collectibles.
			foreach (Collectible collectible in Physics.GetEntitiesMeeting<Collectible>(CollisionBoxType.Soft)) {
				if (collectible.IsPickupable && collectible.IsCollectibleWithItems) {
					collectibles.Add(collectible);
					collectible.Destroy();
					BeginReturn();
				}
			}

			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);

			// Draw collectibles over boomerang.
			for (int i = 0; i < collectibles.Count; i++) {
				Collectible collectible = collectibles[i];
				collectible.SetPositionByCenter(Center);
				collectible.ZPosition = zPosition;
				float percent = i / (float) collectibles.Count;
				collectible.Graphics.Draw(g, Graphics.CurrentDepthLayer);
			}
		}
	}
}

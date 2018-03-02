using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Common.Audio;
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
			// Physics
			if (itemBoomerang.Level == Item.Level2) {
				physics.CustomTileIsNotSolidCondition = delegate(Tile tile) {
					// Don't collide with boomerangable tiles, but instead break them
					// as we pass over them
					return !tile.IsBoomerangable;
				};
			}

			// Interactions
			Interactions.InteractionType = InteractionType.Boomerang;
			Interactions.InteractionEventArgs = new WeaponInteractionEventArgs() {
				Weapon = itemBoomerang
			};

			// Boomerang
			moveSpeed = GameSettings.PROJECTILE_BOOMERANG_SPEEDS[
				itemBoomerang.Level];
			returnDelay = GameSettings.PROJECTILE_BOOMERANG_RETURN_DELAYS[
				itemBoomerang.Level];

			// Player Boomerang
			collectibles = new List<Collectible>();
			this.itemBoomerang = itemBoomerang;
		}


		//-----------------------------------------------------------------------------
		// Collectibles
		//-----------------------------------------------------------------------------

		/// <summary>Pickup a collectible and carry it with the boomerang to be
		/// collected upon returning to the player.</summary>
		public void GrabCollectible(Collectible collectible) {
			collectibles.Add(collectible);
			collectible.Destroy();
			BeginReturning();
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

			collectibles.Clear();
			tileLocation = new Point2I(-1, -1);
		}

		protected override void OnReturnedToOwner() {
			// Collect the collectables that were grabbed
			for (int i = 0; i < collectibles.Count; i++)
				collectibles[i].Collect();
			collectibles.Clear();
		}

		public override void Update() {
			AudioSystem.LoopSoundWhileActive(GameData.SOUND_BOOMERANG_LOOP);

			// Check for boomerangable tiles
			if (itemBoomerang.Level == Item.Level2) {
				Point2I tileLoc = RoomControl.GetTileLocation(position);
				if (tileLoc != tileLocation && RoomControl.IsTileInBounds(tileLoc)) {
					Tile tile = RoomControl.GetTopTile(tileLoc);
					if (tile != null)
						tile.OnBoomerang();
				}
				tileLocation = tileLoc;
			}

			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);

			// Draw collectibles over boomerang
			for (int i = 0; i < collectibles.Count; i++) {
				Collectible collectible = collectibles[i];
				collectible.SetPositionByCenter(Center);
				collectible.ZPosition = zPosition;
				float percent = i / (float) collectibles.Count;
				collectible.Graphics.Draw(g);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public ItemBoomerang Weapon {
			get { return itemBoomerang; }
			set { itemBoomerang = value; }
		}
	}
}

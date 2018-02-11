using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemBoomerang : ItemWeapon {

		private EntityTracker<Boomerang> boomerangTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBoomerang() {
			id			= "item_boomerang";
			name		= new string[] { "Boomerang", "Magic Boomerang" };
			description	= new string[] { "Always comes back to you.", "A remote-control weapon." };
			level		= 0;
			maxLevel	= Item.Level2;
			flags		=
				ItemFlags.UsableInMinecart |
				ItemFlags.UsableWhileJumping |
				ItemFlags.UsableWithSword |
				ItemFlags.UsableWhileInHole;

			boomerangTracker = new EntityTracker<Boomerang>(1);

			sprite = new ISprite[] {
				GameData.SPR_ITEM_ICON_BOOMERANG_1,
				GameData.SPR_ITEM_ICON_BOOMERANG_2
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnButtonPress() {
			if (boomerangTracker.IsMaxedOut)
				return;

			// Shoot and track the boomerang
			PlayerBoomerang boomerang = new PlayerBoomerang(this);
			Player.ShootFromAngle(boomerang, Player.UseAngle, boomerang.Speed);
			boomerangTracker.TrackEntity(boomerang);
			
			if (level == Item.Level1) {
				// Begin the standard busy state for the regular boomerang
				Player.BeginBusyState(10, Player.Animations.Throw);
			}
			else {
				// Enter a player state to control the magic boomerang
				Player.MagicBoomerangState.Weapon = this;
				Player.MagicBoomerangState.BoomerangEntity = boomerang;
				Player.BeginConditionState(Player.MagicBoomerangState);
			}
		}
	}
}

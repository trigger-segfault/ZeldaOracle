using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemBoomerang : ItemWeapon {

		private EntityTracker<Boomerang> boomerangTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		//public ItemBoomerang() : base("boomerang") {
		public ItemBoomerang(string id) : base(id) {
			/*SetName("Boomerang", "Magic Boomerang");
			SetDescription("Always comes back to you.", "A remote-control weapon.");
			SetMessage(
				"You got the <red>Boomerang<red>! " +
					"Use it to stop enemies in their tracks!",
				"It's the <red>Magical Boomerang<red>! " +
					"Press <dpad> while holding the button to control its flight path!");
			SetSprite(
				GameData.SPR_ITEM_ICON_BOOMERANG_1,
				GameData.SPR_ITEM_ICON_BOOMERANG_2);
			MaxLevel	= Item.Level2;

			HoldType = RewardHoldTypes.TwoHands;*/
			Flags =
				WeaponFlags.UsableInMinecart |
				WeaponFlags.UsableWhileJumping |
				WeaponFlags.UsableWithSword |
				WeaponFlags.UsableWhileInHole;

			boomerangTracker = new EntityTracker<Boomerang>(1);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override bool OnButtonPress() {
			if (boomerangTracker.IsMaxedOut)
				return false;

			// Shoot and track the boomerang
			PlayerBoomerang boomerang = new PlayerBoomerang(this);
			Player.ShootFromAngle(boomerang, Player.UseAngle, boomerang.Speed);
			boomerangTracker.TrackEntity(boomerang);
			
			if (Level == Item.Level1) {
				// Begin the standard busy state for the regular boomerang
				Player.BeginBusyState(10, Player.Animations.Throw);
			}
			else {
				// Enter a player state to control the magic boomerang
				Player.MagicBoomerangState.Weapon = this;
				Player.MagicBoomerangState.BoomerangEntity = boomerang;
				Player.BeginConditionState(Player.MagicBoomerangState);
			}
			
			return true;
		}
	}
}

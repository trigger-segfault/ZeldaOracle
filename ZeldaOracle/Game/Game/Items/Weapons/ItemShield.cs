using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemShield : ItemWeapon {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemShield() : base("item_shield") {
			SetName("Wooden Shield", "Iron Shield", "Mirror Shield");
			SetDescription("A small shield.", "A large shield.", "A reflective shield.");
			SetMessage(
				"You got a <red>Wooden Shield<red>!",
				"You got an <red>Iron Shield<red>!",
				"You got the <red>Mirror Shield<red>!");
			SetSprite(
				GameData.SPR_ITEM_ICON_SHIELD_1,
				GameData.SPR_ITEM_ICON_SHIELD_2,
				GameData.SPR_ITEM_ICON_SHIELD_3);
			MaxLevel = Item.Level3;
			HoldType = RewardHoldTypes.TwoHands;

			Flags =
				WeaponFlags.UsableWhileJumping |
				WeaponFlags.UsableWithSword |
				WeaponFlags.UsableWhileInHole;

		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnEquip() {
			// Begin the player's shield state
			Player.ShieldState.Weapon = this;
			Player.BeginConditionState(Player.ShieldState);
		}

		public override void OnUnequip() {
			// End the player's shield state
			if (Player.ShieldState.IsActive)
				Player.ShieldState.End();
		}
	}
}

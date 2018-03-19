using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemShield : ItemWeapon {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemShield() {
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

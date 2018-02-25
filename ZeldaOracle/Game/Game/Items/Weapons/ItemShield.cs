using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemShield : ItemWeapon {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemShield() {
			id			= "item_shield";
			name		= new string[] { "Wooden Shield", "Iron Shield", "Mirror Shield" };
			description	= new string[] { "A small shield.", "A large shield.", "A reflective shield." };
			maxLevel	= Item.Level3;

			flags =
				ItemFlags.UsableWhileJumping |
				ItemFlags.UsableWithSword |
				ItemFlags.UsableWhileInHole;

			sprite = new ISprite[] {
				GameData.SPR_ITEM_ICON_SHIELD_1,
				GameData.SPR_ITEM_ICON_SHIELD_2,
				GameData.SPR_ITEM_ICON_SHIELD_3
			};
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

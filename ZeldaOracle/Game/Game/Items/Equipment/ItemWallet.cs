using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Equipment {
	public class ItemWallet : Item {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemWallet() : base("item_wallet") {
			SetName("Child's Wallet", "Adult's Wallet", "Giant's Wallet");
			SetDescription(
				"Allows you to carry a measly 99 rupees.",
				"Allows you to carry 300 rupees!",
				"Allows you to carry a whopping 999 rupees!");
			SetSprite(
				GameData.SPR_ITEM_ICON_WALLET_1,
				GameData.SPR_ITEM_ICON_WALLET_2,
				GameData.SPR_ITEM_ICON_WALLET_3);
			SetAmmo("rupees", "ore_chunks");
			SetMaxAmmo(99, 300, 999);
			MaxLevel = Item.Level3;
			IncreaseAmmoOnLevelUp = false;
		}
	}
}

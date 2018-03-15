using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemOcarina : ItemWeapon {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemOcarina() : base("item_ocarina") {
			SetName("Ocarina");
			SetDescription("Plays a beautiful sound.");
			SetSprite(GameData.SPR_ITEM_ICON_OCARINA);
			Flags = WeaponFlags.None;
		}

	}
}

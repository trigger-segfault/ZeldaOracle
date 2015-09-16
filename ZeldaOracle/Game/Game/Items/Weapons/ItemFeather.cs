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

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemFeather : Item {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemFeather() : base() {
			id			= "item_feather";
			name		= new string[] { "Roc's Feather", "Roc's Cape" };
			description	= new string[] { "A nice lift.", "A wing-riding cape." };
			maxLevel	= 2;
			currentAmmo	= 0;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Jump/deploy cape.
		public override void OnButtonPress() {
			Player.Jump();
		}

	}
}

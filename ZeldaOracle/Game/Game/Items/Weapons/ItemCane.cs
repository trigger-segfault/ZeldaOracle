using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemCane : ItemWeapon {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemCane() {
			this.id				= "item_cane";
			this.name			= new string[] { "Cane of Somaria" };
			this.description	= new string[] { "Used to create blocks." };
			this.maxLevel		= Item.Level1;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWhileInHole;
			this.sprite			= new Sprite[] { GameData.SPR_ITEM_ICON_CANE };
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			Player.SwingCaneState.Weapon = this;
			Player.BeginState(Player.SwingCaneState);
		}
	}
}

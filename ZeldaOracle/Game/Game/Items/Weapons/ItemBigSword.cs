using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemBigSword : ItemWeapon {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBigSword() {
			this.id				= "item_biggoron_sword";
			this.name			= new string[] { "Biggoron's Sword" };
			this.description	= new string[] { "A powerful, two-handed sword." };
			this.sprite			= new ISprite[] { GameData.SPR_ITEM_ICON_BIGGORON_SWORD };
			this.spriteEquipped	= new ISprite[] { GameData.SPR_ITEM_ICON_BIGGORON_SWORD_EQUIPPED };
			this.flags			= ItemFlags.TwoHanded | ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWhileInHole;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			Player.SwingBigSwordState.Weapon = this;
			Player.BeginState(Player.SwingBigSwordState);
		}
	}
}

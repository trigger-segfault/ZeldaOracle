using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemSword : ItemWeapon {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSword() {
			this.id				= "item_sword";
			this.name			= new string[] { "Wooden Sword", "Noble Sword", "Master Sword" };
			this.description	= new string[] { "A hero's blade.", "A sacred blade.", "The blade of legends." };
			this.maxLevel		= Item.Level3;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWithSword;

			sprite = new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(0, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(1, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(2, 0))
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the item is switched to.
		public override void OnStart() { }

		// Called when the item is put away.
		public override void OnEnd() { }

		// Immediately interrupt this item (ex: if the player falls in a hole).
		public override void Interrupt() { }

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			Player.SwingState.NextState = Player.HoldSwordState;
			Player.SwingState.EquipSlot = CurrentEquipSlot;
			Player.SwingState.WeaponAnimation = GameData.ANIM_SWORD_SWING;
			Player.HoldSwordState.EquipSlot = CurrentEquipSlot;
			Player.HoldSwordState.WeaponAnimation = GameData.ANIM_SWORD_HOLD;
			Player.SpinSwordState.WeaponAnimation = GameData.ANIM_SWORD_SPIN;
			Player.BeginState(Player.SwingState);
		}

		// Update the item.
		public override void Update() { }

		// Draws under link's sprite.
		public override void DrawUnder() { }

		// Draws over link's sprite.
		public override void DrawOver() { }
	}
}

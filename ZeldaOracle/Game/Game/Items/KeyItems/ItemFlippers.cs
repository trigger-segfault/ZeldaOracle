using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Items.KeyItems {
	public class ItemFlippers : ItemSecondary {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemFlippers() {
			this.id = "item_flippers";
			this.name = new string[] { "Zora's Flippers", "Mermaid Suit" };
			this.description = new string[] { "Hit the beach.", "The skin of the mythical beast." };
			this.maxLevel = Item.Level2;
			this.slot = new Point2I(0, 0);
			this.sprite = new Sprite[] {
				GameData.SPR_ITEM_ICON_FLIPPERS_1,
				GameData.SPR_ITEM_ICON_FLIPPERS_2
			};
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Called when the item's level is changed.
		public override void OnLevelUp() {
			Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInWater;
			if (level == Item.Level2)
				Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInOcean;
		}

		// Called when the item has been obtained.
		public override void OnObtained() {
			Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInWater;
			if (level == Item.Level2)
				Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInOcean;
		}

		// Called when the item has been unobtained.
		public override void OnUnobtained() {
			Player.SwimmingSkills &= ~(PlayerSwimmingSkills.CanSwimInWater | PlayerSwimmingSkills.CanSwimInOcean);
		}

		// Called when the item has been stolen.
		public override void OnStolen() {
			Player.SwimmingSkills &= ~(PlayerSwimmingSkills.CanSwimInWater | PlayerSwimmingSkills.CanSwimInOcean);
		}

		// Called when the stolen item has been returned.
		public override void OnReturned() {
			Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInWater;
			if (level == Item.Level2)
				Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInOcean;
		}
	}
}

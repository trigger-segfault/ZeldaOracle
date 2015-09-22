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
				new Sprite(GameData.SHEET_ITEMS_LARGE, new Point2I(6, 1)),
				new Sprite(GameData.SHEET_ITEMS_LARGE, new Point2I(7, 1))
			};
			this.spriteLight = new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_LARGE_LIGHT, new Point2I(6, 1)),
				new Sprite(GameData.SHEET_ITEMS_LARGE_LIGHT, new Point2I(7, 1))
			};
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Called when the item's level is changed.
		public virtual void OnLevelUp() {
			Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInWater;
			if (level == Item.Level2)
				Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInOcean;
		}

		// Called when the item has been obtained.
		public virtual void OnObtained() {
			Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInWater;
			if (level == Item.Level2)
				Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInOcean;
		}

		// Called when the item has been unobtained.
		public virtual void OnUnobtained() {
			Player.SwimmingSkills &= ~(PlayerSwimmingSkills.CanSwimInWater | PlayerSwimmingSkills.CanSwimInOcean);
		}

		// Called when the item has been stolen.
		public virtual void OnStolen() {
			Player.SwimmingSkills &= ~(PlayerSwimmingSkills.CanSwimInWater | PlayerSwimmingSkills.CanSwimInOcean);
		}

		// Called when the stolen item has been returned.
		public virtual void OnReturned() {
			Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInWater;
			if (level == Item.Level2)
				Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInOcean;
		}
	}
}

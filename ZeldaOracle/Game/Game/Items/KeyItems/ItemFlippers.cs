using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.KeyItems {
	public class ItemFlippers : ItemSecondary {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public ItemFlippers(string id) : base(id) {
			/*SetName("Zora's Flippers", "Mermaid Suit");
			SetDescription("Hit the beach.", "The skin of the mythical beast.");
			SetMessage(
				"You got <red>Zora's Flippers<red>! You can now go for a swim! " +
					"Press <a> to swim, <b> to dive!",
				"You got a <red>Mermaid Suit<red>! Now you can swim in deep waters. " +
					"Press <dpad> to swim, <b> to dive and <a> to use items.");
			SetSprite(
				GameData.SPR_ITEM_ICON_FLIPPERS_1,
				GameData.SPR_ITEM_ICON_FLIPPERS_2);
			MaxLevel = Item.Level2;
			HoldType = RewardHoldTypes.TwoHands;

			slot = new Point2I(0, 0);*/
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		// Called when the item's level is changed.
		protected override void OnLevelUp() {
			OnObtained();
		}

		// Called when the item has been obtained.
		protected override void OnObtained() {
			Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInWater;
			if (Level == Item.Level2)
				Player.SwimmingSkills |= PlayerSwimmingSkills.CanSwimInOcean;
		}

		// Called when the item has been unobtained.
		protected override void OnUnobtained() {
			Player.SwimmingSkills &= ~(PlayerSwimmingSkills.CanSwimInWater | PlayerSwimmingSkills.CanSwimInOcean);
		}

		// Called when the item has been lost.
		protected override void OnLost() {
			OnUnobtained();
		}

		// Called when the lost item has been returned.
		protected override void OnReobtained() {
			OnObtained();
		}
	}
}

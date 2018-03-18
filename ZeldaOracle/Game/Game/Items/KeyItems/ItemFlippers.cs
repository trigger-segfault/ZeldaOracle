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

		public ItemFlippers() {
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

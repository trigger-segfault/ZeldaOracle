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
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Items.Ammos;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;

namespace ZeldaOracle.Game.Items.Weapons {

	public abstract class SeedBasedItem : ItemWeapon {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SeedBasedItem(string id) : base(id) {
			SetAmmo(
				"ember_seeds",
				"scent_seeds",
				"pegasus_seeds",
				"gale_seeds",
				"mystery_seeds");
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position) {
			DrawSprite(g, position);
			DrawAmmo(g, position);
			g.DrawSprite(CurrentAmmo.Sprite, position + new Point2I(8, 0));
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SeedType CurrentSeedType {
			get {
				switch (CurrentAmmo.ID) {
				case "ember_seeds":	return SeedType.Ember;
				case "scent_seeds":	return SeedType.Scent;
				case "pegasus_seeds":	return SeedType.Pegasus;
				case "gale_seeds":		return SeedType.Gale;
				case "mystery_seeds":	return SeedType.Mystery;
				default: return SeedType.Ember;
				}
			}
		}
	}
}

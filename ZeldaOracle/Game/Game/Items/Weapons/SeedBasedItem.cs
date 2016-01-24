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

	public class SeedBasedItem : ItemWeapon {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SeedBasedItem() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		// Called when the item is added to the inventory list.
		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);
			
			currentAmmo = 0;

			ammo = new Ammo[] {
				inventory.GetAmmo("ammo_ember_seeds"),
				inventory.GetAmmo("ammo_scent_seeds"),
				inventory.GetAmmo("ammo_pegasus_seeds"),
				inventory.GetAmmo("ammo_gale_seeds"),
				inventory.GetAmmo("ammo_mystery_seeds"),
			};
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SeedType CurrentSeedType {
			get {
				string ammoID = ammo[currentAmmo].ID;
				if (ammoID == "ammo_ember_seeds")
					return SeedType.Ember;
				else if (ammoID == "ammo_scent_seeds")
					return SeedType.Scent;
				else if (ammoID == "ammo_gale_seeds")
					return SeedType.Gale;
				else if (ammoID == "ammo_mystery_seeds")
					return SeedType.Mystery;
				else if (ammoID == "ammo_pegasus_seeds")
					return SeedType.Pegasus;
				return SeedType.Ember;
			}
		}
	}
}

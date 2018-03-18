using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemSword : ItemWeapon {
		
		private EntityTracker<SwordBeam> beamTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSword() {
			Flags =
				WeaponFlags.UsableInMinecart |
				WeaponFlags.UsableUnderwater |
				WeaponFlags.UsableWhileJumping |
				WeaponFlags.UsableWhileInHole;

			beamTracker    = new EntityTracker<SwordBeam>(1);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override bool OnButtonPress() {
			Player.SwingSwordState.Weapon = this;
			Player.BeginWeaponState(Player.SwingSwordState);
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EntityTracker<SwordBeam> BeamTracker {
			get { return beamTracker; }
		}
	}
}

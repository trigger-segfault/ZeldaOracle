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

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemMagicRod : ItemWeapon {
		
		private EntityTracker<MagicRodFire> fireTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		//public ItemMagicRod() : base("magic_rod") {
		public ItemMagicRod(string id) : base(id) {
			/*SetName("Magic Rod");
			SetDescription("Burn, baby burn!");
			SetSprite(GameData.SPR_ITEM_ICON_MAGIC_ROD);*/

			Flags =
				WeaponFlags.UsableInMinecart |
				WeaponFlags.UsableWhileJumping |
				WeaponFlags.UsableWhileInHole;

			fireTracker	= new EntityTracker<MagicRodFire>(2);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override bool OnButtonPress() {
			Player.SwingMagicRodState.Weapon = this;
			Player.BeginWeaponState(Player.SwingMagicRodState);
			return true;
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EntityTracker<MagicRodFire> FireTracker {
			get { return fireTracker; }
		}
	}
}

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

		public ItemMagicRod() {
			this.id				= "item_magic_rod";
			this.name			= new string[] { "Magic Rod" };
			this.description	= new string[] { "Burn, baby burn!" };
			this.maxLevel		= Item.Level1;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWhileInHole;
			this.sprite			= new ISprite[] { GameData.SPR_ITEM_ICON_MAGIC_ROD };
			this.fireTracker	= new EntityTracker<MagicRodFire>(2);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			Player.SwingMagicRodState.Weapon = this;
			Player.BeginWeaponState(Player.SwingMagicRodState);
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EntityTracker<MagicRodFire> FireTracker {
			get { return fireTracker; }
		}
	}
}

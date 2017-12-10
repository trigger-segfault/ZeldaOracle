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
	public class ItemSword : ItemWeapon {
		
		private EntityTracker<SwordBeam> beamTracker;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSword() {
			this.id				= "item_sword";
			this.name			= new string[] { "Wooden Sword", "Noble Sword", "Master Sword" };
			this.description	= new string[] { "A hero's blade.", "A sacred blade.", "The blade of legends." };
			this.maxLevel		= Item.Level3;
			this.flags			= 
				ItemFlags.UsableInMinecart |
				ItemFlags.UsableUnderwater |
				ItemFlags.UsableWhileJumping | 
				ItemFlags.UsableWhileInHole;
			this.beamTracker	= new EntityTracker<SwordBeam>(1);
			this.sprite			= new ISprite[] {
				GameData.SPR_ITEM_ICON_SWORD_1,
				GameData.SPR_ITEM_ICON_SWORD_2,
				GameData.SPR_ITEM_ICON_SWORD_3
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			Player.SwingSwordState.Weapon = this;
			Player.BeginState(Player.SwingSwordState);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EntityTracker<SwordBeam> BeamTracker {
			get { return beamTracker; }
		}
	}
}

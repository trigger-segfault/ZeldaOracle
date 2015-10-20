using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

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
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWhileInHole;
			this.beamTracker	= new EntityTracker<SwordBeam>(1);

			sprite = new Sprite[] {
				GameData.SPR_ITEM_ICON_SWORD_1,
				GameData.SPR_ITEM_ICON_SWORD_2,
				GameData.SPR_ITEM_ICON_SWORD_3
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Immediately interrupt this item (ex: if the player falls in a hole).
		public override void Interrupt() {
			//if (Player.CurrentState == Player.HoldSwordState && !Player.HoldSwordState.IsStabbing) {
			//	Player.BeginNormalState();
			//}
		}

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {

			Player.SwingState.NextState				= Player.HoldSwordState;
			Player.SwingState.Weapon				= this;
			Player.SwingState.WeaponAnimation		= GameData.ANIM_SWORD_SWING;

			Player.HoldSwordState.Weapon			= this;
			Player.HoldSwordState.WeaponAnimation	= GameData.ANIM_SWORD_HOLD;
			Player.SpinSwordState.WeaponAnimation	= GameData.ANIM_SWORD_SPIN;

			Player.SwordStabState.Weapon			= this;

			Player.BeginState(Player.SwingState);
		}

		// Update the item.
		public override void Update() { }

		// Draws under link's sprite.
		public override void DrawUnder() { }

		// Draws over link's sprite.
		public override void DrawOver() { }
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EntityTracker<SwordBeam> BeamTracker {
			get { return beamTracker; }
		}
	}
}

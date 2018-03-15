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

		public ItemSword(string id) : base(id) {
			SetName("Wooden Sword", "Noble Sword", "Master Sword");
			SetDescription("A hero's blade.", "A sacred blade.", "The blade of legends.");
			SetMessage(
				"You got a Hero's <red>Wooden Sword<red>! " +
					"Hold <a> or <b> to charge it up, then release it for a spin attack!",
				"You got the sacred <red>Noble Sword<red>!",
				"You got the legendary <red>Master Sword<red>!");
			SetSprite(
				GameData.SPR_ITEM_ICON_SWORD_1,
				GameData.SPR_ITEM_ICON_SWORD_2,
				GameData.SPR_ITEM_ICON_SWORD_3);
			MaxLevel = Item.Level3;
			HoldType = RewardHoldTypes.OneHand;

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

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
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemFeather : ItemWeapon {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemFeather(int level = 0) {
			this.id				= "item_feather";
			this.name			= new string[] { "Roc's Feather", "Roc's Cape" };
			this.description	= new string[] { "A nice lift.", "A wing-riding cape." };
			this.level			= level;
			this.maxLevel		= Item.Level2;
			this.flags			= ItemFlags.UsableWhileJumping | ItemFlags.UsableWithSword;

			sprite = new ISprite[] {
				GameData.SPR_ITEM_ICON_FEATHER,
				GameData.SPR_ITEM_ICON_CAPE
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Check if the player is allowed to jump.
		public override bool IsUsable() {
			return (Player.Movement.CanJump &&
					!Player.IsInMinecart &&
					!Player.IsUnderwater);
		}

		// Jump when on ground.
		public override void OnButtonPress() {
			if (Player.IsOnGround && !Player.Physics.IsInHole) {
				Player.Movement.Jump();
			}
		}

		// Deplay cape when in air.
		public override void OnButtonDown() {
			if (Player.IsInAir && level == ItemWeapon.Level2) {
				Player.Movement.DeployCape();
			}
		}
	}
}

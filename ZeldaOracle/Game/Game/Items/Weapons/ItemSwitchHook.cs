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
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemSwitchHook : ItemWeapon {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSwitchHook() {
			this.id				= "item_switch_hook";
			this.name			= new string[] { "Switch Hook", "Long Hook" };
			this.description	= new string[] { "User and target trade places.", "Switches places from a distance." };
			this.level			= 0;
			this.maxLevel		= Item.Level2;
			this.flags			=
				ItemFlags.UsableWithSword |
				ItemFlags.UsableUnderwater;
			this.sprite = new Sprite[] {
				GameData.SPR_ITEM_ICON_SWITCH_HOOK,
				GameData.SPR_ITEM_ICON_SWITCH_HOOK
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnButtonPress() {

			Player.Direction = Player.UseDirection;

			// Shoot and hook projectile.
			SwitchHookProjectile hook = new SwitchHookProjectile(level);
			Player.ShootFromDirection(hook, Player.Direction, hook.Speed);

			// Begin the player state.
			Player.SwitchHookState.Hook = hook;
			Player.BeginState(Player.SwitchHookState);
		}
	}
}

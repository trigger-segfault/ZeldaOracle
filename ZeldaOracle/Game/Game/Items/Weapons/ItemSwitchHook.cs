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
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemSwitchHook : ItemWeapon {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		//public ItemSwitchHook() : base("switch_hook") {
		public ItemSwitchHook(string id) : base(id) {
			SetName("Switch Hook", "Long Hook");
			SetDescription("User and target trade places.", "Switches places from a distance.");
			SetSprite(GameData.SPR_ITEM_ICON_SWITCH_HOOK);
			MaxLevel = Item.Level2;
			HoldType = RewardHoldTypes.TwoHands;
			Flags =
				WeaponFlags.UsableWithSword |
				WeaponFlags.UsableUnderwater;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override bool OnButtonPress() {
			Player.Direction = Player.UseDirection;

			// Shoot the switch hook projectile
			SwitchHookProjectile hook = new SwitchHookProjectile(this);
			Player.ShootFromDirection(hook, Player.Direction, hook.Speed);

			// Begin the player's switch hook state
			Player.SwitchHookState.Hook = hook;
			Player.BeginWeaponState(Player.SwitchHookState);
			return true;
		}
	}
}

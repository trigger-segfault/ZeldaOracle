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

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemBoomerang : ItemWeapon {

		private EntityTracker<Boomerang> boomerangTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBoomerang() {
			this.id				= "item_boomerang";
			this.name			= new string[] { "Boomerang", "Magic Boomerang" };
			this.description	= new string[] { "Always comes back to you.", "A remote-control weapon." };
			this.level			= 0;
			this.maxLevel		= Item.Level2;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWithSword | ItemFlags.UsableWhileInHole;
			this.boomerangTracker	= new EntityTracker<Boomerang>(1);

			sprite = new ISprite[] {
				GameData.SPR_ITEM_ICON_BOOMERANG_1,
				GameData.SPR_ITEM_ICON_BOOMERANG_2
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnButtonPress() {
			if (boomerangTracker.IsMaxedOut)
				return;

			// Shoot and track the boomerang.
			PlayerBoomerang boomerang = new PlayerBoomerang(this);
			Player.ShootFromAngle(boomerang, Player.UseAngle, boomerang.Speed);
			boomerangTracker.TrackEntity(boomerang);

			if (level == Item.Level1) {
				// Begin the standard busy state for the regular boomerang.
				Player.BeginBusyState(10, GameData.ANIM_PLAYER_THROW, GameData.ANIM_PLAYER_MINECART_THROW);
			}
			else {
				// Enter a player state to control the magic boomerang.
				Player.MagicBoomerangState.Weapon = this;
				Player.MagicBoomerangState.BoomerangEntity = boomerang;
				Player.BeginState(Player.MagicBoomerangState);
			}
		}
	}
}

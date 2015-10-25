using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Players.States.SwingStates {

	public class PlayerSwingBigSwordState : PlayerBaseSwingSwordState {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerSwingBigSwordState() {
			isReswingable			= false;
			lunge					= false;
			swingAnglePullBack		= 2;
			swingAngleDurations		= new int[] { 12, 4, 4, 4, 10 };
			weaponSwingAnimation	= GameData.ANIM_BIG_SWORD_SWING;
			playerSwingAnimation	= GameData.ANIM_PLAYER_SWING_BIG;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnSwingBegin() {
			AudioSystem.PlayRandomSound("Items/slash_1", "Items/slash_2", "Items/slash_3");
		}

		public override void OnSwingEnd() {
			player.BeginNormalState();
		}
		
		public override void OnHitMonster(Monster monster) {
			monster.TriggerInteraction(monster.HandlerBigSword, Weapon as ItemBigSword);
		}
	}
}

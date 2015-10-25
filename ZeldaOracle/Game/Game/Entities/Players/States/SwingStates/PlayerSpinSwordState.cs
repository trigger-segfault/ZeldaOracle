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

	public class PlayerSpinSwordState : PlayerBaseSwingSwordState {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerSpinSwordState() {
			limitTilesToDirection	= false;
			isReswingable			= false;
			lunge					= false;
			swingAnglePullBack		= 0;
			swingAngleDurations		= new int[] { 3, 2, 3, 2, 3, 2, 3, 2, 5 };
			weaponSwingAnimation	= GameData.ANIM_SWORD_SPIN;
			playerSwingAnimation	= GameData.ANIM_PLAYER_SPIN;
			
			// Will always spin clockwise.
			swingWindingOrders = new WindingOrder[] {
				WindingOrder.Clockwise,
				WindingOrder.Clockwise,
				WindingOrder.Clockwise,
				WindingOrder.Clockwise
			};
		}


		//-----------------------------------------------------------------------------
		// Sword Methods
		//-----------------------------------------------------------------------------

		public override void OnSwingTilePeak(int angle, Point2I tileLocation) {
			// Don't cut the tile when the swing is started.
			if (SwingAngleIndex > 0)
				base.OnSwingTilePeak(angle, tileLocation);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnSwingBegin() {
			// Play sword-spin sound.
		}

		public override void OnSwingEnd() {
			// Cut the center tile.
			CutTileAtLocation(player.RoomControl.GetTileLocation(player.Center));
			player.BeginNormalState();
		}
	}
}

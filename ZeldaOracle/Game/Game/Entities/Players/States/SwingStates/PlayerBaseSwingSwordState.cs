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
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Players.States.SwingStates {

	public class PlayerBaseSwingSwordState : PlayerSwingState {

		protected bool limitTilesToDirection;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerBaseSwingSwordState() {
			isReswingable			= true;
			lunge					= true;
			swingAnglePullBack		= 2;
			swingAngleDurations		= new int[] { 3, 3, 12 };
			weaponSwingAnimation	= GameData.ANIM_SWORD_SWING;
			playerSwingAnimation	= GameData.ANIM_PLAYER_SWING;

			limitTilesToDirection	= false;
		}


		//-----------------------------------------------------------------------------
		// Sword Methods
		//-----------------------------------------------------------------------------

		protected void CutTilesAtPoint(Vector2F point) {
			Tile tile = player.RoomControl.TileManager.GetTopTileAtPosition(point);
			if (tile != null)
				tile.OnSwordHit(Weapon);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override UnitTool GetSwingTool() {
			return player.ToolSword;
		}

		public override void OnSwingEnd() {
			End();
		}

		public override void OnSwingTilePeak(int angle, Vector2F hitPoint) {
			if ((angle == Directions.ToAngle(SwingDirection)
				|| !limitTilesToDirection) && player.IsOnGround)
			{
				CutTilesAtPoint(hitPoint);
			}
		}
	}
}

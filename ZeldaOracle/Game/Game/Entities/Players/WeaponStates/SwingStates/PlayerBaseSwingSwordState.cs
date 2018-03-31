using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Entities.Players.States.SwingStates {

	public class PlayerBaseSwingSwordState : PlayerSwingState {

		protected bool limitTilesToDirection;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerBaseSwingSwordState() {
			limitTilesToDirection = false;
			InitStandardSwing(GameData.ANIM_SWORD_SWING,
				GameData.ANIM_SWORD_MINECART_SWING);
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

		public override void OnSwingTilePeak(Angle angle, Vector2F hitPoint) {
			if ((angle == SwingDirection.ToAngle()
				|| !limitTilesToDirection) && player.IsOnGround)
			{
				CutTilesAtPoint(hitPoint);
			}
		}
	}
}

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

		protected void CutTileAtLocation(Point2I location) {
			Tile tile = player.RoomControl.GetTopTile(location);
			if (tile != null)
				tile.OnSwordHit(Weapon);
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------
		
		// NOTE: this has been moved into the UnitTool class.
		//public virtual void OnHitMonster(Monster monster) {
			//monster.TriggerInteraction(monster.HandlerSword, Weapon as ItemSword);
		//}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		
		public override UnitTool GetSwingTool() {
			return player.ToolSword;
		}

		public override void OnSwingEnd() {
			player.BeginNormalState();
		}

		public override void OnSwingTilePeak(int angle, Point2I tileLocation) {
			if ((angle == Directions.ToAngle(SwingDirection) || !limitTilesToDirection) &&
				player.IsOnGround && player.RoomControl.IsTileInBounds(tileLocation))
			{
				CutTileAtLocation(tileLocation);
			}
		}
		
		// NOTE: this has been moved into the UnitTool class.
		/*public override void OnSwingEntityPeak(int angle, Rectangle2F collisionBox) {
			// Collide with entities.
			for (int i = 0; i < player.RoomControl.EntityCount; i++) {
				Entity e = player.RoomControl.Entities[i];
				if (e.Physics.PositionedSoftCollisionBox.Colliding(collisionBox)) {
					if (e is Collectible && (e as Collectible).IsPickupable && (e as Collectible).IsCollectibleWithItems) {
						(e as Collectible).Collect();
					}
					if (e is Monster) {
						OnHitMonster((Monster) e);
						if (!IsActive)
							return;
					}
				}
			}
		}*/
	}
}

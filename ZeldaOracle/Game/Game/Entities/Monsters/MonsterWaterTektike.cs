using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterWaterTektike : BasicMonster {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public MonsterWaterTektike() {
			// General
			MaxHealth		= 2;
			ContactDamage	= 2;
			Color			= MonsterColor.Green;

			// Movement
			moveSpeed					= 0.0f;
			numMoveAngles				= 8;
			facePlayerOdds				= 0;
			stopTime.Set(10, 10);
			moveTime.Set(60, 70);
			changeDirectionsOnCollide	= false;
			syncAnimationWithDirection	= false;
			playAnimationOnlyWhenMoving	= false;
			avoidHazardTiles			= false;

			// Physics
			Physics.Gravity					= 0.0f;
			Physics.ReboundRoomEdge			= true;
			physics.DisableSurfaceContact	= true;
			Physics.CustomTileIsSolidCondition = delegate(Tile tile) {
				// Collide with non-water tiles
				return !tile.IsWater;
			};

			// Graphics
			animationMove = GameData.ANIM_MONSTER_WATER_TEKTIKE;
			
			// Projectile interactions
			Interactions.SetReaction(InteractionType.SwitchHook, SenderReactions.Intercept, MonsterReactions.Kill);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		protected override bool CanMoveInAngle(int moveAngle) {
			// Only move diagonally, and don't move in the same angle twice in row
			return (moveAngle % 2 == 1 && moveAngle != this.moveAngle);
		}

		public override void UpdateAI() {
			// Accelerate and decelerate
			if (isMoving) {
				if (moveTimer <= GameSettings.MONSTER_WATER_TEKTIKE_DECELERATION_TIME)
					speed = GMath.Max(0.0f, speed -
						GameSettings.MONSTER_WATER_TEKTIKE_DECELERATION);
				else
					speed = GMath.Min(GameSettings.MONSTER_WATER_TEKTIKE_MOVE_SPEED,
						speed + GameSettings.MONSTER_WATER_TEKTIKE_ACCELERATION);
			}

			base.UpdateAI();
		}
	}
}

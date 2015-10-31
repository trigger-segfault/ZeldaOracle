using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Monsters;

namespace ZeldaOracle.Game.Entities.Effects {
	public class Fire : Effect {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Fire() :
			base(GameData.ANIM_EFFECT_SEED_EMBER)
		{
			EnablePhysics(PhysicsFlags.HasGravity);
			Physics.AddCollisionHandler(typeof(Monster), CollisionBoxType.Soft, CollideWithMonster);

			Graphics.DrawOffset	= new Point2I(0, -2);
			Graphics.DepthLayer	= DepthLayer.EffectFire;
		}
		

		//-----------------------------------------------------------------------------
		// Collision Handlers
		//-----------------------------------------------------------------------------

		private void CollideWithMonster(Entity entity) {
			Monster monster = entity as Monster;
			monster.TriggerInteraction(InteractionType.Fire, this);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnDestroy() {
			// Burn tiles.
			Point2I location = RoomControl.GetTileLocation(position);
			if (RoomControl.IsTileInBounds(location)) {
				Tile tile = RoomControl.GetTopTile(location);
				if (tile != null)
					tile.OnBurn();
			}
		}
	}
}

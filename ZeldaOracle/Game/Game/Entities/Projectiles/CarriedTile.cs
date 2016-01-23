using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	public class CarriedTile : Entity {
		private Tile tile;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CarriedTile(Tile tile) {
			this.tile = tile;

			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-3, -5, 6, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-7, -7, 14, 14);
			EnablePhysics(
				PhysicsFlags.HasGravity |
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.DestroyedInHoles);

			// Graphics.
			Graphics.DepthLayer			= DepthLayer.ProjectileCarriedTile;
			Graphics.DrawOffset			= new Point2I(-8, -13);
			centerOffset				= new Point2I(0, -5);
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		public void Break() {
			if (tile.BreakAnimation != null) {
				Effect breakEffect = new Effect(tile.BreakAnimation, DepthLayer.EffectTileBreak, true);
				RoomControl.SpawnEntity(breakEffect, Center);
			}
			if (tile.BreakSound != null)
				AudioSystem.PlaySound(tile.BreakSound);
			Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			if (!tile.SpriteAsObject.IsNull)
				Graphics.PlayAnimation(tile.SpriteAsObject);
			else
				Graphics.PlayAnimation(tile.CurrentSprite);
		}

		public override void OnLand() {
			// Collide with monsters.
			CollisionIterator iterator = new CollisionIterator(this, typeof(Monster), CollisionBoxType.Soft);
			for (iterator.Begin(); iterator.IsGood(); iterator.Next()) {
				Monster monster = iterator.CollisionInfo.Entity as Monster;
				monster.TriggerInteraction(InteractionType.ThrownObject, this);
				if (IsDestroyed)
					return;
			}

			Break();
		}

		public override void Update() {
			base.Update();

			if (Physics.IsColliding)
				Break();
		}
	}
}

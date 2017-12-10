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

namespace ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles {
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
			
			Graphics.ImageVariant = tile.Graphics.ImageVariant;
			
			if (tile.SpriteAsObject != null)
				Graphics.PlayAnimation(tile.SpriteAsObject);
			else
				Graphics.PlayAnimation(tile.Graphics.AnimationPlayer.SpriteOrSubStrip);
		}

		public override void OnLand() {
			// Collide with monsters.
			foreach (Monster monster in Physics.GetEntitiesMeeting<Monster>(CollisionBoxType.Soft)) {
				monster.TriggerInteraction(InteractionType.ThrownObject, this);
				if (IsDestroyed)
					return;
			}

			// Collide with surface tiles.
			Point2I tileLoc = RoomControl.GetTileLocation(position);
			if (RoomControl.IsTileInBounds(tileLoc)) {
				Tile tile = RoomControl.GetTopTile(tileLoc);
				if (tile != null) {
					tile.OnHitByThrownObject(this);
					if (IsDestroyed)
						return;
				}
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

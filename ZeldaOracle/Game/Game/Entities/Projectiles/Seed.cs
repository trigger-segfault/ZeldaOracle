using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	
	public enum SeedType {
		Ember	= 0,
		Scent	= 1,
		Pegasus	= 2,
		Gale	= 3,
		Mystery	= 4
	}

	// This class handles both dropped seeds and projectile seeds.
	public class Seed : Entity {

		private SeedType type;
		private bool isProjectile;
		private int reboundCounter;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Seed(SeedType type, bool isProjectile) {
			this.type = type;
			this.isProjectile = isProjectile;

			if (isProjectile) {
				// Seed projectiles shot from a slingshot or seed-shooter.
				EnablePhysics(
					PhysicsFlags.DestroyedOutsideRoom |
					PhysicsFlags.CollideWorld |
					PhysicsFlags.ReboundSolid |
					PhysicsFlags.HalfSolidPassable |
					PhysicsFlags.LedgePassable);

				Physics.CollisionBox		= new Rectangle2F(-1, -5, 2, 1);
				Physics.SoftCollisionBox	= new Rectangle2F(-1, -5, 2, 1);
			}
			else {
				// Seed's dropped from satchel.
				EnablePhysics(
					PhysicsFlags.HasGravity |
					PhysicsFlags.DestroyedOutsideRoom |
					PhysicsFlags.DestroyedInHoles);
				
				Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 2);
				Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 2);
			}

			graphics.DrawOffset	= new Point2I(-4, -6);
			centerOffset		= new Point2I(0, -2);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnLand() {
			if (isProjectile)
				return;

			// Notify the tile of the seed hitting it.
			Point2I location = RoomControl.GetTileLocation(position);
			Tile tile = RoomControl.GetTopTile(location);
			if (tile != null) {
				tile.OnSeedHit(this);
				if (IsDestroyed)
					return;
			}

			// Create the seed's effect.
			if (type == SeedType.Ember) {
				RoomControl.SpawnEntity(new Fire(), Center);
			}
			else if (type == SeedType.Scent) {
				RoomControl.SpawnEntity(new ScentPod(), Center);
			}
			else if (type == SeedType.Mystery) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_SEED_MYSTERY), Center);
			}
			else if (type == SeedType.Gale) {
				RoomControl.SpawnEntity(new EffectGale(), Center);
			}

			Destroy();
		}
		
		public void CrashOnTile(Tile tile) {
			if (!isProjectile)
				return;

			// Move 3 pixels into the block from where it collided.
			position += Physics.PreviousVelocity.Normalized * 3.0f;

			// Notify the tile of the seed hitting it.
			if (tile != null) {
				tile.OnSeedHit(this);
				if (IsDestroyed)
					return;
			}

			// Create the seed's effect.
			if (type == SeedType.Ember) {
				RoomControl.SpawnEntity(new Fire(), Center - new Vector2F(0, 1), zPosition);
			}
			else if (type == SeedType.Scent) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_SEED_SCENT), Center, zPosition);
			}
			else if (type == SeedType.Mystery) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_SEED_MYSTERY), Center, zPosition);
			}
			else if (type == SeedType.Pegasus) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_SEED_PEGASUS), Center, zPosition);
			}
			else if (type == SeedType.Gale) {
				RoomControl.SpawnEntity(new EffectGale(), Center, zPosition);
			}

			Destroy();
		}

		public override void Initialize() {
			base.Initialize();
			Graphics.PlaySprite(GameData.SPR_ITEM_SEEDS[(int) type]);
			reboundCounter = 0;
		}

		public override void Update() {
			base.Update();

			if (physics.IsColliding) {
				reboundCounter++;
				Tile tile = null;

				// Find the tile we collided with.
				for (int dir = 0; dir < 4; dir++) {
					CollisionInfo collisionInfo = physics.CollisionInfo[dir];
					if (collisionInfo.IsColliding && collisionInfo.Type == CollisionType.Tile) {
						tile = collisionInfo.Tile;
						break;
					}
				}

				if (reboundCounter >= 3 || (tile != null && tile.Flags.HasFlag(TileFlags.AbsorbSeeds))) {
					CrashOnTile(tile);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SeedType Type {
			get { return type; }
		}
	}
}

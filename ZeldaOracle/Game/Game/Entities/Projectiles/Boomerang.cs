using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Entities.Collisions;

namespace ZeldaOracle.Game.Entities.Projectiles {
	public class Boomerang : Projectile, IInterceptable {

		private bool isReturning;
		private float speed;
		private int timer;
		private int returnDelay;
		private int level;
		private List<Collectible> collectibles;
		private Point2I tileLocation;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Boomerang(int level) {
			this.level = level;

			speed		= GameSettings.PROJECTILE_BOOMERANG_SPEEDS[level];
			returnDelay	= GameSettings.PROJECTILE_BOOMERANG_RETURN_DELAYS[level];

			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 2);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 2);
			EnablePhysics(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.CollideRoomEdge);

			if (level == Item.Level2) {
				physics.CustomTileCollisionCondition = delegate(Tile tile) {
					return !tile.IsBoomerangable;
				};
			}
			
			// Graphics.
			Graphics.DepthLayer			= DepthLayer.ProjectileBoomerang;
			Graphics.IsShadowVisible	= false;
		}

		
		//-----------------------------------------------------------------------------
		// Boomerang Methods
		//-----------------------------------------------------------------------------

		public void BeginReturn() {
			if (!isReturning) {
				isReturning					= true;
				physics.CollideWithWorld	= false;
				physics.CollideWithRoomEdge	= false;
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (level == Item.Level1)
				Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_BOOMERANG_1);
			else
				Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_BOOMERANG_2);

			collectibles		= new List<Collectible>();
			isReturning			= false;
			timer				= 0;
			physics.Velocity	= Angles.ToVector(angle) * speed;

			tileLocation = new Point2I(-1, -1);
		}

		public void Intercept() {
			BeginReturn();
		}

		public override void OnCollideRoomEdge() {
			BeginReturn();
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			// Create cling effect.
			Effect effect = new Effect(GameData.ANIM_EFFECT_CLING, DepthLayer.EffectCling);
			RoomControl.SpawnEntity(effect, position, zPosition);
			BeginReturn();
		}

		public override void OnCollideMonster(Monster monster) {
			monster.TriggerInteraction(InteractionType.Boomerang, this);
		}

		public override void Update() {
			// Check for boomerangable tiles.
			if (level == Item.Level2) {
				Point2I tileLoc = RoomControl.GetTileLocation(position);
				if (tileLoc != tileLocation && RoomControl.IsTileInBounds(tileLoc)) {
					Tile tile = RoomControl.GetTopTile(tileLoc);
					if (tile != null) {
						tile.OnBoomerang();
					}
				}
				tileLocation = tileLoc;
			}

			if (isReturning) {
				// Return to player.
				Vector2F trajectory = RoomControl.Player.Center - Center;
				if (trajectory.Length <= speed) {
					Destroy();
					for (int i = 0; i < collectibles.Count; i++)
						collectibles[i].Collect();
				}
				else {
					physics.Velocity = trajectory.Normalized * speed;
				}
			}
			else {

				// Update return timer.
				timer++;
				if (timer > returnDelay)
					BeginReturn();
			}

			// Collide with collectibles.
			CollisionIterator iterator = new CollisionIterator(this, typeof(Collectible), CollisionBoxType.Soft);
			for (iterator.Begin(); iterator.IsGood(); iterator.Next()) {
				Collectible collectible = iterator.CollisionInfo.Entity as Collectible;
				if (collectible.IsPickupable) {
					collectibles.Add(collectible);
					collectible.Destroy();
					BeginReturn();
				}
			}

			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);

			for (int i = 0; i < collectibles.Count; i++) {
				Collectible collectible = collectibles[i];
				collectible.SetPositionByCenter(Center);
				collectible.ZPosition = zPosition;
				float percent = i / (float) collectibles.Count;
				collectible.Graphics.Draw(g, Graphics.CurrentDepthLayer);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsReturning {
			get { return isReturning; }
		}

		public float Speed {
			get { return speed; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	
	public delegate void CollisionResponse();

	public enum ProjectileType {
		Physical = 0,	// Can always be deflected
		Magic,			// Can only be deflected by shields
		Beam,			// Can only be deflected by the Mirror Shield
		NotDeflectable	// Can NEVER be deflected.
	}

	public class Projectile : Entity, IInterceptable {

		protected int		angle;
		protected int		direction;
		protected Entity	owner;
		protected Tile      tileOwner;
		protected bool		syncAnimationWithAngle;
		protected bool		syncAnimationWithDirection;
		protected Animation crashAnimation;
		protected bool		bounceOnCrash;
		protected ProjectileType projectileType;

		private event Action eventCollision;
		private event Action eventLand;
		
		private Point2I tileLocation;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Projectile() {
			EnablePhysics(PhysicsFlags.Flying);

			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= false;

			owner			= null;
			tileOwner		= null;
			eventCollision	= null;
			eventLand		= null;
			angle			= 0;
			direction		= 0;

			crashAnimation		= null;
			bounceOnCrash		= false;

			projectileType = ProjectileType.Physical;
			// Prevent collision snapping from tile owner.
			Physics.CustomTileIsNotSolidCondition = (Tile tile) => {
				return (tile == null || tile != tileOwner);
			};
		}
		
		
		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------
		
		public virtual void Intercept() {
			Destroy();
		}

		protected virtual void OnCrash() { }


		//-----------------------------------------------------------------------------
		// Projectile Methods
		//-----------------------------------------------------------------------------

		protected void Crash(bool isInitialCollision) {
			if (crashAnimation != null) {
				// Create crash effect.
				Effect effect;

				if (bounceOnCrash) {
					effect = new Effect();
					effect.CreateDestroyTimer(32);
					effect.EnablePhysics(PhysicsFlags.HasGravity);
					if (!isInitialCollision)
						effect.Physics.Velocity = Angles.ToVector(Angles.Reverse(Angle)) * 0.25f;
					effect.Physics.ZVelocity	= 1.0f;
					effect.Physics.Gravity		= 0.07f;
					effect.Graphics.PlayAnimation(crashAnimation);
				}
				else {
					effect = new Effect(crashAnimation, Graphics.DepthLayer);
				}
				
				effect.Graphics.IsShadowVisible = false;
				effect.Graphics.DepthLayer = Graphics.DepthLayer;

				RoomControl.SpawnEntity(effect, position);
				DestroyAndTransform(effect);
			}
			else {
				Destroy();
			}

			OnCrash();
		}
		
		protected void CheckInitialCollision() {
			if (physics.IsPlaceMeetingSolid(position, physics.CollisionBox)) {
				Point2I tileLocation = RoomControl.GetTileLocation(Position);
				Tile tile = RoomControl.GetTopTile(tileLocation);
				if (tile != TileOwner)
					OnCollideTile(tile, true);
			}
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		public virtual void OnCollideRoomEdge() { }

		public virtual void OnCollideTile(Tile tile, bool isInitialCollision) { }

		public virtual void OnCollideMonster(Monster monster) { }

		public virtual void OnCollidePlayer(Player player) { }

		public virtual void OnCollideSolidEntity(Entity entity) { }


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnLand() {
			if (eventLand != null)
				eventLand();
		}

		public override void Initialize() {
			base.Initialize();

			if (syncAnimationWithDirection)
				Graphics.SubStripIndex = direction;
			else if (syncAnimationWithAngle)
				Graphics.SubStripIndex = angle;
			
			tileLocation = new Point2I(-1, -1);
		}

		public override void Update() {

			// Check if collided.
			if (physics.IsColliding && eventCollision != null) {
				eventCollision();
				if (IsDestroyed)
					return;
			}

			// Collide with tiles.
			if (physics.IsColliding) {
				CollisionType type = CollisionType.RoomEdge;
				Tile tile = null;
				
				foreach (CollisionInfo collision in Physics.GetCollisions()) {
					type = collision.Type;
					tile = collision.Tile;
					break;
				}

				if (tile != null && tile != TileOwner) {
					if (owner == RoomControl.Player) {
						tile.OnHitByProjectile(this);
						if (IsDestroyed)
							return;
					}
					OnCollideTile(tile, false);
					if (IsDestroyed)
						return;
				}
				else if (type == CollisionType.RoomEdge) {
					OnCollideRoomEdge();
					if (IsDestroyed)
						return;
				}
			}

			// Notify surface tiles the projectile is hovering over.
			Point2I tileLoc = RoomControl.GetTileLocation(position);
			if (tileLoc != tileLocation && RoomControl.IsTileInBounds(tileLoc) && zPosition < 10.0f) { // TODO: magic number
				Tile tile = RoomControl.GetTopTile(tileLoc);
				if (tile != null) {
					tile.OnHitByProjectile(this);
					if (IsDestroyed)
						return;
				}
			}
			tileLocation = tileLoc;

			if (owner is Player) {
				// Collide with monster tools.
				foreach (Monster monster in RoomControl.GetEntitiesOfType<Monster>()) {
					if (monster.IsPassable)
						continue;
					foreach (UnitTool tool in monster.EquippedTools) {
						if (Physics.PositionedCollisionBox.Intersects(tool.PositionedCollisionBox)) {
							tool.OnHitProjectile(this);
							if (IsDestroyed)
								return;
						}
					}
				}

				// Collide with monsters.
				foreach (Monster monster in Physics.GetEntitiesMeeting<Monster>(CollisionBoxType.Soft)) {
					if (monster.IsPassable)
						continue;
					OnCollideMonster(monster);
					if (IsDestroyed)
						return;
				}
			}
			else {
				Player player = RoomControl.Player;
				
				// Collide with the player's tools.
				foreach (UnitTool tool in player.EquippedTools) {
					if (Physics.PositionedCollisionBox.Intersects(tool.PositionedCollisionBox)) {
						tool.OnHitProjectile(this);
						if (IsDestroyed)
							return;
					}
				}
				
				// Collide with the player.
				if (!player.IsPassable && Physics.IsMeetingEntity(player, CollisionBoxType.Soft)) {
					OnCollidePlayer(player);
					if (IsDestroyed)
						return;
				}
			}

			// Collide with solid entities
			foreach (Entity entity in Physics.GetSolidEntitiesMeeting<Entity>(CollisionBoxType.Hard)) {
				OnCollideSolidEntity(entity);
				if (IsDestroyed)
					return;
			}
			


			if (syncAnimationWithDirection)
				Graphics.SubStripIndex = direction;
			else if (syncAnimationWithAngle)
				Graphics.SubStripIndex = angle;
			
			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Angle {
			get { return angle; }
			set {
				angle = value;
				if (angle % 2 == 0)
					direction = angle / 2;
			}
		}
		
		public int Direction {
			get { return direction; }
			set {
				direction = value;
				angle = Directions.ToAngle(direction);
			}
		}

		public Entity Owner {
			get { return owner; }
			set { owner = value; }
		}

		public Tile TileOwner {
			get { return tileOwner; }
			set { tileOwner = value; }
		}

		public ProjectileType ProjectileType {
			get { return projectileType; }
			set { projectileType = value; }
		}

		public event Action EventCollision {
			add { eventCollision += value; }
			remove { eventCollision -= value; }
		}

		public event Action EventLand {
			add { eventLand += value; }
			remove { eventLand -= value; }
		}
	}
}

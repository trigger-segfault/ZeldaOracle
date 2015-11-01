using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	
	public delegate void CollisionResponse();

	public class Projectile : Entity {

		protected int		angle;
		protected int		direction;
		protected Entity	owner;
		protected bool		syncAnimationWithAngle;
		protected bool		syncAnimationWithDirection;
		protected Animation	crashAnimation;
		protected bool		bounceOnCrash;

		private event Action eventCollision;
		private event Action eventLand;



		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Projectile() {
			EnablePhysics();

			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= false;

			owner			= null;
			eventCollision	= null;
			eventLand		= null;
			angle			= 0;
			direction		= 0;

			Graphics.IsRipplesEffectVisible	= false;
			Graphics.IsGrassEffectVisible	= false;

			crashAnimation	= null;
			bounceOnCrash	= false;
		}
		

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
		}
		
		protected void CheckInitialCollision() {
			if (physics.IsPlaceMeetingSolid(position, physics.CollisionBox)) {
				Point2I tileLocation = RoomControl.GetTileLocation(Position);
				Tile tile = RoomControl.GetTopTile(tileLocation);
				OnCollideTile(tile, true);
			}
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		public virtual void OnCollideRoomEdge() {

		}

		public virtual void OnCollideTile(Tile tile, bool isInitialCollision) {

		}

		public virtual void OnCollideMonster(Monster monster) {

		}

		public virtual void OnCollidePlayer(Player player) {

		}


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
		}

		public override void Update() {
			base.Update();

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
				for (int i = 0; i < 4; i++) {
					if (Physics.CollisionInfo[i].IsColliding) {
						type = Physics.CollisionInfo[i].Type;
						tile = Physics.CollisionInfo[i].Tile;
						break;
					}
				}
				if (tile != null) {
					OnCollideTile(tile, false);
					if (IsDestroyed)
						return;
				}
				else if (type == CollisionType.RoomEdge)
					OnCollideRoomEdge();
			}

			if (owner is Player) {
				// Collide with monsters.
				CollisionIterator iterator = new CollisionIterator(this, typeof(Monster), CollisionBoxType.Soft);
				for (iterator.Begin(); iterator.IsGood(); iterator.Next()) {
					OnCollideMonster(iterator.CollisionInfo.Entity as Monster);
					if (IsDestroyed)
						return;
				}
			}
			else {
				// Collide with the player.
				if (Physics.IsMeetingEntity(RoomControl.Player, CollisionBoxType.Soft)) {
					OnCollidePlayer(RoomControl.Player);
				}
			}
			
			if (syncAnimationWithDirection)
				Graphics.SubStripIndex = direction;
			else if (syncAnimationWithAngle)
				Graphics.SubStripIndex = angle;
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

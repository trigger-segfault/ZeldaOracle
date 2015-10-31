using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
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
		}
		

		//-----------------------------------------------------------------------------
		// Projectile Methods
		//-----------------------------------------------------------------------------
		
		protected void CheckInitialCollision() {
			if (physics.IsPlaceMeetingSolid(position, physics.CollisionBox)) {
				Point2I tileLocation = RoomControl.GetTileLocation(Origin);
				Tile tile = RoomControl.GetTopTile(tileLocation);
				OnCollideTile(tile, true);
			}
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------
		
		public virtual void Intercept() {

		}

		public virtual void OnCollideRoomEdge() {

		}

		public virtual void OnCollideTile(Tile tile, bool isInitialCollision) {

		}

		public virtual void OnCollideMonster(Monster monster) {

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

			// Collide with monsters.
			if (owner is Player) {


				CollisionIterator iterator = new CollisionIterator(this, typeof(Monster), CollisionBoxType.Soft);
				for (iterator.Begin(); iterator.IsGood(); iterator.Next()) {
					OnCollideMonster(iterator.CollisionInfo.Entity as Monster);
					if (IsDestroyed)
						return;
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

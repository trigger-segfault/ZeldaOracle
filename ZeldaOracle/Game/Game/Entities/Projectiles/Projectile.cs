using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
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

			syncAnimationWithAngle = false;
			syncAnimationWithDirection = false;

			owner			= null;
			eventCollision	= null;
			eventLand		= null;
			angle			= 0;
			direction		= 0;

			Graphics.IsRipplesEffectVisible	= false;
			Graphics.IsGrassEffectVisible	= false;
		}
		

		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------
		
		public virtual void OnCollideTile(Tile tile) {

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
					OnCollideTile(tile);
					if (IsDestroyed)
						return;
				}
			}

			// Collide with monsters.
			if (owner is Player) {
				for (int i = 0; i < RoomControl.Entities.Count; i++) {
					Monster monster = RoomControl.Entities[i] as Monster;
					if (monster != null && physics.IsSoftMeetingEntity(monster)) {
						OnCollideMonster(monster);
						if (IsDestroyed)
							return;
					}
				}
			}
			
			if (syncAnimationWithDirection)
				Graphics.SubStripIndex = direction;
			else if (syncAnimationWithAngle)
				Graphics.SubStripIndex = angle;
		}

		public override void Draw(Common.Graphics.Graphics2D g) {
			base.Draw(g);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

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
		// Overridden methods
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

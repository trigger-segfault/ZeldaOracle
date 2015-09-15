using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Projectiles {
	
	public delegate void CollisionResponse();

	public class Projectile : Entity {

		private int		angle;
		private Entity	owner;
		private event CollisionResponse eventCollision;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Projectile() {
			EnablePhysics();

			angle = 0;
			owner = null;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
		}

		public override void Update(float ticks) {

			base.Update(ticks);

			// Check if collided.
			if (physics.IsColliding && eventCollision != null) {
				eventCollision();
			}
		}

		public override void Draw(Common.Graphics.Graphics2D g) {
			base.Draw(g);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Angle {
			get { return angle; }
			set { angle = value; }
		}

		public Entity Owner {
			get { return owner; }
			set { owner = value; }
		}

		public event CollisionResponse EventCollision {
			add { eventCollision += value; }
			remove { eventCollision -= value; }
		}
	}
}

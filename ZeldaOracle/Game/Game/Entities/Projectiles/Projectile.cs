using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Projectiles {
	
	public delegate void CollisionResponse();

	public class Projectile : Entity {

		private int		angle;
		private Entity	owner;
		private event Action eventCollision;
		private event Action eventLand;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Projectile() {
			EnablePhysics();

			eventCollision = null;
			eventLand = null;
			angle = 0;
			owner = null;

			Graphics.IsRipplesEffectVisible = false;
			Graphics.IsGrassEffectVisible = false;
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
		}

		public override void Update() {

			base.Update();

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

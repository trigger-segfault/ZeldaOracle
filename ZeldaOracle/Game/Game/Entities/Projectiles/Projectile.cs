using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Projectiles {
	
	public class Projectile : Entity {

		private int		angle;
		private Entity	owner;


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
		}

		public override void Draw(Common.Graphics.Graphics2D g) {
			base.Draw(g);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		private int Angle {
			get { return angle; }
			set { angle = value; }
		}

		private Entity Owner {
			get { return owner; }
			set { owner = value; }
		}
	}
}

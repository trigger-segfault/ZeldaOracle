using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Entities {
	public class NPC : Entity {
		


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public NPC() {
			EnablePhysics(PhysicsFlags.Solid | PhysicsFlags.HasGravity);
			Physics.CollisionBox = new Rectangle2F(2, 0, 16, 14);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			
		}

	}
}

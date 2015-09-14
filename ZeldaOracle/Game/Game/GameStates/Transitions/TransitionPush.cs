using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.GameStates.Transitions {
	
	public class TransitionPush : RoomTransition {

		private RoomControl roomOld;
		private RoomControl roomNew;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TransitionPush(RoomControl roomOld, RoomControl roomNew) :
			base()
		{
			
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {

		}

		public override void Update(float timeDelta) {

		}

		public override void Draw(Graphics2D g) {
			//roomOld.Draw(g);
			//roomNew.Draw(g);
		}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.GameStates.Transitions {
	
	public class RoomTransition : GameState {
		protected RoomControl	roomOld;
		protected RoomControl	roomNew;
		protected Player		player;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RoomTransition(RoomControl roomOld, RoomControl roomNew) :
			base()
		{
			this.roomOld	= roomOld;
			this.roomNew	= roomNew;
			this.player		= roomOld.Player;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			player = roomOld.Player;
		}

		public override void Update() {

		}

		public override void Draw(Graphics2D g) {

		}

	}
}

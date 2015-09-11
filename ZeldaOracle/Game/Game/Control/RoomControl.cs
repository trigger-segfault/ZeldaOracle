using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.Players;

namespace ZeldaOracle.Game.Control {

	// Handles the main zelda gameplay within a room.
	public class RoomControl : GameState {

		private World	world;
		private Level	level;
		private Room	room;
		private Point2I	roomLocation;
		private Player	player;
		private List<Entity> entities;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomControl() {
			world			= null;
			level			= null;
			room			= null;
			player			= null;
			roomLocation	= Point2I.Zero;
			entities		= new List<Entity>();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			
		}
		
		public override void OnEnd() {
			
		}

		public override void Update(float timeDelta) {

		}

		public override void Draw(float timeDelta) {

		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public World World {
			get { return world; }
		}

		public Level Level {
			get { return level; }
		}

		public Room Room {
			get { return room; }
		}

		public Player Player {
			get { return player; }
		}
	}
}

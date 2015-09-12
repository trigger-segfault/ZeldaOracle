using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {

	// Handles the main Zelda gameplay within a room.
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
			player = new Player();
			player.Initialize(this);
			entities.Add(player);
		}
		
		public override void OnEnd() {
			
		}

		public override void Update(float timeDelta) {
			// TODO: Check for opening pause menu or map screens.

			// Update entities.
			for (int i = 0; i < entities.Count; ++i) {
				entities[i].Update(timeDelta);
			}

			// TODO: Update tiles.
		}

		public override void Draw(Graphics2D g) {
			// TODO: Draw tiles.
			// TODO: Draw entities.
			
			// Draw entities.
			for (int i = 0; i < entities.Count; ++i) {
				entities[i].Draw(g);
			}

			// TODO: Draw HUD.
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// The world context of the game.
		public World World {
			get { return world; }
		}

		// The current level that contains the room the player is in.
		public Level Level {
			get { return level; }
		}

		// The current room the player is in.
		public Room Room {
			get { return room; }
		}

		// The player entity (NOTE: this can be null)
		public Player Player {
			get { return player; }
		}
	}
}

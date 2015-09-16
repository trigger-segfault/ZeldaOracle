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
	
	public class TransitionPush : RoomTransition {
		private const int	TRANSITION_DELAY			= 8;		// Ticks
		private const int	TRANSITION_SPEED			= 4;		// Pixels per tick
		private const float	TRANSITION_PLAYER_HSPEED	= 0.38f;	// Pixels per tick
		private const float	TRANSITION_PLAYER_VSPEED	= 0.5f;		// Pixels per tick
		
		private int timer;
		private int distance;
		private int direction;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TransitionPush(RoomControl roomOld, RoomControl roomNew, int direction) :
			base(roomOld, roomNew)
		{
			this.direction = direction;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			timer		= 0;
			distance	= 0;
		}

		public override void Update() {
			int delay = 8;
			int speed = 4;
			int maxDistance = GameSettings.VIEW_SIZE[direction % 2];

			float playerSpeed = TRANSITION_PLAYER_HSPEED;
			if (Directions.IsVertical(direction))
				playerSpeed = TRANSITION_PLAYER_VSPEED;

			timer += 1;
			if (timer > delay) {
				distance += speed;
				
				player.Position += (Vector2F) Directions.ToPoint(direction) * playerSpeed;

				if (distance >= maxDistance) {
					gameManager.PopGameState();
					gameManager.PushGameState(roomNew);
				}
			}
		}


		public override void Draw(Graphics2D g) {

			Point2I panNew = -(Directions.ToPoint(direction) * distance);
			Point2I panOld = Directions.ToPoint(direction) * GameSettings.VIEW_SIZE;

			g.Translate(panNew);
			roomOld.Draw(g);
			
			g.Translate(panNew);
			g.Translate(panOld);
			roomNew.Draw(g);

		}

	}
}

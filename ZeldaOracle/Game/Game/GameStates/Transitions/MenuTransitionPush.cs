using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.GameStates.Transitions {
	
	public class MenuTransitionPush : GameState {
		private const int	TRANSITION_DELAY			= 8;		// Ticks
		private const int	TRANSITION_SPEED			= 4;		// Pixels per tick
		private Menu menuOld;
		private Menu menuNew;

		private int timer;
		private int distance;
		private int direction;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MenuTransitionPush(Menu menuOld, Menu menuNew, int direction) : base() {
			this.direction	= direction;
			this.menuOld	= menuOld;
			this.menuNew	= menuNew;
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
			int delay = 0;
			int speed = 12;
			int maxDistance = GameSettings.VIEW_SIZE[direction % 2];

			timer++;
			if (timer > delay) {
				distance += speed;
				
				if (distance >= maxDistance) {
					gameManager.PopGameState();
					gameManager.PushGameState(menuNew);
				}
			}
		}


		public override void Draw(Graphics2D g) {

			Point2I panNew = -(Directions.ToPoint(direction) * distance);
			Point2I panOld = Directions.ToPoint(direction) * GameSettings.VIEW_SIZE;

			g.Translate(panNew);
			menuOld.Draw(g);
			g.ResetTranslation();
			
			g.Translate(panNew);
			g.Translate(panOld);
			menuNew.Draw(g);

			g.ResetTranslation();
			GameControl.HUD.Draw(g, true);
		}

	}
}

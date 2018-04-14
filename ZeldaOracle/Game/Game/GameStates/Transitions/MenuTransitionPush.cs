using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Control.Menus;

namespace ZeldaOracle.Game.GameStates.Transitions {
	
	public class MenuTransitionPush : GameState {
		private const int TRANSITION_DELAY	= 8;		// Ticks
		private const int TRANSITION_SPEED	= 4;		// Pixels per tick
		private Menu menuOld;
		private Menu menuNew;

		private int timer;
		private int distance;
		private Direction direction;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MenuTransitionPush(Menu menuOld, Menu menuNew, Direction direction) {
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

		public override void AssignPalettes() {
			GameData.SHADER_PALETTE.TilePalette = GameData.PAL_MENU_DEFAULT;
			GameData.SHADER_PALETTE.EntityPalette = GameData.PAL_ENTITIES_MENU;
		}

		public override void Draw(Graphics2D g) {

			Point2I panNew = direction.ToPoint() * -distance;
			Point2I panOld = direction.ToPoint() * GameSettings.VIEW_SIZE;
			
			g.PushTranslation(panNew);
			menuOld.Draw(g);
			g.PopTranslation();
			
			g.PushTranslation(panNew + panOld);
			menuNew.Draw(g);
			g.PopTranslation();
			
			GameControl.HUD.Draw(g);
		}

	}
}

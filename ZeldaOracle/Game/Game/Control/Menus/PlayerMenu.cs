using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Menus {
	public class PlayerMenu : Menu {

		protected PlayerMenu previousMenu;
		protected PlayerMenu nextMenu;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerMenu(GameManager gameManager)
			: base(gameManager) {
			this.previousMenu	= null;
			this.nextMenu		= null;
			this.drawHUD		= true;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Update() {
			base.Update();
			if (Controls.Start.IsPressed()) {
				GameControl.CloseMenu(this);
			}
			if (Controls.Select.IsPressed()) {
				AudioSystem.PlaySound("UI/menu_next");
				gameManager.PopGameState();
				gameManager.PushGameState(new MenuTransitionPush(this, nextMenu, Directions.East));
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PlayerMenu PreviousMenu {
			get { return previousMenu; }
			set { previousMenu = value; }
		}

		public PlayerMenu NextMenu {
			get { return nextMenu; }
			set { nextMenu = value; }
		}
	}
}

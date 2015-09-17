using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Menus {
	public class MenuInventory : Menu {

		public Menu NextMenu;

		public MenuInventory(GameManager gameManager) : base() {
			this.gameManager	= gameManager;
			this.background = Resources.GetImage("UI/menu_weapons_b");
			this.NextMenu = null;
		}


		public override void OnBegin() {

		}

		public override void Update() {
			if (Controls.Start.IsPressed()) {
				GameControl.CloseMenu(this);
			}
			if (Controls.Select.IsPressed()) {
				gameManager.PopGameState();
				gameManager.PushGameState(new MenuTransitionPush(this, NextMenu, Directions.East));
			}
		}

		public override void Draw(Graphics2D g) {
			g.Translate(0, 16);
			g.DrawImage(background, Point2I.Zero);
			g.ResetTranslation();
			GameControl.HUD.Draw(g, true);
		}

	}
}

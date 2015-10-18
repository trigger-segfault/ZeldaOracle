using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.GameStates.Transitions {
	
	public class RoomTransitionSplit : RoomTransition {
		private const int TRANSITION_SPLIT_DELAY = 6;
		
		private Color	splitColor;
		private int[]	sideWidths;
		private int		timer;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RoomTransitionSplit() {
			this.sideWidths		= new int[2];
			this.splitColor		= new Color(248, 208, 136);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			timer = 0;
			for (int i = 0; i < 2; i++)
				sideWidths[i] = GameSettings.VIEW_WIDTH / 2;
			
			// Switch rooms immediately.
			SetupNewRoom();
			DestroyOldRoom();
		}

		public override void Update() {
			timer++;
			if (timer >= TRANSITION_SPLIT_DELAY) {
				sideWidths[(timer + 1) % 2] -= 8;
				if (sideWidths[0] <= 0 && sideWidths[1] <= 0) {
					EndTransition();
				}
			}
		}

		public override void Draw(Graphics2D g) {
			g.ResetTranslation();
			NewRoomControl.Draw(g);

			g.ResetTranslation();
			g.Translate(0, 16);
			g.FillRectangle(new Rectangle2F(0, 0, sideWidths[0], GameSettings.VIEW_HEIGHT), splitColor);
			g.FillRectangle(new Rectangle2F(GameSettings.VIEW_WIDTH - sideWidths[1], 0, sideWidths[1], GameSettings.VIEW_HEIGHT), splitColor);
		}

	}
}

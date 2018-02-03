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
		private const int TRANSITION_SPLIT_BEGIN_DELAY	= 3;
		private const int TRANSITION_SPLIT_MOVE_DELAY	= 7;
		private const int TRANSITION_SPLIT_END_DELAY	= 14;
		
		private ColorOrPalette	splitColor;
		private int[]			sideWidths;
		private int				timer;
		private int				endTimer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RoomTransitionSplit() {
			this.sideWidths		= new int[2];
			this.splitColor     = EntityColors.Tan;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			timer = 0;
			endTimer = 0;
			for (int i = 0; i < 2; i++)
				sideWidths[i] = GameSettings.VIEW_WIDTH / 2;
		}

		public override void Update() {
			timer++;
			if (timer == TRANSITION_SPLIT_BEGIN_DELAY) {
				// Switch rooms.
				SetupNewRoom(true);
				DestroyOldRoom();
			}
			else if (timer >= TRANSITION_SPLIT_BEGIN_DELAY + TRANSITION_SPLIT_MOVE_DELAY) {
				sideWidths[(timer + 1) % 2] -= 8;
				if (sideWidths[0] <= 0 && sideWidths[1] <= 0) {
					endTimer++;
					if (endTimer > TRANSITION_SPLIT_END_DELAY)
						EndTransition();
				}
			}
		}

		public override void AssignPalettes() {
			if (timer < TRANSITION_SPLIT_BEGIN_DELAY)
				OldRoomControl.AssignPalettes();
			else
				NewRoomControl.AssignPalettes();
		}

		public override void Draw(Graphics2D g) {
			//g.ResetTranslation();

			if (timer < TRANSITION_SPLIT_BEGIN_DELAY) {
				OldRoomControl.Draw(g);
			}
			else {
				NewRoomControl.Draw(g);

				//g.ResetTranslation();
				g.PushTranslation(0, GameSettings.HUD_HEIGHT);
				g.FillRectangle(new Rectangle2F(0, 0, sideWidths[0], GameSettings.VIEW_HEIGHT), splitColor);
				g.FillRectangle(new Rectangle2F(GameSettings.VIEW_WIDTH - sideWidths[1], 0, sideWidths[1], GameSettings.VIEW_HEIGHT), splitColor);
				g.PopTranslation();
			}
		}

	}
}

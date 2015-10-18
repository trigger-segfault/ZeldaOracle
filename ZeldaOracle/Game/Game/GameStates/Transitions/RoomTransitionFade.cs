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
	
	public class RoomTransitionFade : RoomTransition {
		private const int TRANSITION_FADE_DURATION = 40; // How long it takes to fade out or in.
		private const int TRANSITION_SWITCH_FADE_DELAY = 16; // Delay before fading back in after fading out.
		
		private int		timer;
		private bool	isBeginningFade;
		private Color	fadeColor;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RoomTransitionFade() {
			fadeColor = Color.White;
		}
		
		public RoomTransitionFade(Color fadeColor) {
			this.fadeColor = fadeColor;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			timer		= 0;
			isBeginningFade	= true;
		}

		public override void Update() {
			timer++;
			if (timer > TRANSITION_FADE_DURATION) {
				if (isBeginningFade) {
					// Switch rooms.
					SetupNewRoom();
					DestroyOldRoom();
					isBeginningFade = false;
					timer = 0;
				}
				else {
					EndTransition();
				}
			}
		}

		public override void Draw(Graphics2D g) {
			g.ResetTranslation();

			// Draw the room.
			if (isBeginningFade)
				OldRoomControl.Draw(g);
			else
				NewRoomControl.Draw(g);

			// Draw the fade.
			int t = timer;
			int delay = TRANSITION_SWITCH_FADE_DELAY / 2;
			if (!isBeginningFade)
				t = (TRANSITION_FADE_DURATION - delay) - t;
			float opacity = t / (float) (TRANSITION_FADE_DURATION - delay);
			opacity = GMath.Clamp(opacity, 0.0f, 1.0f);
			g.FillRectangle(GameSettings.SCREEN_BOUNDS, fadeColor * opacity);
		}

	}
}

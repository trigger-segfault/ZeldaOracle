using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.GameStates {
	
	public enum FadeType {
		FadeOut,
		FadeIn
	}

	public class TransitionFade : GameState {

		// Duration in seconds.
		private int duration;
		// The timer for the fading.
		private int timer;
		// Fade in or fade out.
		private FadeType type;
		// Color to fade to/from.
		private Color color;
		// The old game state to transition from.
		private GameState oldState;
		// The new game state to transition to.
		private GameState newState;



		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TransitionFade(Color color, int duration, GameState oldState, GameState newState) :
			base()
		{
			this.duration	= duration;
			this.timer		= 0;
			this.type		= FadeType.FadeOut;
			this.color		= color;
			this.oldState	= oldState;
			this.newState	= newState;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			timer = 0;
		}

		public override void Update() {
			timer++;
			if (timer >= duration) {
				if (type == FadeType.FadeOut) {
					type = FadeType.FadeIn;
					timer = 0;
				}
				else {
					End();
				}
			}
		}

		public override void Draw(Graphics2D g) {
			float opacity = (float)timer / (float)duration;
			if (type == FadeType.FadeIn)
				opacity = 1.0f - opacity;
			Color c = color * opacity;
			//c.A = (byte) (255.0f * opacity);
			if (type == FadeType.FadeOut) {
				oldState.Draw(g);
			}
			else {
				newState.Draw(g);
			}
			g.FillRectangle(GameSettings.SCREEN_BOUNDS, c);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.GameStates {
	
	public enum FadeType {
		FadeOut,
		FadeIn,
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
		// The game state to transition with.
		private GameState gameState;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TransitionFade(Color color, int duration, FadeType type, GameState gameState) {
			this.duration	= duration;
			this.timer		= 0;
			this.type		= type;
			this.color		= color;
			this.gameState	= gameState;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			timer = 0;
		}

		public override void Update() {
			timer++;
			if (timer >= duration)
				End();
		}

		public override void AssignPalettes() {
			gameState.AssignPalettes();
		}

		public override void Draw(Graphics2D g) {
			float opacity = (float)timer / (float)duration;
			if (type == FadeType.FadeIn)
				opacity = 1.0f - opacity;
			Color c = color * opacity;
			gameState.Draw(g);


			//g.ResetTranslation();
			g.FillRectangle(GameSettings.SCREEN_BOUNDS, c);
		}
	}
}

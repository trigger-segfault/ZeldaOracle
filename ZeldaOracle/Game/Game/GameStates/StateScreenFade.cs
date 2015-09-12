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

	public class StateScreenFade : GameState {
		
		private float		duration;	// Duration in seconds.
		private FadeType	type;		// Fade in or fade out.
		private Color		color;		// Color to fade to/from.
		private float		timer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public StateScreenFade(Color color, float duration, FadeType type) :
			base()
		{
			this.color     = color;
			this.duration  = duration;
			this.type      = type;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			timer = 0.0f;
		}

		public override void Update(float timeDelta) {
			timer += timeDelta;
			if (timer >= duration)
				End();
		}

		public override void Draw(Graphics2D g) {
			float opacity = timer / duration;
			if (type == FadeType.FadeIn)
				opacity = 1.0f - opacity;
			Color c = color;
			c.A = (byte) (255.0f * opacity);
			g.FillRectangle(GameSettings.SCREEN_BOUNDS, c);
		}
	}
}

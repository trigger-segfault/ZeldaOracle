using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Control.VisualEffects {
	/// <summary>The visual effect used when in top-down underwater environments.</summary>
	public class UnderwaterVisualEffect : RoomVisualEffect {

		//-----------------------------------------------------------------------------
		// Slice Class
		//-----------------------------------------------------------------------------

		/// <summary>A slice of the screen that can be shifted horizontally.
		/// For the underwater effect, there are four slices, one shifted to the left,
		/// another shifted to the right, and two not shifted at all.</summary>
		private struct Slice {
			/// <summary>Height of the slice.</summary>
			public int Height { get; }
			/// <summary>Horizontal shift amount.</summary>
			public int ShiftAmount { get; }

			/// <summary>Constructs the room slice.</summary>
			public Slice(int shiftAmount, int height) {
				ShiftAmount	= shiftAmount;
				Height		= height;
			}
		}
		
		/// <summary>The constant slices to draw in the efect.</summary>
		private static readonly Slice[] SLICES = {
			new Slice( 0, 22),
			new Slice(+1, 42),
			new Slice( 0, 22),
			new Slice(-1, 42),
		};
		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The timer used for offsetting the slices.</summary>
		private int timer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the underwater visual effect.</summary>
		public UnderwaterVisualEffect() {
			timer = 0;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws a single slice of the room.</summary>
		private void DrawSlice(Graphics2D g, Texture2D roomImage, Slice slice, int y) {
			Rectangle2I sourceRect = new Rectangle2I(
				0, y, GameSettings.VIEW_WIDTH, slice.Height);
			Rectangle2I destRect = sourceRect;
			sourceRect.X = slice.ShiftAmount;

			// If the slice is below the bottom of the screen, then
			// draw it at the top.
			if (sourceRect.Y >= GameSettings.VIEW_HEIGHT) {
				sourceRect.Y -= GameSettings.VIEW_HEIGHT;
				destRect.Y -= GameSettings.VIEW_HEIGHT;
			}

			g.DrawImage(roomImage, destRect, sourceRect);

			// If the slice is slit between the bottom and top of the screen,
			// then draw it a second time at the top.
			if (sourceRect.Bottom > GameSettings.VIEW_HEIGHT) {
				sourceRect.Y -= GameSettings.VIEW_HEIGHT;
				destRect.Y -= GameSettings.VIEW_HEIGHT;
				g.DrawImage(roomImage, destRect, sourceRect);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Updates the visual effect's state.</summary>
		public override void Update() {
			timer += 1;
		}

		/// <summary>Begins tile drawing for the visual effect. This is called once for
		/// drawing regular tiles and another time for drawing tiles' above sprites.</summary>
		public override void Begin(Graphics2D g, Vector2F position) {
			g.End();
			g.PushTranslation(-position);
			g.SetRenderTarget(GameData.RenderTargetGameTemp);
			g.Clear(Color.Transparent);
			g.Begin(GameSettings.DRAW_MODE_PALLETE);
		}

		/// <summary>Ends tile drawing for the visual effect. This is called once for
		/// drawing regular tiles and another time for drawing tiles' above sprites.</summary>
		public override void End(Graphics2D g, Vector2F position) {
			g.End();
			g.PopTranslation(); // -position
			g.SetRenderTarget(GameData.RenderTargetGame);
			g.Begin(GameSettings.DRAW_MODE_PALLETE);
			g.PushTranslation(-ViewPosition);
			Render(g, GameData.RenderTargetGameTemp);
			g.PopTranslation(); // -ViewPosition
		}

		/// <summary>Renders the visual effect after all drawing is completed.
		/// By default this is called in RoomVisualEffect.End.</summary>
		public override void Render(Graphics2D g, Texture2D roomImage) {
			int y = GameSettings.VIEW_HEIGHT - (timer % GameSettings.VIEW_HEIGHT);

			for (int i = 0; i < 4; i++) {
				Slice slice = SLICES[i];
				DrawSlice(g, roomImage, slice, y);
				y += slice.Height;
			}
		}
	}
}

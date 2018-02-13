using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Control {

	public class RoomVisualEffect {

		// TODO: create a base class for this, to have more visual effects.
		// Example of other effects: using the Harp of ages to warp into an 
		// area not allowed, the screen will become extremly wavey.

		//-----------------------------------------------------------------------------
		// Slice Class
		//-----------------------------------------------------------------------------

		// A slice of the screen that can be shifted horizontally.
		// For the underwater effect, there are four slices, one shifted to the
		// left, another shifted to the right, and two not shifted at all.
		private class Slice {
			// Height of the screen
			public int Height  { get; set; }
			// Horizontal shift amount
			public int ShiftAmount { get; set; }

			public Slice(int shiftAmount, int height) {
				this.ShiftAmount = shiftAmount;
				this.Height = height;
			}
		}
		

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------
		
		private RoomControl roomControl;
		private int timer = 0;

		private Slice[] slices = {
			new Slice( 0, 22),
			new Slice(+1, 42),
			new Slice( 0, 22),
			new Slice(-1, 42),
		};


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomVisualEffect() {
			this.timer = 0;
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void DrawSlice(Graphics2D g, RenderTarget2D roomImage, Slice slice, int y) {
			Rectangle2I sourceRect = new Rectangle2I(
				0, y, GameSettings.VIEW_WIDTH, slice.Height);
			Rectangle2I destRect = sourceRect;
			sourceRect.X = slice.ShiftAmount;

			// If the slice is below the bottom of the screen, then
			// draw it at the top.
			if (sourceRect.Y >= GameSettings.VIEW_HEIGHT)
			{
				sourceRect.Y -= GameSettings.VIEW_HEIGHT;
				destRect.Y -= GameSettings.VIEW_HEIGHT;
			}

			g.DrawImage(roomImage, destRect, sourceRect);

			// If the slice is slit between the bottom and top of the screen,
			// then draw it a second time at the top.
			if (sourceRect.Bottom > GameSettings.VIEW_HEIGHT)
			{
				sourceRect.Y -= GameSettings.VIEW_HEIGHT;
				destRect.Y -= GameSettings.VIEW_HEIGHT;
				g.DrawImage(roomImage, destRect, sourceRect);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------


		public virtual void Update() {
			timer += 1;
		}

		public virtual void Begin(Graphics2D g, Vector2F position) {
			g.End();
			g.PushTranslation(-position);
			g.SetRenderTarget(GameData.RenderTargetGameTemp);
			g.Clear(Color.Transparent);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
		}

		public virtual void End(Graphics2D g, Vector2F position) {
			g.End();
			g.PopTranslation(); // -position
			g.SetRenderTarget(GameData.RenderTargetGame);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			Render(g, GameData.RenderTargetGameTemp);
		}

		public virtual void Render(Graphics2D g, RenderTarget2D roomImage) {
			int y = 0;
			y = GameSettings.VIEW_HEIGHT -
				(timer % GameSettings.VIEW_HEIGHT);

			for (int i = 0; i < 4; i++) {
				Slice slice = slices[i];
				DrawSlice(g, roomImage, slice, y);
				y += slice.Height;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public RoomControl RoomControl {
			get { return roomControl; }
			set { roomControl = value; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A sprite that can be directly drawn to by using a render target.</summary>
	public class CanvasSprite : ISprite {

		/// <summary>The render target data for the canvas sprite.</summary>
		public RenderTarget RenderTarget { get; }
		/// <summary>The draw offset for the sprite.</summary>
		public Point2I DrawOffset { get; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a canvas sprite and render target.</summary>
		public CanvasSprite(Point2I size) {
			RenderTarget	= new RenderTarget(Resources.GraphicsDevice, size,
				SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
			DrawOffset		= Point2I.Zero;
		}

		/// <summary>Constructs a canvas sprite and render target with a draw offset.</summary>
		public CanvasSprite(Point2I size, Point2I drawOffset) {
			RenderTarget	= new RenderTarget(Resources.GraphicsDevice, size,
				SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
			DrawOffset		= drawOffset;
		}

		/// <summary>Constructs a canvas sprite from a render target.</summary>
		public CanvasSprite(RenderTarget renderTarget) {
			RenderTarget	= renderTarget;
			DrawOffset		= Point2I.Zero;
		}

		/// <summary>Constructs a canvas sprite from a render target and draw offset.</summary>
		public CanvasSprite(RenderTarget renderTarget, Point2I drawOffset) {
			RenderTarget	= renderTarget;
			DrawOffset		= drawOffset;
		}

		/// <summary>Constructs a copy of the canvas sprite.</summary>
		public CanvasSprite(CanvasSprite copy) {
			RenderTarget	= copy.RenderTarget;
			DrawOffset		= copy.DrawOffset;
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public SpritePart GetParts(SpriteSettings settings) {
			return new SpritePart(RenderTarget, RenderTarget.Bounds, DrawOffset/*,
				flipEffects, rotation*/);
		}

		/// <summary>Gets all sprites contained by this sprite including this one.</summary>
		public IEnumerable<ISprite> GetAllSprites() {
			yield return this;
		}

		/// <summary>Returns true if this sprite contains the specified sprite.</summary>
		public bool ContainsSubSprite(ISprite sprite) {
			return false;
		}

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new CanvasSprite(this);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteSettings settings) {
			return new Rectangle2I(DrawOffset, RenderTarget.Size);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get { return new Rectangle2I(DrawOffset, RenderTarget.Size); }
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		/// <summary>Begins drawing to the canvas sprite. CanvasSprite.End() must be
		/// called instead of Graphics2D.End().</summary>
		public Graphics2D Begin(DrawMode drawMode) {
			Graphics2D g = new Graphics2D(Resources.SpriteBatch);
			g.SetRenderTarget(RenderTarget);
			g.Begin(drawMode);
			return g;
		}

		/// <summary>Ends drawing to the canvas sprite. this must be called instead
		/// of Graphics2D.End().</summary>
		public void End(Graphics2D g) {
			g.End();
			g.SetRenderTarget(null);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the size of the canvas.</summary>
		public Point2I Size {
			get { return RenderTarget.Size; }
		}

		/// <summary>Gets the width of the canvas.</summary>
		public int Width {
			get { return RenderTarget.Width; }
		}

		/// <summary>Gets the height of the canvas.</summary>
		public int Height {
			get { return RenderTarget.Height; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A subclass of image that confirms the texture is a render target.</summary>
	public class RenderTarget : Image {
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an unassigned render target.</summary>
		public RenderTarget() : base() { }

		/// <summary>Constructs an render target with the specified render target.</summary>
		public RenderTarget(RenderTarget2D renderTarget) : base(renderTarget) { }

		/// <summary>Constructs an new render target with the specified size.</summary>
		public RenderTarget(GraphicsDevice graphicsDevice, int width, int height)
			: base(new RenderTarget2D(graphicsDevice, width, height)) { }

		/// <summary>Constructs an new render target with the specified size.</summary>
		public RenderTarget(GraphicsDevice graphicsDevice, Point2I size)
			: base(new RenderTarget2D(graphicsDevice, size.X, size.Y)) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(GraphicsDevice graphicsDevice, int width, int height,
			SurfaceFormat format)
			: base(new RenderTarget2D(graphicsDevice, width, height, false, format,
				DepthFormat.None)) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(GraphicsDevice graphicsDevice, Point2I size,
			SurfaceFormat format)
			: base(new RenderTarget2D(graphicsDevice, size.X, size.Y, false, format,
				DepthFormat.None)) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(GraphicsDevice graphicsDevice, int width, int height,
			SurfaceFormat format, RenderTargetUsage usage)
			: base(new RenderTarget2D(graphicsDevice, width, height, false, format,
				DepthFormat.None, 0, usage)) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(GraphicsDevice graphicsDevice, Point2I size,
			SurfaceFormat format, RenderTargetUsage usage)
			: base(new RenderTarget2D(graphicsDevice, size.X, size.Y, false, format,
				DepthFormat.None, 0, usage)) { }


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Used to auto-convert RenderTarget into XNA RenderTarget2Ds.</summary>
		public static implicit operator RenderTarget2D(RenderTarget image) {
			return (RenderTarget2D) image.Texture;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Information ----------------------------------------------------------------

		/// <summary>Gets the XNA render target of the render target.</summary>
		public RenderTarget2D RenderTarget2D {
			get { return (RenderTarget2D) Texture; }
		}
	}
}

using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A subclass of image that confirms the texture is a render target.</summary>
	public class RenderTarget : Image {
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an unassigned render target.</summary>
		private RenderTarget() { }

		/// <summary>Constructs an render target with the specified render target.</summary>
		private RenderTarget(RenderTarget2D renderTarget, bool asContent = true)
			: base(renderTarget, asContent) { }

		/// <summary>Constructs an new render target with the specified size.</summary>
		public RenderTarget(int width, int height, bool asContent = true)
			: base(new RenderTarget2D(ContentContainer.GraphicsDevice, width, height),
				  asContent) { }

		/// <summary>Constructs an new render target with the specified size.</summary>
		public RenderTarget(Point2I size, bool asContent = true)
			: base(new RenderTarget2D(ContentContainer.GraphicsDevice, size.X, size.Y),
				  asContent) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(int width, int height, SurfaceFormat format,
			bool asContent = true)
			: base(new RenderTarget2D(ContentContainer.GraphicsDevice, width, height,
				false, format, DepthFormat.None), asContent) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(Point2I size, SurfaceFormat format, bool asContent = true)
			: base(new RenderTarget2D(ContentContainer.GraphicsDevice, size.X, size.Y,
				false, format, DepthFormat.None), asContent) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(int width, int height, RenderTargetUsage usage,
			bool asContent = true)
			: base(new RenderTarget2D(ContentContainer.GraphicsDevice, width, height,
				false, SurfaceFormat.Color, DepthFormat.None, 0, usage), asContent) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(Point2I size, RenderTargetUsage usage,
			bool asContent = true)
			: base(new RenderTarget2D(ContentContainer.GraphicsDevice, size.X, size.Y,
				false, SurfaceFormat.Color, DepthFormat.None, 0, usage), asContent) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(int width, int height, SurfaceFormat format,
			RenderTargetUsage usage, bool asContent = true)
			: base(new RenderTarget2D(ContentContainer.GraphicsDevice, width, height,
				false, format, DepthFormat.None, 0, usage), asContent) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(Point2I size, SurfaceFormat format,
			RenderTargetUsage usage, bool asContent = true)
			: base(new RenderTarget2D(ContentContainer.GraphicsDevice, size.X, size.Y,
				false, format, DepthFormat.None, 0, usage), asContent) { }


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Used to auto-convert RenderTargets into XNA RenderTarget2Ds.</summary>
		public static implicit operator RenderTarget2D(RenderTarget renderTarget) {
			return (RenderTarget2D) renderTarget.texture;
		}

		/// <summary>Used to auto-convert RenderTargets into XNA Texture2Ds.</summary>
		public static implicit operator Texture2D(RenderTarget renderTarget) {
			return renderTarget.texture;
		}


		//-----------------------------------------------------------------------------
		// Resizing
		//-----------------------------------------------------------------------------

		/// <summary>Wraps a new render target of the specified size.</summary>
		public void Resize(int width, int height) {
			Resize(new Point2I(width, height));
		}

		/// <summary>Wraps a new render target of the specified size.</summary>
		public void Resize(Point2I newSize) {
			if (IsRootTarget)
				throw new InvalidOperationException("Cannot resize root render " +
					"target!");
			if (Size != newSize) {
				SurfaceFormat format = Format;
				RenderTargetUsage usage = Usage;
				Dispose();
				texture = new RenderTarget2D(ContentContainer.GraphicsDevice,
					newSize.X, newSize.Y, false, format, DepthFormat.None, 0, usage);
				if (asContent)
					ContentContainer.AddDisposable(texture);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Wraps the XNA RenderTarget2D into a RenderTarget.</summary>
		public static RenderTarget Wrap(RenderTarget2D renderTarget) {
			RenderTarget wrapper = new RenderTarget();
			// Assign the render target this way so that is not added
			// as an independent resource to the content database.
			wrapper.texture = renderTarget;
			return wrapper;
		}

		/// <summary>Loads the texture from content.</summary>
		public new static RenderTarget FromContent(string assetName) {
			RenderTarget renderTarget = new RenderTarget();
			// Assign the render target this way so that is not added
			// as an independent resource to the content database.
			renderTarget.texture =
				ContentContainer.ContentManager.Load<RenderTarget2D>(assetName);
			return renderTarget;
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public new static RenderTarget FromFile(string filePath,
			bool premultiply = false, bool asContent = true)
		{
			using (Stream stream = File.OpenRead(filePath))
				return FromStream(stream, premultiply, asContent);
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public static RenderTarget FromFile(string filePath, RenderTargetUsage usage,
			bool premultiply = false, bool asContent = true)
		{
			using (Stream stream = File.OpenRead(filePath))
				return FromStream(stream, usage, premultiply, asContent);
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public new static RenderTarget FromStreamAndSize(Stream stream,
			bool premultiply = false, bool asContent = true)
		{
			using (Stream memory = BinaryCounter.ReadStream(stream)) {
				if (memory.Length == 0)
					return null;
				return FromStream(memory, premultiply, asContent);
			}
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public static RenderTarget FromStreamAndSize(Stream stream,
			RenderTargetUsage usage, bool premultiply = false, bool asContent = true)
		{
			using (Stream memory = BinaryCounter.ReadStream(stream)) {
				if (memory.Length == 0)
					return null;
				return FromStream(memory, usage, premultiply, asContent);
			}
		}

		/// <summary>Loads the texture from the stream.</summary>
		public new static RenderTarget FromStream(Stream stream,
			bool premultiply = false, bool asContent = true)
		{
			RenderTarget2D target =
				Texture2DHelper.FromStream<RenderTarget2D>(stream);
			if (premultiply)
				Texture2DHelper.PremultiplyAlpha(target);
			return new RenderTarget(target, asContent);
		}

		/// <summary>Loads the texture from the stream.</summary>
		public static RenderTarget FromStream(Stream stream, RenderTargetUsage usage,
			bool premultiply = false, bool asContent = true)
		{
			RenderTarget2D target =
				Texture2DHelper.FromStream<RenderTarget2D>(stream, usage);
			if (premultiply)
				Texture2DHelper.PremultiplyAlpha(target);
			return new RenderTarget(target, asContent);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Dimensions -----------------------------------------------------------------

		/// <summary>Gets the bounding box of the image.</summary>
		public new Rectangle2I Bounds {
			get {
				if (texture == null)
					return ContentContainer.GraphicsDevice.Viewport.Bounds.ToRectangle2I();
				return texture.Bounds.ToRectangle2I();
			}
		}

		/// <summary>Gets the size of the image.</summary>
		public new Point2I Size {
			get { return Bounds.Size; }
		}

		/// <summary>Gets the width of the image.</summary>
		public new int Width {
			get {
				if (texture == null)
					return ContentContainer.GraphicsDevice.Viewport.Bounds.Width;
				return texture.Width;
			}
		}

		/// <summary>Gets the height of the image.</summary>
		public new int Height {
			get {
				if (texture == null)
					return ContentContainer.GraphicsDevice.Viewport.Bounds.Height;
				return texture.Height;
			}
		}
		
		// Information ----------------------------------------------------------------

		/// <summary>Gets the XNA render target of the render target.</summary>
		public RenderTarget2D RenderTarget2D {
			get { return (RenderTarget2D) texture; }
		}

		/// <summary>Gets if this target is referencing the root render target.</summary>
		public bool IsRootTarget {
			get { return texture == null; }
		}

		/// <summary>Gets how the render target is used.</summary>
		public RenderTargetUsage Usage {
			get { return RenderTarget2D.RenderTargetUsage; }
		}
	}
}

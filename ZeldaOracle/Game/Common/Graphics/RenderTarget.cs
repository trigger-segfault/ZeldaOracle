using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		public RenderTarget(RenderTarget2D renderTarget) : base(renderTarget) { }

		/// <summary>Constructs an new render target with the specified size.</summary>
		public RenderTarget(int width, int height)
			: base(new RenderTarget2D(Resources.GraphicsDevice, width, height)) { }

		/// <summary>Constructs an new render target with the specified size.</summary>
		public RenderTarget(Point2I size)
			: base(new RenderTarget2D(Resources.GraphicsDevice, size.X, size.Y)) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(int width, int height, SurfaceFormat format)
			: base(new RenderTarget2D(Resources.GraphicsDevice, width, height, false,
				format, DepthFormat.None)) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(Point2I size, SurfaceFormat format)
			: base(new RenderTarget2D(Resources.GraphicsDevice, size.X, size.Y, false,
				format, DepthFormat.None)) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(int width, int height, SurfaceFormat format,
			RenderTargetUsage usage)
			: base(new RenderTarget2D(Resources.GraphicsDevice, width, height, false,
				format, DepthFormat.None, 0, usage)) { }

		/// <summary>Constructs an new render target with the specified texture
		/// information.</summary>
		public RenderTarget(Point2I size, SurfaceFormat format,
			RenderTargetUsage usage)
			: base(new RenderTarget2D(Resources.GraphicsDevice, size.X, size.Y, false,
				format, DepthFormat.None, 0, usage)) { }


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Used to auto-convert RenderTarget into XNA RenderTarget2Ds.</summary>
		public static implicit operator RenderTarget2D(RenderTarget renderTarget) {
			return (RenderTarget2D) renderTarget.texture;
		}
		

		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Loads the texture from content.</summary>
		public new static RenderTarget FromContent(string assetName) {
			RenderTarget renderTarget = new RenderTarget();
			// Assign the render target this way so that is not added
			// as an independent resource to the content database.
			renderTarget.texture =
				Resources.ContentManager.Load<RenderTarget2D>(assetName);
			return renderTarget;
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public new static RenderTarget FromFile(string filePath,
			bool premultiply = false)
		{
			using (FileStream stream = File.Open(filePath, FileMode.Open))
				return FromStream(stream, premultiply);
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public static RenderTarget FromFile(string filePath, RenderTargetUsage usage,
			bool premultiply = false)
		{
			using (FileStream stream = File.Open(filePath, FileMode.Open))
				return FromStream(stream, usage, premultiply);
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public new static RenderTarget FromStream(Stream stream, int fileSize,
			bool premultiply = false)
		{
			BinaryReader reader = new BinaryReader(stream);
			using (Stream memory = new MemoryStream(reader.ReadBytes(fileSize)))
				return FromStream(memory, premultiply);
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public static RenderTarget FromStream(Stream stream, int fileSize,
			RenderTargetUsage usage, bool premultiply = false)
		{
			BinaryReader reader = new BinaryReader(stream);
			using (Stream memory = new MemoryStream(reader.ReadBytes(fileSize)))
				return FromStream(memory, usage, premultiply);
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public new static RenderTarget FromStreamAndSize(Stream stream,
			bool premultiply = false)
		{
			BinaryReader reader = new BinaryReader(stream);
			int fileSize = reader.ReadInt32();
			if (fileSize < 0)
				throw new IOException("Png file size in file is less than zero!");
			else if (fileSize == 0)
				return null;
			using (Stream memory = new MemoryStream(reader.ReadBytes(fileSize)))
				return FromStream(memory, premultiply);
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public static RenderTarget FromStreamAndSize(Stream stream,
			RenderTargetUsage usage, bool premultiply = false)
		{
			BinaryReader reader = new BinaryReader(stream);
			int fileSize = reader.ReadInt32();
			if (fileSize < 0)
				throw new IOException("Png file size in file is less than zero!");
			else if (fileSize == 0)
				return null;
			using (Stream memory = new MemoryStream(reader.ReadBytes(fileSize)))
				return FromStream(memory, usage, premultiply);
		}

		/// <summary>Loads the texture from the stream.</summary>
		public new static RenderTarget FromStream(Stream stream,
			bool premultiply = false)
		{
			RenderTarget2D target =
				Texture2DHelper.FromStream<RenderTarget2D>(stream);
			if (premultiply)
				Texture2DHelper.PremultiplyAlpha(target);
			return new RenderTarget(target);
		}

		/// <summary>Loads the texture from the stream.</summary>
		public static RenderTarget FromStream(Stream stream, RenderTargetUsage usage,
			bool premultiply = false)
		{
			RenderTarget2D target =
				Texture2DHelper.FromStream<RenderTarget2D>(stream, usage);
			if (premultiply)
				Texture2DHelper.PremultiplyAlpha(target);
			return new RenderTarget(target);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Information ----------------------------------------------------------------

		/// <summary>Gets the XNA render target of the render target.</summary>
		public RenderTarget2D RenderTarget2D {
			get { return (RenderTarget2D) texture; }
		}
	}
}

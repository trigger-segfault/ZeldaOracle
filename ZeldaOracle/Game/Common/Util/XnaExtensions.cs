using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ZeldaOracle.Common.Util {
	/// <summary>A static class for XNA Extensions.</summary>
	public static class XnaExtensions {

		/// <summary>Gets the current render target for the graphics device.</summary>
		public static RenderTarget2D GetRenderTarget(
			this GraphicsDevice graphicsDevice)
		{
			return graphicsDevice.GetRenderTargets()
				.LastOrDefault().RenderTarget as RenderTarget2D;
		}
	}
}

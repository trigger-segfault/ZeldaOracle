using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Util {
	/// <summary>A static class for Xna Extensions.</summary>
	public static class XnaExtensions {

		/// <summary>Gets the current render target for the graphics device.</summary>
		public static RenderTarget GetRenderTarget(
			this GraphicsDevice graphicsDevice)
		{
			return RenderTarget.Wrap(graphicsDevice.GetRenderTargets()
				.LastOrDefault().RenderTarget as RenderTarget2D);
		}
	}
}

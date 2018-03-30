using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Graphics.Shaders {
	/// <summary>A shader to shift the view horizontally using a sine wave.</summary>
	public class SineShiftShader : Shader {

		/// <summary>The color or palette background parameter.</summary>
		private ColorOrPalette background;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the sine-shift shader.</summary>
		public SineShiftShader(Effect effect) : base(effect) {
			background = Color.Black;
		}


		//-----------------------------------------------------------------------------
		// Parameters
		//-----------------------------------------------------------------------------

		/// <summary>Applies the parameters to the shader and graphics device.</summary>
		public override void ApplyParameters() {
			Color color = background.UnmappedColorSafe;
			Parameters["Background"].SetValue(color.ToXnaVector4());
			RenderTarget target = RenderTarget.Wrap(GraphicsDevice.GetRenderTarget());
			Parameters["TargetSize"].SetValue(target.Size.ToXnaVector2());
		}

		/// <summary>Resets the shader parameters.</summary>
		public override void ResetParameters() {
			Background = Color.Black;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Used to auto-convert Shaders into XNA Effects.</summary>
		public static implicit operator Effect(SineShiftShader shader) {
			return shader.effect;
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets or sets the pixel size of the render target.</summary>
		/*private Point2I TargetSize {
			get { return Parameters["TargetSize"].GetValueVector2().ToPoint2I(); }
			set { Parameters["TargetSize"].SetValue(value.ToXnaVector2()); }
		}*/


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the background to use outside of the visible pixels.</summary>
		public ColorOrPalette Background {
			get { return background; }
			set {
				background = value;
				Color color = background.UnmappedColor;
				Parameters["Background"].SetValue(color.ToXnaVector4());
			}
		}

		/// <summary>Gets or sets the vertical offset of the sine wave in pixels.</summary>
		public float Offset {
			get { return Parameters["Offset"].GetValueSingle(); }
			set { Parameters["Offset"].SetValue(value); }
		}

		/// <summary>Gets or sets the height of each sine wave in pixels.</summary>
		public float Height {
			get { return Parameters["Height"].GetValueSingle(); }
			set { Parameters["Height"].SetValue(value); }
		}

		/// <summary>Gets or sets the horizontal magnitude of the sine wave.</summary>
		public float Magnitude {
			get { return Parameters["Magnitude"].GetValueSingle(); }
			set { Parameters["Magnitude"].SetValue(value); }
		}
	}
}

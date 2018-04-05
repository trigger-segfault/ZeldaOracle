using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A compilation of all available settings for use with sprite batches.</summary>
	public class DrawMode {
		/// <summary>Gets or sets the method for sorting sprites as their draw functions
		/// are called.</summary>
		public SpriteSortMode SortMode { get; set; }
		/// <summary>Gets or sets the alpha blend state to use.</summary>
		public BlendState BlendState { get; set; }
		/// <summary>Gets or sets the sampler state to use.</summary>
		public SamplerState SamplerState { get; set; }
		/// <summary>Gets or sets the depth stencil state to use.</summary>
		public DepthStencilState DepthStencilState { get; set; }
		/// <summary>Gets or sets the rasterizer for shapes to use.</summary>
		public RasterizerState RasterizerState { get; set; }
		/// <summary>Gets or sets the shader effect to apply.</summary>
		public Effect Effect { get; set; }
		/// <summary>Gets or sets the matrix transformation to apply.</summary>
		public Matrix Transform { get; set; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the default draw mode.</summary>
		public DrawMode() {
			SortMode			= SpriteSortMode.Immediate;
			BlendState			= BlendState.NonPremultiplied;
			SamplerState		= SamplerState.LinearClamp;
			DepthStencilState	= DepthStencilState.None;
			RasterizerState		= RasterizerState.CullNone;
			Effect				= null;
			Transform			= Matrix.Identity;
		}

		/// <summary>Constructs a copy of the specified draw mode.</summary>
		public DrawMode(DrawMode copy) {
			SortMode			= copy.SortMode;
			BlendState			= copy.BlendState;
			SamplerState		= copy.SamplerState;
			DepthStencilState	= copy.DepthStencilState;
			RasterizerState		= copy.RasterizerState;
			Effect				= copy.Effect;
			Transform			= copy.Transform;
		}
	}
}

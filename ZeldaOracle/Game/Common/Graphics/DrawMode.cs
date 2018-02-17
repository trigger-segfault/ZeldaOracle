using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A compilation of all available settings for use with sprite batches.</summary>
	public class DrawMode {
		/// <summary>The method for sorting sprites as their draw functions are called.</summary>
		private SpriteSortMode sortMode;
		/// <summary>The alpha blend state to use.</summary>
		private BlendState blendState;
		/// <summary>The sampler state to use.</summary>
		private SamplerState samplerState;
		/// <summary>The depth stencil state to use.</summary>
		private DepthStencilState depthStencilState;
		/// <summary>The rasterizer for shapes to use.</summary>
		private RasterizerState rasterizerState;
		/// <summary>The shader effect to apply.</summary>
		private Effect effect;
		/// <summary>The matrix transformation to apply.</summary>
		private Matrix transform;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the default draw mode.</summary>
		public DrawMode() {
			sortMode			= SpriteSortMode.Immediate;
			blendState			= BlendState.NonPremultiplied;
			samplerState		= SamplerState.LinearClamp;
			depthStencilState	= DepthStencilState.None;
			rasterizerState		= RasterizerState.CullNone;
			effect				= null;
			transform			= Matrix.Identity;
		}

		/// <summary>Constructs a copy of the specified draw mode.</summary>
		public DrawMode(DrawMode copy) {
			sortMode			= copy.sortMode;
			blendState			= copy.blendState;
			samplerState		= copy.samplerState;
			depthStencilState	= copy.depthStencilState;
			rasterizerState		= copy.rasterizerState;
			effect				= copy.effect;
			transform			= copy.transform;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the method for sorting sprites as their draw functions
		/// are called.</summary>
		public SpriteSortMode SortMode {
			get { return sortMode; }
			set { sortMode = value; }
		}

		/// <summary>Gets or sets the alpha blend state to use.</summary>
		public BlendState BlendState {
			get { return blendState; }
			set { blendState = value; }
		}

		/// <summary>Gets or sets the sampler state to use.</summary>
		public SamplerState SamplerState {
			get { return samplerState; }
			set { samplerState = value; }
		}

		/// <summary>Gets or sets the depth stencil state to use.</summary>
		public DepthStencilState DepthStencilState {
			get { return depthStencilState; }
			set { depthStencilState = value; }
		}

		/// <summary>Gets or sets the rasterizer for shapes to use.</summary>
		public RasterizerState RasterizerState {
			get { return rasterizerState; }
			set { rasterizerState = value; }
		}

		/// <summary>Gets or sets the shader effect to apply.</summary>
		public Effect Effect {
			get { return effect; }
			set { effect = value; }
		}

		/// <summary>Gets or sets the matrix transformation to apply.</summary>
		public Matrix Transform {
			get { return transform; }
			set { transform = value; }
		}
	}
}
